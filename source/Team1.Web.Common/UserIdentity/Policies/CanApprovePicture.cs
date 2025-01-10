using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Team1.Infrastructure.UserIdentity;

namespace Team1.Web.Common.UserIdentity.Policies;

public class CanApprovePictureRequirement : IAuthorizationRequirement
{
}

public class CanApprovePicture : AuthorizationHandler<CanApprovePictureRequirement>
{
  private readonly UserPermissionService userPermissionService;

  public CanApprovePicture(UserPermissionService userPermissionService)
  {
    this.userPermissionService = userPermissionService;
  }
  protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CanApprovePictureRequirement requirement)
  {
    if (!userPermissionService.IsSetup || (context.User.HasClaim(w => w.Type == ClaimTypes.Email) && userPermissionService.IsSetup && string.IsNullOrEmpty(userPermissionService.UserClaimModel.Email)))
      userPermissionService.Setup(new UserClaimBuilder(context.User));

    if (userPermissionService.UserPolicies?.CanApprovePicture ?? false)
    {
      context.Succeed(requirement);
    }

    return Task.CompletedTask;
  }
}
