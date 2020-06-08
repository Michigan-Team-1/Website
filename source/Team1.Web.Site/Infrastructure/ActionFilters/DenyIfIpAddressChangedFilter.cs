using Microsoft.AspNetCore.Mvc.Filters;
using Team1.Infrastructure.UserIdentity;
using System.Net;
using System.Threading.Tasks;

namespace Team1.Web.Site.Infrastructure.ActionFilters
{
    /// <summary>
    /// Use on any action you want to log the request information and response information
    /// </summary>
    public class DenyIfIpAddressChangedFilter : ActionFilterAttribute
    {
        private UserPermissionService _userPermissionService;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userPermissionService"></param>
        public DenyIfIpAddressChangedFilter(UserPermissionService userPermissionService)
        {
            _userPermissionService = userPermissionService;
        }

        /// <summary>
        /// deny request if ip address has changed
        /// </summary>
        /// <param name="context"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        public async override Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (_userPermissionService.IpAddressHasChanged)
            {
                context.Result = new Microsoft.AspNetCore.Mvc.ContentResult()
                {
                    StatusCode = (int)HttpStatusCode.Forbidden,
                    Content = "Access Denied:  Ip Address has changed."
                };
            }

            await base.OnActionExecutionAsync(context, next);
        }
    }
}
