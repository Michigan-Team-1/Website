using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Team1.Infrastructure;
using Team1.Infrastructure.Dtos;
using Team1.Infrastructure.Dtos.Token;
using Team1.Infrastructure.Dtos.Users;
using Team1.Infrastructure.Resources;
using Team1.Infrastructure.Services;
using Team1.Infrastructure.Services.ReCAPTCHA;
using Team1.Infrastructure.Services.Users;
using Team1.Model.UserIdentity;
using Team1.Web.Site.Infrastructure.UserIdentity;
using Team1.Web.Site.Infrastructure.UserIdentity.Policies;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Team1.Infrastructure.Services.Addresses;

namespace Team1.Web.Site.Controllers
{
    /// <summary>
    /// Contains all account related APIs
    /// </summary>
    [Produces("application/json")]
    [Route("api/Account")]
    public class AccountController : BaseController
    {
        private readonly IStringLocalizer<SharedValidationResource> _localizerValidation;
        private readonly IStringLocalizer<SharedResource> _localizer;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger _logger;
        private readonly JWTSettings _jWTSettings;

        /// <summary>
        /// Constructor for account controller.  Components should all be injected.
        /// </summary>
        public AccountController(IStringLocalizer<SharedResource> localizer, IStringLocalizer<SharedValidationResource> localizerValidation, UserManager<User> userManager,
            SignInManager<User> signInManager, ILogger<AccountController> logger, IOptions<JWTSettings> jWTSettings)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _localizerValidation = localizerValidation;
            _localizer = localizer;
            _logger = logger;
            _jWTSettings = jWTSettings.Value;
        }

        /// <summary>
        /// Login to the system
        /// </summary>
        /// <param name="dto">login information</param>
        /// <returns>returns token info or page to redirect to if successful</returns>
        [AllowAnonymous]
        [HttpPost(nameof(Login))]
        [ProducesResponseType(typeof(TokenDto), (int)System.Net.HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)System.Net.HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Login([FromBody]LoginDto dto)
        {
            if (!ModelState.IsValid)
                return CreateResponse(new BaseServiceResponse<LoginDto>(dto, System.Net.HttpStatusCode.BadRequest));
            
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return BadRequest(_localizerValidation["EmailPasswordAreIncorrect"]);
            var result = await _signInManager.PasswordSignInAsync(user, dto.Password, true, true);
            if (result.Succeeded)
            {
                var tokenDto = await GenerateTokenPackage(user);
                return Ok(tokenDto);
            }
            else if (result.RequiresTwoFactor)
            {
                if (!user.AuthyUserId.HasValue)
                    return Ok(new TokenDto { Token = "2faSetup", UserInfo = new TokenUserDto { PhoneNumber = user.PhoneNumber } });
                return Ok(new TokenDto { Token = "2fa" });
            }
            else if (result.IsLockedOut)
            {
                return BadRequest(_localizerValidation["UserLockedOut"]);
            }

            return BadRequest(_localizerValidation["EmailPasswordAreIncorrect"]);
        }

        /// <summary>
        /// Login with 2fa
        /// </summary>
        /// <param name="dto">2fa information</param>
        /// <returns>Token information if successful</returns>
        [AllowAnonymous]
        [HttpPost(nameof(LoginWith2fa))]
        [ProducesResponseType(typeof(TokenDto), (int)System.Net.HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)System.Net.HttpStatusCode.BadRequest)]
        public async Task<IActionResult> LoginWith2fa([FromBody]TwoFactorCodeDto dto)
        {
            //if (!ModelState.IsValid)
            //    return CreateResponse(new BaseServiceResponse<TwoFactorCodeDto>(dto, System.Net.HttpStatusCode.BadRequest));

            //var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            //if (user == null)
            //{
            //    return CreateResponse(new BaseServiceResponse<TwoFactorCodeDto>(dto, System.Net.HttpStatusCode.BadRequest));
            //}

            //dto.TwoFactorCode = dto.TwoFactorCode.Replace(" ", string.Empty).Replace("-", string.Empty);
            //dto.UserId = user.UserId;
            //dto.AuthyUserId = user.AuthyUserId;

            //var authyService = GetService<AuthyService>();
            //var authyResponse = await authyService.VerifyOneTimePassword(dto);
            //if (!authyResponse.Succeeded)
            //{
            //    return CreateResponse(authyResponse);
            //}

            //await _signInManager.SignInAsync(user, dto.RememberMe);

            //if (dto.RememberBrowser)
            //    await _signInManager.RememberTwoFactorClientAsync(user);

            //_logger.LogInformation("User with ID {UserId} logged in with 2fa.", user.UserId);
            //var tokenDto = await GenerateTokenPackage(user);
            //return Ok(tokenDto);
            return Ok();
        }

        /// <summary>
        /// Refreshes token information
        /// </summary>
        /// <param name="dto">Refresh Token information</param>
        /// <returns>refreshed token information</returns>
        [AllowAnonymous]
        [HttpPost(nameof(RefreshToken))]
        [ProducesResponseType(typeof(TokenDto), (int)System.Net.HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)System.Net.HttpStatusCode.BadRequest)]
        public async Task<IActionResult> RefreshToken([FromBody]RefreshTokenDto dto)
        {
            var userRefreshTokensGet = GetService<UserRefreshTokensGet>();

            var response = await userRefreshTokensGet.IsRefreshTokenValid(dto?.RefreshToken);
            if (response.Status != System.Net.HttpStatusCode.OK)
                return CreateResponse(response);

            var user = await _userManager.FindByIdAsync(response.Data.ToString());
            if (user == null)
                return BadRequest();

            var tokenDto = await GenerateTokenPackage(user);

            return Ok(tokenDto);
        }

        /// <summary>
        /// Logs out of the system
        /// </summary>
        /// <returns>200 success code</returns>
        [AllowAnonymous]
        [HttpPost(nameof(Logout))]
        [ProducesResponseType(typeof(string), (int)System.Net.HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Logout()
        {
            if (!UserPermissionService.UserClaimModel.IsAuthenticated)
                return Ok();

            var userRefreshTokenCreateUpdate = GetService<UserRefreshTokensCreateUpdate>();
            var response = await userRefreshTokenCreateUpdate.ExpireUserRefreshToken(UserPermissionService.UserClaimModel.UserId);
            if (response.Status != System.Net.HttpStatusCode.OK)
                return BadRequest();

            await _signInManager.SignOutAsync();

            return Ok();
        }

        /// <summary>
        /// Logs out of the system
        /// </summary>
        /// <returns>200 success code</returns>
        [AllowAnonymous]
        [HttpPost(nameof(Register))]
        [ProducesResponseType(typeof(string), (int)System.Net.HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (!ModelState.IsValid)
                return CreateResponse(new BaseServiceResponse<RegisterDto>(dto, System.Net.HttpStatusCode.BadRequest));

            var reCAPTCHAVerfiy = GetService<ReCAPTCHAVerfiy>();

            var recaptchaResponse = await reCAPTCHAVerfiy.Verify(nameof(Register), dto.RecaptchaToken);

            if (!recaptchaResponse.Succeeded)
                return CreateResponse(recaptchaResponse);

            if (recaptchaResponse.Data < 0.3m)
            {
                ModelState.AddModelError("Error", "Are you a bot?  Please try again.");
                return CreateResponse(new BaseServiceResponse<RegisterDto>(dto, System.Net.HttpStatusCode.BadRequest));
            }

            using (var transaction = await SpudContext.BeginTransactionAsync())
            {
                var usersCreateUpdate = GetService<UsersCreateUpdate>();
                var response = await usersCreateUpdate.RegisterUser(dto, _userManager, transaction);
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

                if (!response.Succeeded)
                {
                    return CreateResponse(response);
                }
            }

            _logger.LogInformation("User created a new account with password.");

            var callbackUrl = $"{BaseUrl}ConfirmEmail?UserId={dto.UserId}&code={System.Net.WebUtility.UrlEncode(dto.Code)}";

            var emailTemplateService = (Team1.Infrastructure.Services.Templates.Emails.EmailsCreate)HttpContext.RequestServices.GetService(typeof(Team1.Infrastructure.Services.Templates.Emails.EmailsCreate));

            var emailTemplateResponse = await emailTemplateService.GenerateEmail(BaseUrl,
                string.Format(_localizer["ConfirmEmailMainContent"], callbackUrl),
                string.Format(_localizer["TemplateEmailFooter"], dto.Email));

            var emailer = (Services.Email.IEmailer)HttpContext.RequestServices.GetService(typeof(Services.Email.IEmailer));
            await emailer.SendEmailAsync(dto.Email, _localizer["ConfirmEmailSubject"], emailTemplateResponse.Data);

            // email BoD about new registration
            emailTemplateResponse = await emailTemplateService.GenerateEmail(BaseUrl,
                string.Format(_localizer["NewRegistrationMainContent"], dto.FirstName, dto.LastName, dto.Email),
                string.Format(_localizer["SimpleTemplateEmailFooter"]));

            await emailer.SendEmailAsync(new[] { "prefect@team1.org", "viceprefect@team1.org", "secretary@team1.org" }, _localizer["NewRegistrationSubject"], emailTemplateResponse.Data);

            return Ok();
        }

        /// <summary>
        /// Confirms a users email
        /// </summary>
        /// <param name="userId">user's id</param>
        /// <param name="code">generated code from the system</param>
        /// <returns>ConfirmEmail or Error</returns>
        [AllowAnonymous]
        [HttpGet(nameof(ConfirmUserEmail))]
        [ProducesResponseType(typeof(string), (int)System.Net.HttpStatusCode.BadRequest)]
        public async Task<IActionResult> ConfirmUserEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return BadRequest();
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{userId}'.");
            }
            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded)
                return Ok();

            return BadRequest("Error confirming email.");
        }

        /// <summary>
        /// Generates forgot password email and sends it.
        /// </summary>
        /// <param name="dto">forgot password information</param>
        /// <returns>200 success code</returns>
        [AllowAnonymous]
        [HttpPost(nameof(ForgotPassword))]
        [ProducesResponseType(typeof(string), (int)System.Net.HttpStatusCode.BadRequest)]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            if (!ModelState.IsValid)
                return CreateResponse(new BaseServiceResponse<ForgotPasswordDto>(dto, System.Net.HttpStatusCode.BadRequest));
            
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
            {
                // Don't reveal that the user does not exist or is not confirmed
                return Ok();
            }

            var code = await _userManager.GeneratePasswordResetTokenAsync(user);

            var callbackUrl = $"{BaseUrl}resetPassword?UserId={user.UserId}&code={System.Net.WebUtility.UrlEncode(code)}";

            var emailTemplateService = (Team1.Infrastructure.Services.Templates.Emails.EmailsCreate)HttpContext.RequestServices.GetService(typeof(Team1.Infrastructure.Services.Templates.Emails.EmailsCreate));

            var emailTemplateResponse = await emailTemplateService.GenerateEmail(BaseUrl, 
                string.Format(_localizer["ResetPasswordEmailMainContent"], callbackUrl), 
                string.Format(_localizer["TemplateEmailFooter"], dto.Email));

            var emailer = (Services.Email.IEmailer)HttpContext.RequestServices.GetService(typeof(Services.Email.IEmailer));
            await emailer.SendEmailAsync(dto.Email, _localizer["ResetPasswordEmailSubject"], emailTemplateResponse.Data);

            return Ok();
        }

        /// <summary>
        /// Reset the a users password
        /// </summary>
        /// <param name="dto">reset password information</param>
        /// <returns>200 success code</returns>
        [AllowAnonymous]
        [HttpPost(nameof(ResetPassword))]
        [ProducesResponseType(typeof(string), (int)System.Net.HttpStatusCode.BadRequest)]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            if (!ModelState.IsValid)
                return CreateResponse(new BaseServiceResponse<ResetPasswordDto>(dto, System.Net.HttpStatusCode.BadRequest));

            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return Ok();
            }

            var result = await _userManager.ResetPasswordAsync(user, dto.Code, dto.Password);
            if (result.Succeeded)
            {
                var userCreateUpdate = GetService<UsersCreateUpdate>();
                var updateLastPasswordChangeDateTimeResponse = await userCreateUpdate.SaveLastPasswordChangeDateTime(user.UserId);
                return Ok();
            }

            AddErrors(result);
            return CreateResponse(new BaseServiceResponse<ResetPasswordDto>(dto, System.Net.HttpStatusCode.BadRequest));
        }

        /// <summary>
        /// Change the a users password
        /// </summary>
        /// <param name="dto">change password information</param>
        /// <returns>200 success code</returns>
        [HttpPost(nameof(ChangePassword))]
        [ProducesResponseType(typeof(string), (int)System.Net.HttpStatusCode.BadRequest)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            if (!ModelState.IsValid)
                return CreateResponse(new BaseServiceResponse<ChangePasswordDto>(dto, System.Net.HttpStatusCode.BadRequest));

            var user = await _userManager.FindByIdAsync(UserPermissionService.UserClaimModel.UserId.ToString());
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return Ok();
            }

            var result = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.Password);
            if (result.Succeeded)
            {
                var userCreateUpdate = GetService<UsersCreateUpdate>();
                var updateLastPasswordChangeDateTimeResponse = await userCreateUpdate.SaveLastPasswordChangeDateTime(user.UserId);
                return Ok();
            }

            AddErrors(result);
            return CreateResponse(new BaseServiceResponse<ChangePasswordDto>(dto, System.Net.HttpStatusCode.BadRequest));
        }

        /// <summary>
        /// Sends a password reset to the specified user
        /// Requires permission to access
        /// </summary>
        /// <param name="id">user's id</param>
        /// <param name="isNew">is it a new user</param>
        /// <returns>true on success</returns>
        [Authorize(Policy = PolicyNames.UserAddEditDelete)]
        [HttpGet("{id}/" + nameof(SendPasswordResetEmail) + "/{isNew}")]
        [ProducesResponseType(typeof(bool), (int)System.Net.HttpStatusCode.OK)]
        public async Task<IActionResult> SendPasswordResetEmail(int id, bool isNew)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
            {
                // Don't reveal that the user does not exist or is not confirmed
                return BadRequest(_localizerValidation["UserNotFound"]);
            }

            var code = await _userManager.GeneratePasswordResetTokenAsync(user);

            var callbackUrl = $"{BaseUrl}resetPassword?UserId={user.UserId}&code={System.Net.WebUtility.UrlEncode(code)}";
            
            var emailer = (Services.Email.IEmailer)HttpContext.RequestServices.GetService(typeof(Services.Email.IEmailer));
            var emailTemplateService = (Team1.Infrastructure.Services.Templates.Emails.EmailsCreate)HttpContext.RequestServices.GetService(typeof(Team1.Infrastructure.Services.Templates.Emails.EmailsCreate));
            var body = string.Format(isNew ? _localizer["PasswordSetupNewAccountEmailMainContent"] : _localizer["PasswordSetupEmailMainContent"], callbackUrl);
            var emailTemplateResponse = await emailTemplateService.GenerateEmail(BaseUrl,
                body,
                string.Format(_localizer["TemplateEmailFooter"], user.Email));
            
            var subject = isNew ? _localizer["PasswordSetupNewAccountEmailSubject"] : _localizer["PasswordSetupEmailSubject"];
            await emailer.SendEmailAsync(user.Email, subject, emailTemplateResponse.Data);

            return Ok(true);
        }

        /// <summary>
        /// Setting up two factor 
        /// </summary>
        /// <param name="dto">forgot password information</param>
        /// <returns>200 success code</returns>
        [AllowAnonymous]
        [HttpPost(nameof(TwoFactorSetup))]
        [ProducesResponseType(typeof(string), (int)System.Net.HttpStatusCode.BadRequest)]
        public async Task<IActionResult> TwoFactorSetup([FromBody] TwoFactorSetupDto dto)
        {
            if (!ModelState.IsValid)
                return CreateResponse(new BaseServiceResponse<TwoFactorSetupDto>(dto, System.Net.HttpStatusCode.BadRequest));

            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return Ok();
            }

            dto.UserId = user.UserId;
            dto.Email = user.Email;
            await _userManager.SetPhoneNumberAsync(user, dto.PhoneNumber);

            //if (_twilioSettings.AuthyEnabled)
            //{
            //    var authyService = GetService<AuthyService>();
            //    var authyResponse = await authyService.CreateAuthyUser(dto);
            //    if (!authyResponse.Succeeded)
            //        return CreateResponse(authyResponse);
            //}

            return Ok();
        }

        /// <summary>
        /// impersonate a company and/or user
        /// </summary>
        /// <param name="dto">info to impersonate</param>
        /// <returns>returns token info</returns>
        [Authorize(Policy = PolicyNames.CanImpersonate)]
        [HttpPost(nameof(Impersonate))]
        [ProducesResponseType(typeof(TokenDto), (int)System.Net.HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)System.Net.HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Impersonate([FromBody] ImpersonateDto dto)
        {
            if (dto.UserId.HasValue && dto.UserId.Value == UserPermissionService.UserClaimModel.UserIdOriginal)
                dto.UserId = null;

            var user = await _userManager.FindByIdAsync(UserPermissionService.UserClaimModel.UserIdOriginal.ToString());
            var tokenDto = await GenerateTokenPackage(user, dto?.UserId);
            return Ok(tokenDto);
        }

        #region helpers

        private async Task<List<System.Security.Claims.Claim>> GenerateClaims(User user, IList<RoleDto> roles, UserDto userDto)
        {
            var claims = Infrastructure.UserIdentity.UserClaimBuilder.GenerateClaims(user, roles, UserPermissionService.IpAddress);
            var userClaims = await _userManager.GetClaimsAsync(user);
            claims = claims.Union(userClaims).ToList();

            if (userDto != null)
                claims = Infrastructure.UserIdentity.UserClaimBuilder.ImpersonateUser(claims, userDto);

            return claims;
        }

        private JwtSecurityToken GenerateToken(List<System.Security.Claims.Claim> claims)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jWTSettings.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_jWTSettings.Issuer,
              _jWTSettings.Issuer,
              claims,
              expires: DateTime.UtcNow.AddMinutes(_jWTSettings.TokenExpirationMinutes),
              signingCredentials: creds);
            return token;
        }

        private async Task<TokenDto> GenerateTokenPackage(User user, int? userId = null)
        {
            var usersGet = GetService<UsersGet>();
            UserDto userToImpersonate = null;
            if (userId.HasValue)
                userToImpersonate = await usersGet.GetUser(userId.Value);

            var roles = await usersGet.GetUserRoles(user.UserId, UserPermissionService, true);
            var claims = await GenerateClaims(user, roles, userToImpersonate);
            JwtSecurityToken token = GenerateToken(claims);
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var tokenString = jwtSecurityTokenHandler.WriteToken(token);
            SecurityToken validatedToken = null;
            var userPrincipal = jwtSecurityTokenHandler.ValidateToken(tokenString, Startup.TokenValidationParameters, out validatedToken);
            UserPermissionService.Setup(new UserClaimBuilder(userPrincipal));
            var userRefreshTokenCreateUpdate = GetService<UserRefreshTokensCreateUpdate>();
            var response = await userRefreshTokenCreateUpdate.SaveUserRefreshToken(user.UserId);
            if (!response.Succeeded)
                return null;

            var userCreateUpdate = GetService<UsersCreateUpdate>();
            var saveIpResponse = await userCreateUpdate.SaveIpAddress(user.UserId);
            if (!saveIpResponse.Succeeded)
                return null;

            var updateLastSignInDateResponse = await userCreateUpdate.SaveLastSignInDateTime(user.UserId);
            if (!updateLastSignInDateResponse.Succeeded)
                return null;

            var tokenDto = new TokenDto()
            {
                RefreshToken = response.Data,
                Token = tokenString,
                ValidTo = token.ValidTo,
                Roles = roles.Select(s => s.Name).ToArray(),
                UserPolicies = new UserPoliciesDto(UserPermissionService.UserPolicies),
                UserInfo = new TokenUserDto()
                {
                    Email = userToImpersonate != null ? userToImpersonate.Email : user.Email,
                    FirstName = userToImpersonate != null ? userToImpersonate.FirstName : user.FirstName,
                    LastName = userToImpersonate != null ? userToImpersonate.LastName : user.LastName,
                    PhoneNumber = userToImpersonate != null ? userToImpersonate.PhoneNumber : user.PhoneNumber,

                    IsImpersonatingUser = userToImpersonate != null,
                }
            };

            return tokenDto;
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        #endregion 
    }
}
