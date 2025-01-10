using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Team1.Infrastructure.UserIdentity;

namespace Team1.Web.Common.UserIdentity.Policies;

public class AnnouncementAddEditDeleteRequirement : IAuthorizationRequirement
{
}

public class AnnouncementAddEditDelete : AuthorizationHandler<AnnouncementAddEditDeleteRequirement>
{
  private readonly UserPermissionService userPermissionService;

  public AnnouncementAddEditDelete(UserPermissionService userPermissionService)
  {
    this.userPermissionService = userPermissionService;
  }

  protected override Task HandleRequirementAsync( AuthorizationHandlerContext context, AnnouncementAddEditDeleteRequirement requirement)
  {
    if (!userPermissionService.IsSetup || (context.User.HasClaim(w => w.Type == ClaimTypes.Email) && userPermissionService.IsSetup && string.IsNullOrEmpty(userPermissionService.UserClaimModel.Email)))
      userPermissionService.Setup(new UserClaimBuilder(context.User));
    var userClaims = new UserClaimBuilder(context.User);
    if (userClaims.UserPolicies.AnnouncementAddEditDelete)
    {
      context.Succeed(requirement);
    }

    return Task.CompletedTask;
  }
}
