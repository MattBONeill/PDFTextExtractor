using PDFExtractor.Primitives;
using PdfSharp.Pdf;
using PdfSharp.Pdf.Advanced;
using System;
using System.Collections.Generic;

namespace PDFExtractor
{
    public static class PDFTextExtractorExt
    {
        public static SizeD GetSize(this PdfPage page) => page.MediaBox != null ? new SizeD(page.MediaBox.Width, page.MediaBox.Height) : new SizeD(0, 0);

        public static TextPage GetParsedPage(this PdfPage page) => PDFTextExtractor.GetText(page);
    }
}
