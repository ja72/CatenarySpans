using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace JA.Gdi
{

    /// <summary>
    /// Class used to hold values for defining pens and brushes in GDI.
    /// </summary>
    public class Style : IDisposable
    {
        bool is_disposed;
        public Style()
        { is_disposed=false; }

        public Color pen_color=Color.Black;
        public Color fill_color=Color.FromArgb(40, Color.Gray);        
        public Color text_color=Color.Black;
        public bool visible=true;
        public float pen_width=2f;
        public float point_width=4f;
        public LineCap end_cap=LineCap.NoAnchor, start_cap=LineCap.NoAnchor;
        public DashStyle dash=DashStyle.Solid;
        public FontFamily font_family=FontFamily.GenericSerif;
        public StringFormat string_format=new StringFormat();
        public float font_size=9;
        public FontStyle font_style=FontStyle.Regular;
        public System.Drawing.Drawing2D.CompositingMode compositing_mode=System.Drawing.Drawing2D.CompositingMode.SourceOver;
        public System.Drawing.Drawing2D.CompositingQuality compositing_quality=System.Drawing.Drawing2D.CompositingQuality.Default;
        public System.Drawing.Drawing2D.InterpolationMode interpolation_mode=System.Drawing.Drawing2D.InterpolationMode.Default;
        public System.Drawing.Drawing2D.SmoothingMode smoothing_mode=System.Drawing.Drawing2D.SmoothingMode.Default;
        public System.Drawing.Text.TextRenderingHint text_hint=System.Drawing.Text.TextRenderingHint.SystemDefault;

        public static readonly Style Default=new Style();

        public static Style EndArrow(Color color)
        {
            return new Style()
            {
                pen_color=color,
                end_cap=LineCap.ArrowAnchor
            };
        }

        public static Style StandardFill(Color color)
        {
            return new Style()
            {
                pen_color=color,
                fill_color=Colors.Brighten(color, 0.3).LessOpaque(0.4)
            };
        }

        public Pen MakePen() { return MakePen(pen_color); }
        public Pen MakePen(Color color)
        {
            var pen = new Pen(color, pen_width)
            {
                EndCap = end_cap,
                StartCap = start_cap,
                DashStyle = dash
            };
            return pen;
        }
        public SolidBrush MakeSolidBrush()
        {
            return new SolidBrush(fill_color);
        }
        public SolidBrush MakeSolidBrush(Color color)
        {
            return new SolidBrush(color);
        }
        public SolidBrush MakeTextBrush()
        {
            return new SolidBrush(text_color);
        }
        public LinearGradientBrush MakeGradientBrush(PointF point1, PointF point2)
        {
            Color second_color=Colors.Brighten(fill_color, 0.4).DeSaturate(0.2);
            return new LinearGradientBrush(point1, point2, fill_color, second_color);
        }
        public PathGradientBrush MakePathBrush(GraphicsPath path)
        {
            return new PathGradientBrush(path);
        }
        public Font MakeFont() { return MakeFont(font_size); }
        public Font MakeFont(float size)
        {
            return new Font(font_family, size, font_style);
        }
        public Font MakeFont(Font prototype)
        {
            return new Font(prototype, font_style);
        }

        #region Graphics Quality

        public void SetGraphicsQuality(Graphics g)
        {
            g.CompositingMode=compositing_mode;
            g.CompositingQuality=compositing_quality;
            g.InterpolationMode=interpolation_mode;
            g.SmoothingMode=smoothing_mode;
            g.TextRenderingHint=text_hint;
        }

        public void GetGraphicsQuality(Graphics g)
        {
            compositing_mode=g.CompositingMode;
            compositing_quality=g.CompositingQuality;
            interpolation_mode=g.InterpolationMode;
            smoothing_mode=g.SmoothingMode;
            text_hint=g.TextRenderingHint;
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
        public void HorizontalArrow(System.Drawing.Graphics g, float y, float x1, float x2, string text, bool bidirectional)
        {
            SizeF sz=new SizeF(8, 8);
            using(var font=MakeFont(SystemFonts.DialogFont))
            {
                if(!string.IsNullOrEmpty(text))
                {
                    sz=g.MeasureString(text, font);
                }
                using(Pen pen=new Pen(pen_color, 0))
                {
                    var arrow=new System.Drawing.Drawing2D.AdjustableArrowCap(0.2f*sz.Height, 0.4f*sz.Height);
                    pen.CustomEndCap=arrow;
                    if(bidirectional)
                    {
                        pen.CustomStartCap=arrow;
                    }
                    g.DrawLine(pen, x1, y, x2, y);

                }
                using(Brush brush=MakeSolidBrush(text_color))
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
        public void VerticalArrow(System.Drawing.Graphics g, float x, float y1, float y2, string text, bool bidirectional)
        {
            SizeF sz=new SizeF(8, 8);
            using(var font = MakeFont(SystemFonts.DialogFont))
            {
                if(!string.IsNullOrEmpty(text))
                {
                    sz=g.MeasureString(text, font);
                }
                using(Pen pen=new Pen(pen_color, 0))
                {
                    var arrow=new System.Drawing.Drawing2D.AdjustableArrowCap(0.2f*sz.Height, 0.4f*sz.Height);
                    pen.CustomEndCap=arrow;
                    if(bidirectional)
                    {
                        pen.CustomStartCap=arrow;
                    }
                    g.DrawLine(pen, x, y1, x, y2);

                }
                using(Brush brush = MakeSolidBrush(text_color))
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
                if (disposing)
                {
                    // dispose managed resourced here
                    this.string_format.Dispose();
                }
            }
            is_disposed=true;
        }

        #endregion
    }

}
