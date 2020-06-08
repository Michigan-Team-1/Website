using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Team1.Web.Site.Infrastructure.UserIdentity.Policies
{
    public class AnnouncementAddEditDelete : AuthorizationHandler<AnnouncementAddEditDelete>, IAuthorizationRequirement
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            AnnouncementAddEditDelete requirement)
        {
            var userClaims = new UserClaimBuilder(context.User);
            if (userClaims.UserPolicies.AnnouncementAddEditDelete)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
