using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Team1.Web.Site.Infrastructure.UserIdentity.Policies
{
    public class UserAddEditDelete : AuthorizationHandler<UserAddEditDelete>, IAuthorizationRequirement
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            UserAddEditDelete requirement)
        {
            var userClaims = new UserClaimBuilder(context.User);
            if (userClaims.UserPolicies.UserAddEditDelete)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
