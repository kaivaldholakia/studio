using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Studio_Mobile.Classes
{
    public class PageNumberEventHandler : PdfPageEventHelper
    {
        protected int totalPageNumber;

        public override void OnStartPage(PdfWriter writer, Document document)
        {
            base.OnStartPage(writer, document);
            totalPageNumber = writer.PageNumber;
        }
        public override void OnEndPage(PdfWriter writer, Document document)
        {
            base.OnEndPage(writer, document);

            // Add page number
            int pageN = writer.PageNumber;
            string text = "Page " + pageN + " of " + totalPageNumber.ToString();
            float len = writer.GetVerticalPosition(true);
            float xPos = (document.Right + document.Left) / 2;
            float yPos = document.Bottom - 20;

            // Add page number to the document
            PdfContentByte cb = writer.DirectContent;
            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.WINANSI, BaseFont.NOT_EMBEDDED), 12);
            cb.ShowTextAligned(PdfContentByte.ALIGN_RIGHT, text, xPos, yPos, 0);
            cb.EndText();
        }
    }
}
