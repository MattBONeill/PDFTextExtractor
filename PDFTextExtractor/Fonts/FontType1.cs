﻿using PdfSharp.Pdf;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections;
using PdfSharp.Drawing;
using System.Reflection.Emit;
using System.Text.Json;
using System.Text;
using System.Collections.Generic;
using PdfSharp.Pdf.Advanced;
using System;
using System.ComponentModel;

namespace PDFExtractor.Fonts
{
    internal class FontType1 : BaseFont
    {
        public string BaseFont { get; protected set; }
        public string SubsetFontName { get; }


        public FontType1(PdfDictionary dictionary) : base(dictionary)
        {
            if (dictionary.Elements.ContainsKey("/BaseFont"))
            {
                PdfItem item = dictionary.Elements["/BaseFont"];
                BaseFont = item.GetString().TrimStart('/');

                //remove Font Subset Name
                if (BaseFont.Contains('+'))
                {
                    var split = BaseFont.Split('+');
                    SubsetFontName = split.FirstOrDefault();
                    BaseFont = string.Join(string.Empty, split.Skip(1));
                }
                //for MM may want to be about to extract Spaced Values 
            }

            FillStandard14();
            UpdateUnicode(dictionary);
        }

        private void FillStandard14()
        {
            switch (BaseFont)
            {
                case "Times-Roman":
                    Base14Parser.FillFont(Standard14FontsSource.Times_Roman_Source, this);
                    break;
                case "Helvetica":
                    Base14Parser.FillFont(Standard14FontsSource.Helvetica_Source, this);
                    break;
                case "Courier":
                    Base14Parser.FillFont(Standard14FontsSource.Courier_Source, this);
                    break;
                case "Symbol":
                    Base14Parser.FillFont(Standard14FontsSource.Symbol_Source, this);
                    break;
                case "Times-Bold":
                    Base14Parser.FillFont(Standard14FontsSource.Times_Bold_Source, this);
                    break;
                case "Helvetica-Bold":
                    Base14Parser.FillFont(Standard14FontsSource.Helvetica_Bold_Source, this);
                    break;
                case "Courier-Bold":
                    Base14Parser.FillFont(Standard14FontsSource.Courier_Bold_Source, this);
                    break;
                case "ZapfDingbats":
                    Base14Parser.FillFont(Standard14FontsSource.ZapfDingbats_Source, this);
                    break;
                case "Times-Italic":
                    Base14Parser.FillFont(Standard14FontsSource.Times_Italic_Source, this);
                    break;
                case "Helvetica-Oblique":
                    Base14Parser.FillFont(Standard14FontsSource.Helvetica_Oblique_Source, this);
                    break;
                case "Courier-Oblique":
                    Base14Parser.FillFont(Standard14FontsSource.Courier_Oblique_Source, this);
                    break;
                case "Times-BoldItalic":
                    Base14Parser.FillFont(Standard14FontsSource.Times_BoldItalic_Source, this);
                    break;
                case "Helvetica-BoldOblique":
                    Base14Parser.FillFont(Standard14FontsSource.Helvetica_BoldOblique_Source, this);
                    break;
                case "Courier-BoldOblique":
                    Base14Parser.FillFont(Standard14FontsSource.Courier_BoldOblique_Source, this);
                    break;

            }


        }

    }


    internal class FontType0 : BaseFont
    {
        public string BaseFont { get; protected set; }
        public string SubsetFontName { get; }

        public CIDFont CIDSystemInfo { get; set; }


        public FontType0(PdfDictionary dictionary) : base(dictionary)
        {
            if (dictionary.Elements.ContainsKey("/BaseFont"))
            {
                PdfItem item = dictionary.Elements["/BaseFont"];
                BaseFont = item.GetString().TrimStart('/');

                //remove Font Subset Name
                if (BaseFont.Contains('+'))
                {
                    var split = BaseFont.Split('+');
                    SubsetFontName = split.FirstOrDefault();
                    BaseFont = string.Join(string.Empty, split.Skip(1));
                }
                //for MM may want to be about to extract Spaced Values 
            }
            UpdateUnicode(dictionary);
            //[/DescendantFonts, [ 166 0 R ]]
            if (dictionary.Elements.ContainsKey("/DescendantFonts"))
            {
                PdfItem item = dictionary.Elements["/DescendantFonts"];
                if (item is PdfReference) item = item.RemovePDfReference();
                if (item is PdfArray) item = (item as PdfArray).First();
                if (item is PdfReference) item = item.RemovePDfReference();
                if (item is PdfDictionary)
                {
                    //string map = .Stream.ToString();
                    //toUnicode = ParseCMap(map);
                    CIDSystemInfo = new CIDFont(item as PdfDictionary);
                    FontDescriptor = CIDSystemInfo.FontDescriptor;
                }
            }

        }



        public class CIDFont
        {
            public string Subtype{get;set;}
            public string CIDSystemInfo{get;set;}
            public string Type{get;set;}
            public string BaseFont{get;set;}
            public FontDescriptor FontDescriptor {get;set;}
            public double DW { get; set; } = 1000;
            public Dictionary<int, double> W { get; set; } = new Dictionary<int, double>();
            //public List<double> W { get; set; } = new List<double>();
            public CIDFont(PdfDictionary dictionary)
            {

                if (dictionary.Elements.ContainsKey("/FontDescriptor"))
                {
                    FontDescriptor = new FontDescriptor(dictionary.Elements["/FontDescriptor"].RemovePDfReference() as PdfDictionary);
                }

                if (dictionary.Elements.ContainsKey("/W"))
                {
                    W = ParseW(dictionary.Elements["/W"].RemovePDfReference() as PdfArray);
                    //var BaseArray = dictionary.Elements["/W"].RemovePDfReference() as PdfArray;
                    //if (BaseArray.First().ToString() == "0")
                    //{
                    //    var WidthArray = BaseArray.Skip(1).First().RemovePDfReference() as PdfArray;

                    //    foreach (var item in WidthArray)
                    //    {
                    //        W.Add(item.GetNumber());
                    //    }
                          
                    //}
                }

                if (dictionary.Elements.ContainsKey("/DW"))
                {
                    DW = dictionary.Elements["/DW"].GetNumber();
                }
            }

            Dictionary<int, double> ParseW(PdfArray Array)
            {
                var ret = new Dictionary<int, double>();
                int Index = -1;
                foreach(PdfItem item in Array)
                {
                    if (item is PdfArray)
                    {
                        if (Index == -1)
                            throw new Exception("Unable to Parse Font: /W Field of CIDFont is Formated Incorrectly");

                        foreach (var Width in item as PdfArray)
                        {
                            ret.Add(Index, Width.GetNumber());
                            Index++;
                        }
                    }
                    else if (item is PdfNumber || item is PdfInteger)
                    {
                        Index = item.GetInt();
                    }
                    else 
                        throw new Exception("Unable to Parse Font: /W Field of CIDFont has Unexpected Value");

                }




                return ret;
            }

        }

    }

    

}
