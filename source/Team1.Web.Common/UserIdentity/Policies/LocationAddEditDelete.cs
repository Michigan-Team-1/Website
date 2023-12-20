using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Team1.Web.Common.UserIdentity.Policies
{
  public class LocationAddEditDelete : AuthorizationHandler<LocationAddEditDelete>, IAuthorizationRequirement
  {
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        LocationAddEditDelete requirement)
    {
      var userClaims = new UserClaimBuilder(context.User);
      if (userClaims.UserPolicies.LocationAddEditDelete)
      {
        context.Succeed(requirement);
      }

      return Task.CompletedTask;
    }
  }
}
