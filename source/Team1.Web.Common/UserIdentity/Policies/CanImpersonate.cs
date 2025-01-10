using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Team1.Infrastructure.UserIdentity;

namespace Team1.Web.Common.UserIdentity.Policies;

public class CanImpersonateRequirement : IAuthorizationRequirement
{
}

public class CanImpersonate : AuthorizationHandler<CanImpersonateRequirement>
{
  private readonly UserPermissionService userPermissionService;

  public CanImpersonate(UserPermissionService userPermissionService)
  {
    this.userPermissionService = userPermissionService;
  }

  protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CanImpersonateRequirement requirement)
  {
    if (!userPermissionService.IsSetup || (context.User.HasClaim(w => w.Type == ClaimTypes.Email) && userPermissionService.IsSetup && string.IsNullOrEmpty(userPermissionService.UserClaimModel.Email)))
      userPermissionService.Setup(new UserClaimBuilder(context.User));

    if (userPermissionService.UserPolicies?.CanImpersonate ?? false)
    {
      context.Succeed(requirement);
    }

    return Task.CompletedTask;
  }
}