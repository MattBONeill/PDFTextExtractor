using PDFExtractor.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDFExtractor.Tools
{
    public static class PDFTextTools
    {
        public static string GenerateText(this IEnumerable<ExtractedText> Source, double SpaceSize = 1)
        {
            if (Source == null || !Source.Any())
                return string.Empty;

            var Rows = SearchForRows(Source);

            StringBuilder ret = new StringBuilder();
            foreach (var grp in Rows)
            {
                grp.Text.Sort((l, r) => l.ScreenBounds.Left.CompareTo(r.ScreenBounds.Left));
                ScreenRectangle previousBounds = new ScreenRectangle(-1, -1, -1, -1);

                foreach (var item in grp.Text)
                {
                    if (previousBounds.Left >= 0)
                    {
                        var diff = Math.Abs(item.ScreenBounds.Left - previousBounds.Right);
                        if (diff > SpaceSize)
                        {
                            int TimesLarger = (int)(diff / SpaceSize);

                            var Tabs = TimesLarger / 4;
                            var Spaces = TimesLarger % 4;

                            ret.Append(new string(' ', Spaces));
                            if (Tabs > 0) 
                            { 
                                ret.Append(new string('\t', Tabs));
                            }
                        }
                    }
                    ret.Append(item.Text);
                    previousBounds = item.ScreenBounds;
                }
                ret.AppendLine(string.Empty);
            }

            return ret.ToString();
        }

        public static List<RowData> SearchForRows(IEnumerable<ExtractedText> Source)
        {
            List<RowData> Lines = new List<RowData>();

            if (Source == null || !Source.Any())
                return Lines;

            var SortedList = new List<ExtractedText>(Source);
            SortedList.Sort((l, r) => l.ScreenBounds.Top.CompareTo(r.ScreenBounds.Top));

            List<ExtractedText> buffer = new List<ExtractedText>();
            ScreenRectangle rectangle = new ScreenRectangle(SortedList.First().ScreenBounds);

            foreach (var area in SortedList.Skip(1))
            {
                if (string.IsNullOrWhiteSpace(area.Text))
                    continue;


                if (area.ScreenBounds.Bottom >= rectangle.Top &&
                    area.ScreenBounds.Top <= rectangle.Bottom)
                {
                    buffer.Add(area);

                    //grow area to include 
                    rectangle.Height = area.ScreenBounds.Bottom - rectangle.Top;
                }
                else
                {
                    Lines.Add(new RowData(buffer, rectangle));
                    buffer = new List<ExtractedText>() { area };
                    rectangle = new ScreenRectangle(area.ScreenBounds);
                }
            }
            Lines.Add(new RowData(buffer, rectangle));

            return Lines;
        }

        public static List<ColumnData> SearchForColumns(IEnumerable<ExtractedText> Source)
        {
            List<ColumnData> Lines = new List<ColumnData>();

            if (Source == null || !Source.Any())
                return Lines;

            var SortedList = new List<ExtractedText>(Source);
            SortedList.Sort((l, r) => l.ScreenBounds.Left.CompareTo(r.ScreenBounds.Left));


            List<ExtractedText> buffer = new List<ExtractedText>();
            ScreenRectangle rectangle = new ScreenRectangle(SortedList.First().ScreenBounds);

            foreach (var area in SortedList.Skip(1))
            {
                if (string.IsNullOrWhiteSpace(area.Text))
                    continue;

                if (area.ScreenBounds.Right >= rectangle.Left &&
                    area.ScreenBounds.Left <= rectangle.Right)
                {
                    buffer.Add(area);

                    //grow area to include 
                    rectangle.Width = area.ScreenBounds.Right - rectangle.Left;
                }
                else
                {
                    Lines.Add(new ColumnData(buffer, rectangle));
                    buffer = new List<ExtractedText>() { area };
                    rectangle = new ScreenRectangle(area.ScreenBounds);
                }
            }
            Lines.Add(new ColumnData(buffer, rectangle));

            return Lines;

        }


        public static IEnumerable<string[]> SearchForTable(IEnumerable<ExtractedText> Source)
        {
            if (Source == null || !Source.Any())
                yield break;

            var Rows = SearchForRows(Source);
            var Columns = SearchForColumns(Source);

            foreach (var row in Rows)
            { 
                var lineData = Columns.Select(column => row.Text.Where(txt => txt.ScreenBounds.InArea(column.Area)).GenerateText());
                yield return lineData.ToArray();
            }
        }
    }
    public struct RowData
    {
        public List<ExtractedText> Text { get; }
        public ScreenRectangle Area { get; internal set; }

        public RowData(List<ExtractedText> text, ScreenRectangle area)
        {
            Text = text;
            Area = area;
        }
    }

    public struct ColumnData
    {
        public List<ExtractedText> Text { get; }
        public ScreenRectangle Area { get; internal set; }

        public ColumnData(List<ExtractedText> text, ScreenRectangle area)
        {
            Text = text;
            Area = area;
        }
    }


}
