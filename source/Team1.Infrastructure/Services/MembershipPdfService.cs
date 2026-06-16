using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace Team1.Infrastructure.Services;

public class MembershipPdfService : BaseService
{
    public byte[] GenerateApplication()
    {
        QuestPDF.Settings.License = LicenseType.Community;

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
