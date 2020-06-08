using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Team1.Infrastructure.Services.Users;
using Team1.Infrastructure.Dtos;
using Team1.Infrastructure.Services;

namespace Team1.Web.Site.Controllers
{
    /// <summary>
    /// Roles controller
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
    public class RolesController : BaseController
    {
        /// <summary>
        /// Get roles
        /// </summary>
        /// <returns>List of roles</returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<RoleDto>), (int)System.Net.HttpStatusCode.OK)]
        public async Task<IActionResult> GetGrantableRoles()
        {
            var service = GetService<UsersGet>();
            return Ok(await service.GetGrantableRoles());
        }
    }
}
