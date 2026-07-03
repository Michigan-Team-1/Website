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

    public byte[] GenerateApplication(byte[]? logoBytes = null)
    {
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.Letter);
                page.Margin(0.6f, Unit.Inch);
                page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                page.Content().Element(c => ComposeContent(c, logoBytes));
            });
        }).GeneratePdf();

    }


    // MAIN CONTENT
    private void ComposeContent(IContainer container, byte[]? logoBytes)
    {
        container.Column(col =>
        {
            col.Item().Row(row =>
            {
                row.RelativeItem().Column(titleCol =>
                {
                    titleCol.Item().Text("MICHIGAN TEAM-1").Bold().FontSize(18);
                    titleCol.Item().Text("MEMBERSHIP APPLICATION").Bold().FontSize(18);
                });

                //LOGO 
                if (logoBytes != null && logoBytes.Length > 0)
                {
                    row.ConstantItem(110).Height(90)
                       .Image(logoBytes)
                       .FitArea();
                }
                else
                {
                    // Fallback if no logo
                    row.ConstantItem(110).Height(90)
                       .Border(1).BorderColor(Colors.Grey.Lighten1)
                       .AlignCenter().AlignMiddle()
                       .Text("LOGO").FontColor(Colors.Grey.Medium);
                }
            });

            col.Item().PaddingTop(12);


            //Mailing info 
            col.Item().Row(row =>
            {
                row.ConstantItem(110).Text("Mail this form to:");
                row.RelativeItem().Column(c =>
                {
                    c.Item().Text("Team-1 Membership").Bold();
                    c.Item().Text("c/o Robert Schultz").Bold();
                    c.Item().Text("5402 Red Fox Drive").Bold();
                    c.Item().Text("Brighton, MI 48114").Bold();
                });
            });

            col.Item().PaddingTop(10);

            //Dues line 
            col.Item().Text(t =>
            {
                t.Span("Dues are $30 per year, make checks payable to: ").Bold();
                t.Span("Michigan Team-1").Bold().FontSize(12);
            });

            col.Item().PaddingTop(8);

            //New / Renewal 
            col.Item().Row(row =>
            {
                row.ConstantItem(100);
                row.AutoItem().Text("New").Bold();
                row.ConstantItem(60).BorderBottom(1);
                row.ConstantItem(20);
                row.AutoItem().Text("Renewal").Bold();
                row.ConstantItem(60).BorderBottom(1);
                row.RelativeItem();
            });

            col.Item().PaddingTop(10);
            col.Item().Text("Please print:");

            col.Item().PaddingTop(6);

            //Name/TRA
            col.Item().Row(row =>
            {
                row.ConstantItem(55).Text("Name:").Bold();
                row.RelativeItem(3).BorderBottom(1);
                row.ConstantItem(10);
                row.ConstantItem(50).Text("TRA #").Bold();
                row.RelativeItem(1).BorderBottom(1);
            });
            col.Item().PaddingLeft(55).Text("(first – middle initial – last)").FontSize(7);

            col.Item().PaddingTop(8);

            //Address 
            col.Item().Row(row =>
            {
                row.ConstantItem(65).Text("Address:").Bold();
                row.RelativeItem().BorderBottom(1);
            });

            col.Item().PaddingTop(8);

            //City/State/Zip
            col.Item().Row(row =>
            {
                row.ConstantItem(40).Text("City:").Bold();
                row.RelativeItem(3).BorderBottom(1);
                row.ConstantItem(10);
                row.ConstantItem(40).Text("State:").Bold();
                row.RelativeItem(1).BorderBottom(1);
                row.ConstantItem(10);
                row.ConstantItem(30).Text("Zip:").Bold();
                row.RelativeItem(1).BorderBottom(1);
            });

            col.Item().PaddingTop(8);

            //Phone
            col.Item().Row(row =>
            {
                row.ConstantItem(45).Text("Phone:").Bold();
                row.ConstantItem(45).Text("(home)");
                row.RelativeItem(2).BorderBottom(1);
                row.ConstantItem(10);
                row.ConstantItem(30).Text("(cell)");
                row.RelativeItem(2).BorderBottom(1);
            });

            col.Item().PaddingTop(8);

            //DOB 
            col.Item().Row(row =>
            {
                row.ConstantItem(90).Text("Date of Birth:").Bold();
                row.RelativeItem(1).BorderBottom(1);
                row.RelativeItem(2);
            });
            col.Item().PaddingLeft(90).Text("(mm/dd/yyyy)").FontSize(7);

            col.Item().PaddingTop(8);

            //Email
            col.Item().Row(row =>
            {
                row.ConstantItem(95).Text("Email Address:").Bold();
                row.RelativeItem().BorderBottom(1);
            });

            col.Item().PaddingTop(10);

            //Certified/Authority/Level
            col.Item().Row(row =>
            {
                row.ConstantItem(65).Text("Certified:").Bold();
                row.AutoItem().Text("Yes").Bold();
                row.ConstantItem(8);
                row.AutoItem().Text("No").Bold();
                row.ConstantItem(8);
                row.AutoItem().Text("(circle one)").FontSize(7);
                row.ConstantItem(15);

                row.AutoItem().Text("Authority:").Bold();
                row.ConstantItem(8);
                row.AutoItem().Text("TRA").Bold();
                row.ConstantItem(8);
                row.AutoItem().Text("NAR").Bold();
                row.ConstantItem(8);
                row.AutoItem().Text("(circle one)").FontSize(7);
                row.ConstantItem(15);

                row.AutoItem().Text("Level:").Bold();
                row.ConstantItem(8);
                row.RelativeItem().BorderBottom(1);
            });

            col.Item().PaddingTop(14);

            //Paragraph 1
            col.Item().Text(
                "I am applying for TRIPOLI Prefecture 9, (Michigan Team-1) membership status. " +
                "I agree to all regulations, safety codes, rules and I am a member in good standing " +
                "with the TRIPOLI ROCKETRY ASSOCIATION, INC. It is further understood that by my " +
                "signature the purpose and objectives of our group is scientific and recreational."
            ).FontSize(9).LineHeight(1.3f);

            col.Item().PaddingTop(8);

            //Paragraph 2
            col.Item().Text(
                "I also agree to hold harmless TRIPOLI Prefecture 9, (Michigan TEAM-1) and TRIPOLI " +
                "ROCKETRY ASSOCIATION, INC. from any liability of group activities. This will remain " +
                "in effect until I submit a letter of resignation or my membership is allowed to lapse."
            ).FontSize(9).LineHeight(1.3f);

            col.Item().PaddingTop(14);

            //Signed/Date
            col.Item().Row(row =>
            {
                row.ConstantItem(50).Text("Signed:").Bold();
                row.RelativeItem(2).BorderBottom(1);
                row.ConstantItem(20);
                row.ConstantItem(35).Text("Date:").Bold();
                row.RelativeItem(1).BorderBottom(1);
            });
            col.Item().AlignRight().Width(150).Text("(mm/dd/yyyy)").FontSize(7);

            col.Item().PaddingTop(10);

            //Opt-out checkbox line
            col.Item().Row(row =>
            {
                row.AutoItem().Text(t =>
                {
                    t.DefaultTextStyle(x => x.FontSize(9));
                    t.Span("If you prefer that your contact information ");
                    t.Span("not").Underline();
                    t.Span(" be shown on the prefecture's web site, check here: ");
                });
                row.ConstantItem(50).AlignBottom().BorderBottom(1);
                row.RelativeItem();
            });

            col.Item().PaddingTop(14);

            //Minor/Adult-Parent section
            col.Item().Text("Adult/parent name and signature is required for applicants under eighteen years of age.")
               .FontSize(9);

            col.Item().PaddingTop(6);

            col.Item().Row(row =>
            {
                row.ConstantItem(130).Text("Adult/parent name (print)");
                row.RelativeItem().BorderBottom(1);
            });
            col.Item().PaddingLeft(130).Text("(first – middle – last)").FontSize(7);

            col.Item().PaddingTop(8);

            col.Item().Row(row =>
            {
                row.ConstantItem(130).Text("Adult/parent signature");
                row.RelativeItem(2).BorderBottom(1);
                row.ConstantItem(15);
                row.ConstantItem(35).Text("Date:");
                row.RelativeItem(1).BorderBottom(1);
            });
            col.Item().Row(row =>
            {
                row.ConstantItem(130).Text("(first – middle – last)").FontSize(7);
                row.RelativeItem(2);
                row.ConstantItem(15);
                row.ConstantItem(35);
                row.RelativeItem(1).AlignRight().Text("(mm/dd/yyyy)").FontSize(7);
            });
        });
    }
}