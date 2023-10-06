using PDFExtractor.Primitives;
using System.Collections.Generic;
using System.Linq;

namespace PDFExtractor
{
    /// <summary>
    /// Wraps a TextPage(List of string and Location) and a semi-accurate text output
    /// </summary>
    public class TextPage : List<ExtractedText>
    {
        public string PageText { get; }
        public SizeD Size { get; }

        public bool IsEmpty => Count == 0 && string.IsNullOrWhiteSpace(PageText);

        public TextPage(string pageText, IEnumerable<ExtractedText> textBlocks, SizeD size)
        {
            AddRange(textBlocks);
            PageText = pageText;
            Size = size;
        }
    }
}
