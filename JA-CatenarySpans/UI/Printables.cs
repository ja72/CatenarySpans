using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace JA.UI
{

    public class EmptyLine : IPrintable
    {
        #region IPrintable Members

        public void Render(System.Drawing.Graphics g, PointF pos, SizeF bounds)
        {
            //Do Nothing
        }

        public SizeF GetSize(System.Drawing.Graphics g, Rectangle page, PointF pos)
        {
            return new SizeF(12f, 12f);
        }

        public HorizontalAlignment Alignment
        {
            get { return HorizontalAlignment.Left; }
        }

        #endregion
    }

    public class HorizontalLine : IPrintable
    {
        #region IPrintable Members

        public void Render(Graphics g, PointF pos, SizeF bounds)
        {
            using(Pen pen=new Pen(Color.Black, 2f))
            {
                g.DrawLine(pen, pos.X, pos.Y, pos.X+bounds.Width, pos.Y);
            }
        }

        public SizeF GetSize(Graphics g, Rectangle page, PointF pos)
        {
            return new SizeF(page.Width, 12f);
        }

        public HorizontalAlignment Alignment
        {
            get { return HorizontalAlignment.Center; }
        }
        #endregion
    }

    public class TextLine : IPrintable
    {
        HorizontalAlignment align;

        public TextLine(string text) : this(text, SystemFonts.DialogFont, new StringFormat()) { }
        public TextLine(string text, Font font, StringFormat sf)
        {
            this.Text=text;
            this.Font=font;
            this.Format=sf;
            this.align=HorizontalAlignment.Left;
        }
        public string Text { get; set; }
        public Font Font { get; set; }
        public StringFormat Format { get; set; }

        #region IPrintable Members

        public void Render(Graphics g, PointF pos, SizeF bounds)
        {
            g.DrawString(Text, Font, Brushes.Black, pos, Format);
        }

        public SizeF GetSize(Graphics g, Rectangle page, PointF pos)
        {
            return g.MeasureString(Text, Font, pos, Format);
        }

        public HorizontalAlignment Alignment
        {
            get { return align; }
            set { align=value; }
        }


        #endregion
    }

    public class Paragraph : IPrintable, IDisposable
    {
        bool is_disposed;

        public Paragraph(string text, Font font, Color color)
            : this(text, font, color, false) { }

        public Paragraph(string text, Font font, Color color, bool draw_border)
        {
            this.Text=text;
            this.Font1=font;
            this.Color1=color;
            this.Format=new StringFormat(StringFormatFlags.NoWrap);     // default: StringFormatFlags.LineLimit
            this.ShowBorder=draw_border;
            this.Alignment=HorizontalAlignment.Left;
            this.is_disposed=false;
        }
        public string Text { get; set; }
        public Font Font
        {
            get { return Font1; }
            set
            {
                if(Font1!=value)
                {
                    Font1=value;
                }
            }
        }
        public Color Color { get { return Color1; } set { Color1=value; } }
        public StringFormat Format { get; set; }
        public bool ShowBorder { get; set; }

        #region IPrintable Members

        public void Render(System.Drawing.Graphics g, PointF pos, SizeF layout)
        {
            using(Brush b=new SolidBrush(Color1))
            {
                g.DrawString(Text, Font1, b, new RectangleF(pos, layout), Format);
                if(ShowBorder)
                {
                    g.DrawRectangle(Pens.Blue, pos.X, pos.Y, layout.Width, layout.Height);
                }
            }
        }

        public SizeF GetSize(System.Drawing.Graphics g, Rectangle page, PointF pos)
        {
            return g.MeasureString(Text, Font1, (int)(page.Right-pos.X), Format);
        }

        public HorizontalAlignment Alignment { get; set; }

        public Font Font1 { get; set; }
        public Color Color1 { get; set; }
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
                    this.Font1.Dispose();
                    this.Format.Dispose();
                }
            }
            is_disposed=true;
        }

        #endregion
    }

    public class Picture : IPrintable
    {
        public Picture(Image image)
        {
            this.Image=image;
            this.Alignment=HorizontalAlignment.Center;
            this.FitToPage=true;
            this.Caption=string.Empty;
        }

        #region IPrintable Members

        public void Render(Graphics g, PointF pos, SizeF bounds)
        {
            SizeF sz=SizeF.Empty;
            if(!string.IsNullOrEmpty(Caption))
            {
                sz=g.MeasureString(Caption, SystemFonts.CaptionFont);
            }
            RectangleF rect=new RectangleF(pos.X, pos.Y, bounds.Width, bounds.Height-sz.Height - 8);
            if(FitToPage)
            {
                g.DrawImage(Image, rect);
            }
            else
            {
                g.DrawImageUnscaledAndClipped(Image, Rectangle.Round(rect));
            }
            if(!string.IsNullOrEmpty(Caption))
            {
                var sf = new StringFormat(StringFormatFlags.FitBlackBox | StringFormatFlags.NoWrap)
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };
                RectangleF capt = new RectangleF(rect.Left, rect.Bottom+4, rect.Width, sz.Height );
                g.DrawString(Caption, SystemFonts.CaptionFont, Brushes.Black, capt, sf);
            }
        }

        public SizeF GetSize(Graphics g, Rectangle page, PointF pos)
        {            
            SizeF sz = SizeF.Empty;
            if(!string.IsNullOrEmpty(Caption))
            {
                sz=g.MeasureString(Caption, SystemFonts.CaptionFont);
            }
            if(FitToPage)
            {
                float aspect=Image.Height/(1f*Image.Width);
                float wt=page.Width+page.Left-pos.X;
                float ht=aspect*wt;
                return new SizeF(wt, ht+sz.Height + 8);
            }
            else
            {
                return new SizeF(Image.Width, Image.Height + 8);
            }
        }

        public HorizontalAlignment Alignment { get; set; }
        public bool FitToPage { get; set; }
        public string Caption { get; set; }

        public Image Image { get; }
        #endregion
    }

}
