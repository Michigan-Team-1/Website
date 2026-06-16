
using Microsoft.AspNetCore.Mvc;
using Team1.Infrastructure.Services;

namespace Team1.Web.Controllers;

[Produces("application/json")]
[Route("api/Downloads")]
public class DownloadsController : BaseController
{
    [HttpGet("membership-application")]
    public IActionResult MembershipApplication()
    {
        var service = GetService<MembershipPdfService>();
        var pdfBytes = service.GenerateApplication();

        return File(pdfBytes, "application/pdf", "Michigan-Team1-Membership-Application.pdf");
    }
}
