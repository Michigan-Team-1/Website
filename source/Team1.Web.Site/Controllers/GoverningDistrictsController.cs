using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Team1.Infrastructure.Dtos;
using Team1.Infrastructure.Services.GoverningDistricts;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Team1.Web.Site.Controllers
{
    /// <summary>
    /// Governing Districts controller
    /// </summary>
    [Authorize]
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class GoverningDistrictsController : BaseController
    {
        /// <summary>
        /// Gets governining Districts
        /// </summary>
        /// <returns>list of governing districts</returns>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(List<GoverningDistrictDto>), (int)System.Net.HttpStatusCode.OK)]
        public async Task<IActionResult> GetGoverningDistricts()
        {
            var service = GetService<GoverningDistrictsGet>();
            return Ok(await service.GetGoverningDistricts());
        }
    }
}
