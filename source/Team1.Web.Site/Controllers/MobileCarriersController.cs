using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Team1.Infrastructure.Dtos;
using Team1.Infrastructure.Services.MobileCarriers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Team1.Web.Site.Controllers
{
    /// <summary>
    ///  Mobile Carriers controller
    /// </summary>
    [Authorize]
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class MobileCarriersController : BaseController
    {
        /// <summary>
        /// Gets countries
        /// </summary>
        /// <returns>List of countries</returns>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(List<SelectionOptionDto<int>>), (int)System.Net.HttpStatusCode.OK)]
        public async Task<IActionResult> GetMobileCarriers()
        {
            var service = GetService<MobileCarriersGet>();
            return Ok(await service.GetMobileCarriers());
        }
    }
}
