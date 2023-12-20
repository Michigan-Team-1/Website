using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace Team1.Web.Common.UserIdentity.Policies
{
  public class CanApprovePicture : AuthorizationHandler<CanApprovePicture>, IAuthorizationRequirement
  {
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        CanApprovePicture requirement)
    {
      var userClaims = new UserClaimBuilder(context.User);
      if (userClaims.UserPolicies.CanApprovePicture)
      {
        context.Succeed(requirement);
      }

      return Task.CompletedTask;
    }
  }
}
