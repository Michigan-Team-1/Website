using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Team1.Infrastructure.Resources;
using Team1.Model.UserIdentity;
using Services.Email;

namespace Team1.Web.Components.Account;

public class CustomEmailSender : IEmailSender<User>
{
  private readonly IEmailer emailSender;
  private readonly IStringLocalizer<SharedResource> sharedLocalizer;

  public CustomEmailSender(IEmailer emailer, IStringLocalizer<SharedResource> sharedLocalizer)
  {
    this.emailSender = emailer;
    this.sharedLocalizer = sharedLocalizer;
  }
  public Task SendConfirmationLinkAsync(User user, string email, string confirmationLink) => 
       emailSender.SendEmailAsync(email, sharedLocalizer["ConfirmEmailSubject"], string.Format(sharedLocalizer["ConfirmEmailMainContent"], confirmationLink));

  public Task SendPasswordResetLinkAsync(User user, string email, string resetLink) =>
      emailSender.SendEmailAsync(email, sharedLocalizer["ResetPasswordEmailSubject"], string.Format(sharedLocalizer["ResetPasswordEmailMainContent"], resetLink));

  public Task SendPasswordResetCodeAsync(User user, string email, string resetCode) =>
    emailSender.SendEmailAsync(email, sharedLocalizer["ResetPasswordEmailSubject"], string.Format(sharedLocalizer["PasswordResetCodeContent"], resetCode));
}
