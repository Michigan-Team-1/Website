using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace Team1.Web.Common.UserIdentity.Policies
{
  public class PictureAddEditDelete : AuthorizationHandler<PictureAddEditDelete>, IAuthorizationRequirement
  {
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PictureAddEditDelete requirement)
    {
      var userClaims = new UserClaimBuilder(context.User);
      if (userClaims.UserPolicies.EventAddEditDelete)
      {
        context.Succeed(requirement);
      }

      return Task.CompletedTask;
    }
  }
}
