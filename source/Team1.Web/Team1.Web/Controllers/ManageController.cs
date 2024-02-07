using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Localization;
using System.Net;
using System.Text;
using Team1.Infrastructure.Dtos.Users;
using Team1.Infrastructure.Resources;
using Team1.Infrastructure.Services;
using Team1.Infrastructure.Services.Addresses;
using Team1.Infrastructure.Services.Users;
using Team1.Model.UserIdentity;
using Team1.Web.Common.UserIdentity.Policies;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

namespace Team1.Web.Controllers;

/// <summary>
/// Manage controller
/// </summary>
[Produces("application/json")]
[Authorize(Policy = PolicyNames.UserProfileEdit)]
[Route("api/Manage")]
public class ManageController : BaseController
{
  private readonly UserManager<User> userManager;
  private readonly IStringLocalizer<SharedResource> localizer;
  private readonly IEmailSender<User> emailSender;

  public ManageController(UserManager<User> userManager, IStringLocalizer<SharedResource> localizer, IEmailSender<User> EmailSender)
  {
    this.userManager = userManager;
    this.localizer = localizer;
    this.emailSender = EmailSender;
  }

  /// <summary>
  /// Get user profile
  /// </summary>
  /// <returns>user profile</returns>
  [HttpGet]
  [ProducesResponseType(typeof(UserDto), (int)HttpStatusCode.OK)]
  public async Task<IActionResult> GetUserProfile()
  {
    var service = GetService<UsersGet>();
    return Ok(await service.GetUserProfile());
  }

  /// <summary>
  /// Update an user profile
  /// </summary>
  /// <param name="dto">user object</param>
  /// <returns>updated user object</returns>
  [HttpPut]
  [ProducesResponseType(typeof(UserDto), (int)HttpStatusCode.OK)]
  public async Task<IActionResult> UpdateUserProfile([FromBody] UserDto dto)
  {
    return await SaveUser(dto);
  }

  /// <summary>
  /// Register a user
  /// </summary>
  /// <param name="dto">user object</param>
  /// <returns>updated user object</returns>
  [HttpPost("Register")]
  [AllowAnonymous]
  [ProducesResponseType(typeof(UserDto), (int)HttpStatusCode.OK)]
  public async Task<IActionResult> Register([FromBody] UserDto dto)
  {
    if (!ModelState.IsValid)
      return CreateResponse(new BaseServiceResponse<UserDto>(dto, System.Net.HttpStatusCode.BadRequest));

    using (var transaction = await DataContext.BeginTransactionAsync())
    {
      var service = GetService<UsersCreateUpdate>();
      var response = await service.RegisterUser(dto, userManager);
      if (response.Succeeded)
      {
        var addressService = GetService<AddressCreateUpdate>();
        var addressResponse = await addressService.SaveAddresses(dto.Addresses, dto.UserId);
        if (addressResponse.Succeeded)
          response.Data.Addresses = addressResponse.Data;
        else
          return CreateResponse(addressResponse);

        transaction.Commit();
      }

      if (response.Succeeded)
      {
        var callbackUrl = $"{BaseUrl}account/ConfirmEmail?UserId={dto.UserId}&Code={WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(dto.Code))}";

        var emailTemplateService = (Infrastructure.Services.Templates.Emails.EmailsCreate)HttpContext.RequestServices.GetService(typeof(Infrastructure.Services.Templates.Emails.EmailsCreate))!;

        var emailTemplateResponse = await emailTemplateService.GenerateEmail(BaseUrl,
            string.Format(localizer["ConfirmEmailMainContent"], callbackUrl),
            string.Format(localizer["TemplateEmailFooter"], dto.Email));

        var emailer = (Services.Email.IEmailer)HttpContext.RequestServices.GetService(typeof(Services.Email.IEmailer))!;
        await emailer.SendEmailAsync(dto.Email, localizer["ConfirmEmailSubject"], emailTemplateResponse.Data);

        // email BoD about new registration
        emailTemplateResponse = await emailTemplateService.GenerateEmail(BaseUrl,
            string.Format(localizer["NewRegistrationMainContent"], dto.FirstName, dto.LastName, dto.Email),
            string.Format(localizer["SimpleTemplateEmailFooter"]));

        await emailer.SendEmailAsync(new[] { "prefect@team1.org", "viceprefect@team1.org", "secretary@team1.org" }, localizer["NewRegistrationSubject"], emailTemplateResponse.Data);
      }

      return CreateResponse(response);
    }
  }

  #region private helpers

  private async Task<IActionResult> SaveUser(UserDto dto, bool newRegister = false)
  {
    if (!ModelState.IsValid)
      return CreateResponse(new BaseServiceResponse<UserDto>(dto, System.Net.HttpStatusCode.BadRequest));

    using (var transaction = await DataContext.BeginTransactionAsync())
    {
      var service = GetService<UsersCreateUpdate>();
      var response = await service.SaveUserProfile(dto);
      if (response.Succeeded)
      {
        var addressService = GetService<AddressCreateUpdate>();
        var addressResponse = await addressService.SaveAddresses(dto.Addresses, dto.UserId);
        if (addressResponse.Succeeded)
          response.Data.Addresses = addressResponse.Data;
        else
          return CreateResponse(addressResponse);

        transaction.Commit();
      }

      return CreateResponse(response);
    }
  }

  #endregion
}
