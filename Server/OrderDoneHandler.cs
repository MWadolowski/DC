using System;
using System.Collections.Generic;
using System.Net.Mail;
using Interpreter;
using Models;
using Newtonsoft.Json;
using PdfSharp.Drawing;
using PdfSharp.Pdf;

namespace Server
{
    public class OrderDoneHandler : IProcessHandler
    {
        public string StepName => StepNames.OrderSucces;
        public void Handle(ProcessMessage message, ulong tag)
        {
            var db = new HistoryBase();
            var data = JsonConvert.DeserializeObject<OrderData>(message.Attachments[Data.OrderDataFile] as string);
            db.Save(data);

            //Tworzenie pustego dokumentu
            PdfDocument document = new PdfDocument();
            //Opcje, używane w tworzeniu czcionek - inaczej nie działają polskie znaki
            XPdfFontOptions options = new XPdfFontOptions(PdfFontEncoding.Unicode, PdfFontEmbedding.Always);
            //tytuł #PODAJ JAKIŚ TYTUŁ
            document.Info.Title = "xxx";
            //Dodaj stronę
            PdfPage page = document.AddPage();
            //Stwórz pisaka, tym się wszystko rysuje
            XGraphics gfx = XGraphics.FromPdfPage(page, XGraphicsPdfPageOptions.Prepend);
            //Czcionki, mozesz sobie zmienic, jesli chcesz. Uwaga - na koncu musi byc options bo inaczej pierdoli polskie znaki
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            XFont fontLarge = new XFont("Georgia", 24, XFontStyle.Bold, options);
            XFont fontNormal = new XFont("Georgia", 19, XFontStyle.Regular, options);
            XFont fontSmall = new XFont("Georgia", 17, XFontStyle.Regular, options);
            // Mięsko, czyli tekst. Niestety automatycznie nie dzieli tego na linie, wiec musisz linia po linii robic i samemu je łamać. Chujowo, ale automatyczne łamanie wymagałoby ODZDZIELNEGO gówna, wiec pierdoliłem to
            //Główne powitanie, wyśrodkowane, duze
            gfx.DrawString("Szanowny Kliencie!", fontLarge, XBrushes.Black, new XRect(0, 32, page.Width, page.Height), XStringFormats.TopCenter);
            //tekts pdfa, mozesz sobie zmienic jak cos, co przerwe jest 24 pixele
            gfx.DrawString("Serdecznie dziękujemy za zakupy w naszym sklepie.", fontNormal, XBrushes.Black, new XRect(32, 80, page.Width, page.Height - 32), XStringFormats.TopLeft);
            gfx.DrawString("Jako firma z wieloletnią tradycją wierzymy, że bedziesz", fontNormal, XBrushes.Black, new XRect(32, 104, page.Width, page.Height - 32), XStringFormats.TopLeft);
            gfx.DrawString("zadowolony z jakości naszych kaczek.", fontNormal, XBrushes.Black, new XRect(32, 128, page.Width, page.Height - 32), XStringFormats.TopLeft);
            gfx.DrawString("Poniżej podana jest lista kaczek, które zakupiłeś:", fontNormal, XBrushes.Black, new XRect(32, 176, page.Width, page.Height - 32), XStringFormats.TopLeft);

            // przykładowe wypisanie produktów, liczbe n wykorzustuje do tego, ze potem dalszy tekts odpowiednio obnizam
            foreach (var product in data.Products)
            {
                gfx.DrawString($"{product.Product}: {product.Quantity}", fontSmall, XBrushes.Black, new XRect(48, 200 + data.Products.IndexOf(product) * 22, page.Width, page.Height - 32), XStringFormats.TopLeft);
            }

            var n = data.Products.Count + 1;
            //reszta tekstu, odpowiednio przesunieta po liscie produktow
            gfx.DrawString("Dziękujemy za zakupy i czekamy na kolejne zamówienie!", fontNormal, XBrushes.Black, new XRect(32, 200 + n * 22 + 24, page.Width, page.Height - 32), XStringFormats.TopLeft);
            gfx.DrawString("Rubber Duck Company", fontNormal, XBrushes.Black, new XRect(32, 200 + n * 22 + 48, page.Width, page.Height - 32), XStringFormats.TopLeft);


            //tu jest caly magiczny watermark, nie chce mi sie tlumaczyc co i jak, najwyzej zmien 100 na wieksza lub mniejsza oraz napis "InDucksWeTrust" na inny
            XFont watermark = new XFont("Arial", 100, XFontStyle.Bold);
            String watermarkText = "InDucksWeTrust";
            //nie tykac
            var size = gfx.MeasureString(watermarkText, watermark);
            gfx.TranslateTransform(page.Width / 2, page.Height / 2);
            gfx.RotateTransform(-Math.Atan(page.Height / page.Width) * 180 / Math.PI);
            gfx.TranslateTransform(-page.Width / 2, -page.Height / 2);
            var format = new XStringFormat();
            format.Alignment = XStringAlignment.Near;
            format.LineAlignment = XLineAlignment.Near;
            //tu jest kolor argb. takze tez mozesz sobie zmienic
            XBrush brush = new XSolidBrush(XColor.FromArgb(50, 63, 72, 204));
            gfx.DrawString(watermarkText, watermark, brush, new XPoint((page.Width - size.Width) / 2, (page.Height - size.Height) / 2), format);

            // tu masz nazwe pliku, DAJ JAKAS FAJNA ALBO PO NUMERZE CZY COS!!! 
            string filename = $"zamówienie{data.Number}.pdf";
            // tu jest zapis pliku
            document.Save(filename);
            var emailSener = new MailSender();

            emailSener.Send(data.Email, String.Empty, $"zamówienie numer {data.Number}",  new List<Attachment>
            {
                new Attachment(filename)
            });

            ShitHelper.Model.BasicAck(tag, false);
        }
    }
}
