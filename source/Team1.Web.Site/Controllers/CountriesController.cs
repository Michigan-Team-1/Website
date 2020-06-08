using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Team1.Infrastructure.Dtos;
using Team1.Infrastructure.Services.Countries;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Team1.Web.Site.Controllers
{
    /// <summary>
    /// Countries controller
    /// </summary>
    [Authorize]
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class CountriesController : BaseController
    {
        /// <summary>
        /// Gets countries
        /// </summary>
        /// <returns>List of countries</returns>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(List<CountryDto>), (int)System.Net.HttpStatusCode.OK)]
        public async Task<IActionResult> GetCountries()
        {
            var service = GetService<CountriesGet>();
            return Ok(await service.GetCountries());
        }
    }
}
