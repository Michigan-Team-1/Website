using Microsoft.AspNetCore.Mvc;
using Team1.Infrastructure.Services;
using Team1.Infrastructure.Services.Users;

namespace Team1.Web.Controllers;

[Produces("application/json")]
[Route("api/Downloads")]
public class DownloadsController : BaseController
{
    private readonly MembershipPdfService _membershipPdfService;
    private readonly IWebHostEnvironment _env;

    public DownloadsController(
        MembershipPdfService membershipPdfService,
        IWebHostEnvironment env)
    {
        _membershipPdfService = membershipPdfService;
        _env = env;
    }

    [HttpGet("membership-application")]
    public async Task<IActionResult> MembershipApplication()
    {
        var logoFile = _env.WebRootFileProvider.GetFileInfo("logo.png");

        if (!logoFile.Exists)
        {
            return NotFound("logo.png not found via WebRootFileProvider");
        }

        byte[] logoBytes;
        using (var stream = logoFile.CreateReadStream())
        using (var ms = new MemoryStream())
        {
            await stream.CopyToAsync(ms);
            logoBytes = ms.ToArray();
        }

        var usersGet = GetService<UsersGet>();
        var secretary = await usersGet.GetSecretary();

        var pdfBytes = _membershipPdfService.GenerateApplication(secretary, logoBytes);
        return File(pdfBytes, "application/pdf",
            "Michigan-Team1-Membership-Application.pdf");
    }
}