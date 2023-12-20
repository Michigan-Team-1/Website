using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Team1.Web.Common.UserIdentity.Policies
{
  public class CanImpersonate : AuthorizationHandler<CanImpersonate>, IAuthorizationRequirement
  {
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        CanImpersonate requirement)
    {
      var userClaims = new UserClaimBuilder(context.User);
      if (userClaims.UserPolicies.CanImpersonate)
      {
        context.Succeed(requirement);
      }

      return Task.CompletedTask;
    }
  }
}
