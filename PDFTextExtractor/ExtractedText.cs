using PDFExtractor.Primitives;
using PdfSharp.Pdf;

namespace PDFExtractor
{
    public class ExtractedText
    {
        public PDFRectangle PdfBounds { get; }
        public ScreenRectangle ScreenBounds { get; }
        public string Text { get; }

        public ExtractedText(PDFRectangle pdfBounds, SizeD PageSize, string text)
        {
            PdfBounds = pdfBounds;
            ScreenBounds = new ScreenRectangle(pdfBounds, PageSize);
            Text = text;
        }
        public override string ToString() => $"Bounds:{PdfBounds};Text:{Text}";
    }
}
