using Microsoft.AspNetCore.Mvc;
using Team1.Infrastructure.Services;

namespace Team1.Web.Controllers;

[Produces("application/json")]
[Route("api/Downloads")]
public class DownloadsController : BaseController
{
    private readonly MembershipPdfService _membershipPdfService;
    private readonly IWebHostEnvironment _env;
    private readonly IHttpClientFactory _httpClientFactory;

    public DownloadsController(
        MembershipPdfService membershipPdfService,
        IWebHostEnvironment env,
        IHttpClientFactory httpClientFactory)
    {
        _membershipPdfService = membershipPdfService;
        _env = env;
        _httpClientFactory = httpClientFactory;
    }

    [HttpGet("membership-application")]
    public async Task<IActionResult> MembershipApplication()
    {
        var client = _httpClientFactory.CreateClient();

        var logoBytes = await client.GetByteArrayAsync(
            $"{Request.Scheme}://{Request.Host}/logo.png");

        var pdfBytes = _membershipPdfService.GenerateApplication(logoBytes);

        return File(pdfBytes, "application/pdf",
            "Michigan-Team1-Membership-Application.pdf");
    }
}