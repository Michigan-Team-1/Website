using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Team1.Infrastructure.Dtos;
using Team1.Infrastructure.Services.Addresses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Team1.Web.Site.Controllers
{
    /// <summary>
    /// Addresses controller
    /// </summary>
    [Authorize]
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class AddressesController : BaseController
    {
        /// <summary>
        /// Get addresses for a user
        /// </summary>
        /// <param name="userId">user for whom to get addresses</param>
        /// <returns>List of Addresses</returns>
        [HttpGet("User/{userId}")]
        [ProducesResponseType(typeof(List<AddressDto>), (int)System.Net.HttpStatusCode.OK)]
        public async Task<IActionResult> GetUserAddresses(int userId)
        {
            var service = GetService<AddressesGet>();
            return Ok(await service.GetUserAddresses(userId));
        }
    }
}
