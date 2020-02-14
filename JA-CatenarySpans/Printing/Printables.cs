using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace JA.Printing
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
        string text;
        Font font;
        StringFormat sf;
        HorizontalAlignment align;

        public TextLine(string text) : this(text, SystemFonts.DialogFont, new StringFormat()) { }
        public TextLine(string text, Font font, StringFormat sf)
        {
            this.text=text;
            this.font=font;
            this.sf=sf;
            this.align=HorizontalAlignment.Left;
        }
        public string Text { get { return text; } set { text=value; } }
        public Font Font { get { return font; } set { font=value; } }
        public StringFormat Format { get { return sf; } set { sf=value; } }

        #region IPrintable Members

        public void Render(Graphics g, PointF pos, SizeF bounds)
        {
            g.DrawString(text, font, Brushes.Black, pos, sf);
        }

        public SizeF GetSize(Graphics g, Rectangle page, PointF pos)
        {
            return g.MeasureString(text, font, pos, sf);
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
        string text;
        Font font;
        Color color;
        StringFormat sf;
        bool border;
        HorizontalAlignment align;

        public Paragraph(string text, Font font, Color color)
            : this(text, font, color, false) { }

        public Paragraph(string text, Font font, Color color, bool draw_border)
        {
            this.text=text;
            this.font=font;
            this.color=color;
            this.sf=new StringFormat(StringFormatFlags.NoWrap);     // default: StringFormatFlags.LineLimit
            this.border=draw_border;
            this.align=HorizontalAlignment.Left;
            this.is_disposed=false;
        }
        public string Text { get { return text; } set { text=value; } }
        public Font Font
        {
            get { return font; }
            set
            {
                if(font!=value)
                {
                    font=value;
                }
            }
        }
        public Color Color { get { return color; } set { color=value; } }
        public StringFormat Format { get { return sf; } set { sf=value; } }
        public bool ShowBorder { get { return border; } set { border=value; } }

        #region IPrintable Members

        public void Render(System.Drawing.Graphics g, PointF pos, SizeF layout)
        {
            using(Brush b=new SolidBrush(color))
            {
                g.DrawString(text, font, b, new RectangleF(pos, layout), sf);
                if(border)
                {
                    g.DrawRectangle(Pens.Blue, pos.X, pos.Y, layout.Width, layout.Height);
                }
            }
        }

        public SizeF GetSize(System.Drawing.Graphics g, Rectangle page, PointF pos)
        {
            return g.MeasureString(text, font, (int)(page.Right-pos.X), sf);
        }

        public HorizontalAlignment Alignment
        {
            get { return align; }
            set { align=value; }
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
                    this.font.Dispose();
                    this.sf.Dispose();
                }
            }
            is_disposed=true;
        }

        #endregion
    }

    public class Picture : IPrintable
    {
        Image image;

        public Picture(Image image)
        {
            this.image=image;
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
                g.DrawImage(image, rect);
            }
            else
            {
                g.DrawImageUnscaledAndClipped(image, Rectangle.Round(rect));
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
                float aspect=image.Height/(1f*image.Width);
                float wt=page.Width+page.Left-pos.X;
                float ht=aspect*wt;
                return new SizeF(wt, ht+sz.Height + 8);
            }
            else
            {
                return new SizeF(image.Width, image.Height + 8);
            }
        }

        public HorizontalAlignment Alignment { get; set; }
        public bool FitToPage { get; set; }
        public string Caption { get; set; }
        #endregion
    }

}
