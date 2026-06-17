using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Team1.Infrastructure.Services;

public class MembershipPdfService
{
    static MembershipPdfService()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public byte[] GenerateApplication()
    {
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.Letter);
                page.Margin(1, Unit.Inch);
                page.DefaultTextStyle(x => x.FontSize(12));

                page.Content().Text("Test");
            });
        }).GeneratePdf();
    }
}
