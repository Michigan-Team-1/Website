using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Team1.Model.Attributes;
using System;
using System.Diagnostics;

namespace Team1.Web.Site.Controllers
{
    /// <summary>
    /// Home controller
    /// </summary>
    [TypeWriterIgnore]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class HomeController : BaseController
    {
        /// <summary>
        /// Change the language of the site
        /// </summary>
        /// <param name="culture">culture to set to</param>
        /// <param name="returnUrl">return url to go to after language changed</param>
        /// <returns>View</returns>
        [AllowAnonymous]
        [HttpPost]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            return LocalRedirect(returnUrl);
        }
    }
}
