using Microsoft.AspNetCore.Mvc;
using Team1.Infrastructure.Services;

namespace Team1.Web.Controllers;

[Produces("application/json")]
[Route("api/Downloads")]
public class DownloadsController : BaseController
{
    private readonly MembershipPdfService _membershipPdfService;

    public DownloadsController(MembershipPdfService membershipPdfService)
    {
        _membershipPdfService = membershipPdfService;
    }

    [HttpGet("membership-application")]
    public IActionResult MembershipApplication()
    {
        var pdfBytes = _membershipPdfService.GenerateApplication();
        return File(pdfBytes, "application/pdf", "Michigan-Team1-Membership-Application.pdf");
    }
}
