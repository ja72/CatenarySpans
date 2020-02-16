using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace JA.Gdi
{

    /// <summary>
    /// Class used to hold values for defining pens and brushes in GDI.
    /// </summary>
    public class Style : IDisposable
    {
        bool is_disposed;
        public Style()
        {
            this.is_disposed=false;
            this.FontStyle = FontStyle.Regular;
            this.TextHint = TextRenderingHint.SystemDefault;
            this.SmoothingMode = SmoothingMode.Default;
            this.InterpolationMode = InterpolationMode.Default;
            this.CompositingQuality = CompositingQuality.Default;
            this.CompositingMode = CompositingMode.SourceOver;
            this.FontSize = 9;
            this.StringFormat = new StringFormat();
            this.FontFamily = FontFamily.GenericSerif;
            this.Dash = DashStyle.Solid;
            this.EndCap = LineCap.NoAnchor;
            this.StartCap = LineCap.NoAnchor;
            this.PointWidth = 4f;
            this.PenWidth = 2f;
            this.Visible = true;
            this.TextColor = Color.Black;
            this.FillColor = Color.FromArgb(40, Color.Gray);
            this.PenColor = Color.Black;
     }

        public static readonly Style Default = new Style()
        {
            SmoothingMode = SmoothingMode.AntiAlias,
            CompositingQuality = CompositingQuality.HighQuality,
            StringFormat = new StringFormat() { Alignment = StringAlignment.Center },
        };

        public FontStyle FontStyle { get; set; } 
        public TextRenderingHint TextHint { get; set; }
        public SmoothingMode SmoothingMode { get; set; }
        public InterpolationMode InterpolationMode { get; set; }
        public CompositingQuality CompositingQuality { get; set; }
        public CompositingMode CompositingMode { get; set; } 
        public float FontSize { get; set; }
        public StringFormat StringFormat { get; set; }
        public FontFamily FontFamily { get; set; } 
        public DashStyle Dash { get; set; } 
        public LineCap EndCap { get; set; } 
        public LineCap StartCap { get; set; } 
        public float PointWidth { get; set; } 
        public float PenWidth { get; set; } 
        public bool Visible { get; set; } 
        public Color TextColor { get; set; }
        public Color FillColor { get; set; } 
        public Color PenColor { get; set; } 

        public static Style EndArrow(Color color)
        {
            return new Style()
            {
                PenColor=color,
                EndCap=LineCap.ArrowAnchor
            };
        }

        public static Style StandardFill(Color color)
        {
            return new Style()
            {
                PenColor=color,
                FillColor=Colors.Brighten(color, 0.3).LessOpaque(0.4)
            };
        }

        public Pen MakePen() { return MakePen(PenColor); }
        public Pen MakePen(Color color)
        {
            var pen = new Pen(color, PenWidth)
            {
                EndCap = EndCap,
                StartCap = StartCap,
                DashStyle = Dash
            };
            return pen;
        }
        public SolidBrush MakeSolidBrush()
        {
            return new SolidBrush(FillColor);
        }
        public SolidBrush MakeSolidBrush(Color color)
        {
            return new SolidBrush(color);
        }
        public SolidBrush MakeTextBrush()
        {
            return new SolidBrush(TextColor);
        }
        public LinearGradientBrush MakeGradientBrush(PointF point1, PointF point2)
        {
            Color second_color=Colors.Brighten(FillColor, 0.4).DeSaturate(0.2);
            return new LinearGradientBrush(point1, point2, FillColor, second_color);
        }
        public PathGradientBrush MakePathBrush(GraphicsPath path)
        {
            return new PathGradientBrush(path);
        }
        public Font MakeFont() { return MakeFont(FontSize); }
        public Font MakeFont(float size)
        {
            return new Font(FontFamily, size, FontStyle);
        }
        public Font MakeFont(Font prototype)
        {
            return new Font(prototype, FontStyle);
        }

        #region Graphics Quality

        public void SetGraphicsQuality(Graphics g)
        {
            g.CompositingMode=CompositingMode;
            g.CompositingQuality=CompositingQuality;
            g.InterpolationMode=InterpolationMode;
            g.SmoothingMode=SmoothingMode;
            g.TextRenderingHint=TextHint;
        }

        public void GetGraphicsQuality(Graphics g)
        {
            CompositingMode=g.CompositingMode;
            CompositingQuality=g.CompositingQuality;
            InterpolationMode=g.InterpolationMode;
            SmoothingMode=g.SmoothingMode;
            TextHint=g.TextRenderingHint;
        }
        #endregion

        #region Drawing Helpers
        /// <summary>
        /// Draws a horizontal arrow with leader text either mid length, or near the end point
        /// </summary>
        /// <param name="g">The graphics object</param>
        /// <param name="y">The y-coordinate of the arrow</param>
        /// <param name="x1">The start x-coordinate of the arrow</param>
        /// <param name="x2">The end x-coordinate of the arrow</param>
        /// <param name="text">The text to display or null</param>
        /// <param name="bidirectional">True to draw arrows on both ends</param>
        public void HorizontalArrow(Graphics g, float y, float x1, float x2, string text, bool bidirectional)
        {
            SizeF sz=new SizeF(8, 8);
            using(var font=MakeFont(SystemFonts.DialogFont))
            {
                if(!string.IsNullOrEmpty(text))
                {
                    sz=g.MeasureString(text, font);
                }
                using(Pen pen=new Pen(PenColor, 0))
                {
                    var arrow=new AdjustableArrowCap(0.2f*sz.Height, 0.4f*sz.Height);
                    pen.CustomEndCap=arrow;
                    if(bidirectional)
                    {
                        pen.CustomStartCap=arrow;
                    }
                    g.DrawLine(pen, x1, y, x2, y);

                }
                using(Brush brush=MakeSolidBrush(TextColor))
                {

                    if(!string.IsNullOrEmpty(text))
                    {
                        float xm=(x1+x2)/2;

                        if(Math.Abs(x2-x1)>=sz.Height)
                        {
                            //middle text
                            g.DrawString(
                                text,
                                font,
                                brush,
                                xm-sz.Width/2, y-sz.Height-2);
                        }
                        else
                        {
                            float text_offset=x1>x2?
                                -sz.Width-2:
                                2;

                            g.DrawString(
                                text,
                                font,
                                brush,
                                x2+text_offset,y-sz.Height/2);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Draws a vertical arrow with leader text either mid length, or near the end point
        /// </summary>
        /// <param name="g">The graphics object</param>
        /// <param name="x">The x-coordinate of the arrow</param>
        /// <param name="y1">The start y-coordinate of the arrow</param>
        /// <param name="y2">The end y-coordinate of the arrow</param>
        /// <param name="text">The text to display or null</param>
        /// <param name="bidirectional">True to draw arrows on both ends</param>
        public void VerticalArrow(Graphics g, float x, float y1, float y2, string text, bool bidirectional)
        {
            SizeF sz=new SizeF(8, 8);
            using(var font = MakeFont(SystemFonts.DialogFont))
            {
                if(!string.IsNullOrEmpty(text))
                {
                    sz=g.MeasureString(text, font);
                }
                using(Pen pen=new Pen(PenColor, 0))
                {
                    var arrow=new AdjustableArrowCap(0.2f*sz.Height, 0.4f*sz.Height);
                    pen.CustomEndCap=arrow;
                    if(bidirectional)
                    {
                        pen.CustomStartCap=arrow;
                    }
                    g.DrawLine(pen, x, y1, x, y2);

                }
                using(Brush brush = MakeSolidBrush(TextColor))
                {

                    if(!string.IsNullOrEmpty(text))
                    {
                        float ym=(y1+y2)/2;

                        if(Math.Abs(y2-y1)>=sz.Width)
                        {
                            //vertical text
                            var sf=new StringFormat(StringFormatFlags.DirectionVertical);
                            g.DrawString(
                                text,
                                font,
                                brush,
                                x, ym-sz.Width/2, sf);
                        }
                        else
                        {
                            float text_offset=y1>y2?
                                -sz.Height-2:
                                2;

                            g.DrawString(
                                text,
                                font,
                                brush,
                                x-sz.Width/2, y2+text_offset);
                        }
                    }
                }
            }
        }
        
        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!is_disposed)
            {
#pragma warning disable S1066 // Collapsible "if" statements should be merged
                if (disposing)
#pragma warning restore S1066 // Collapsible "if" statements should be merged
                {
                    // dispose managed resourced here
                    this.StringFormat.Dispose();
                }
            }
            is_disposed=true;
        }

        #endregion
    }

}
