using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Team1.Infrastructure.Resources;
using Team1.Model.UserIdentity;
using Services.Email;
using Team1.Infrastructure.Services.Templates.Emails;

namespace Team1.Web.Components.Account;

public class CustomEmailSender : IEmailSender<User>
{
  private readonly IEmailer emailSender;
  private readonly IStringLocalizer<SharedResource> sharedLocalizer;
  private readonly EmailsCreate emailCreate;
  private readonly string baseUrl;

  public CustomEmailSender(IEmailer emailer, IStringLocalizer<SharedResource> sharedLocalizer, EmailsCreate emailsCreate, IConfiguration configuration)
  {
    this.emailSender = emailer;
    this.sharedLocalizer = sharedLocalizer;
    this.emailCreate = emailsCreate;
    baseUrl = configuration.GetValue<string>("ApiUrl")!;
  }

  public async Task SendConfirmationLinkAsync(User user, string email, string confirmationLink)
  {
    var response = await emailCreate.GenerateEmail(baseUrl, string.Format(sharedLocalizer["ConfirmEmailMainContent"], confirmationLink), string.Format(sharedLocalizer["TemplateEmailFooter"], email));
    await emailSender.SendEmailAsync(email, sharedLocalizer["ConfirmEmailSubject"], response.Data);
  }

  public async Task SendPasswordResetLinkAsync(User user, string email, string resetLink)
  {
    var response = await emailCreate.GenerateEmail(baseUrl, string.Format(sharedLocalizer["ResetPasswordEmailMainContent"], resetLink), string.Format(sharedLocalizer["TemplateEmailFooter"], email));
    await emailSender.SendEmailAsync(email, sharedLocalizer["ResetPasswordEmailSubject"], response.Data);
  }

  public async Task SendPasswordResetCodeAsync(User user, string email, string resetCode)
  {
    var response = await emailCreate.GenerateEmail(baseUrl, string.Format(sharedLocalizer["PasswordResetCodeContent"], resetCode), string.Format(sharedLocalizer["TemplateEmailFooter"], email));
    await emailSender.SendEmailAsync(email, sharedLocalizer["ResetPasswordEmailSubject"], response.Data);
  }
}
