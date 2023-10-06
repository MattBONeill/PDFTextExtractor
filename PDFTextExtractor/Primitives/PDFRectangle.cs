namespace PDFExtractor.Primitives
{

    /// <summary>
    /// Coords are in PDF Page Space. (0,0) is Bottom left of the Page
    /// </summary>
    public struct PDFRectangle
    {
        public double X { get; set; }

        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }

        public double Right => X + Width;
        public double Left => X;
        public double Bottom => Y;
        public double Top => Y + Height;

        public PDFRectangle(double x, double y, double width, double height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public PDFRectangle(PointD loc, double width, double height) : this(loc.X, loc.Y, width, height) { }
        
        public PDFRectangle(PDFRectangle area) : this(area.X, area.Y, area.Width, area.Height) { }

        public PDFRectangle(PointD LowerLeft, PointD UpperRight) : this(LowerLeft.X, LowerLeft.Y, UpperRight.X - LowerLeft.X, UpperRight.Y - LowerLeft.Y) { }

        public override string ToString() => $"X:{X:0.00},Y:{Y:0.00},Width:{Width:0.00},Height:{Height:0.00}";

    }


    /// <summary>
    /// Coords in Screen Space. (0,0) is Top left of the Page
    /// </summary>
    public struct ScreenRectangle
    {
        public double X { get; set; }

        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }

        public double Right => X + Width;
        public double Left => X;
        public double Bottom => Y + Height;
        public double Top => Y;

        public ScreenRectangle(double x, double y, double width, double height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }


        public ScreenRectangle(ScreenRectangle area) : this(area.X, area.Y, area.Width, area.Height) { }

        public ScreenRectangle(PDFRectangle area, SizeD PageSize) : this(area.X, PageSize.Height - (area.Y + area.Height), area.Width, area.Height) { }


        public bool InArea(ScreenRectangle CheckArea)
        {
            return Right >= CheckArea.Left &&
                    Left <= CheckArea.Right &&
                    Bottom >= CheckArea.Top &&
                    Top <= CheckArea.Bottom;
        }

        public override string ToString() => $"X:{X:0.00},Y:{Y:0.00},Width:{Width:0.00},Height:{Height:0.00}";


    }

}
