using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Windows.Forms;
using JA.Gdi;

namespace JA.Printing
{
    /// <summary>
    /// When implemented, 2D objects can draw themselgs on a print output via the <see cref="PrintDoc"/> class.
    /// </summary>
    public interface IPrintable
    {
        void Render(Graphics g, PointF pos, SizeF bounds);
        SizeF GetSize(Graphics g, Rectangle page, PointF pos);
        HorizontalAlignment Alignment { get; }
    }


    public class PrintDoc : System.Drawing.Printing.PrintDocument
    {
        List<IPrintable> items;
        int index;
        bool split;
        float split_y;
        PointF pos;
        PrintDialog pd;
        PrintPreviewDialog pp;

        public PrintDoc()
        {
            this.BeginPrint+=new PrintEventHandler(PrintDoc_BeginPrint);
            this.PrintPage+=new System.Drawing.Printing.PrintPageEventHandler(PrintDoc_PrintPage);

            this.items=new List<IPrintable>();

            pd = new PrintDialog
            {
                Document = this,
                ShowNetwork = true,
                UseEXDialog = true,
                AllowCurrentPage = true,
                AllowPrintToFile = true,
                AllowSelection = true,
                AllowSomePages = true,
                ShowHelp = true
            };

            pp = new PrintPreviewDialog
            {
                Document = this
            };
            pp.Scale(new SizeF(1.4f, 1.4f));
            pp.Load+=new EventHandler(m_pp_Load);

        }

        void m_pp_Load(object sender, EventArgs e)
        {
            (sender as Form).WindowState=FormWindowState.Maximized;
        }

        void PrintDoc_BeginPrint(object sender, PrintEventArgs e)
        {
            index=0;
            split=false;
            split_y=0f;
            pos=PointF.Empty;
        }

        void PrintDoc_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            IPrintable item;
            pos=e.MarginBounds.Location;
            if(split)
            {
                pos.Y-=split_y;
            }
            if(items.Count==0)
            {
                e.HasMorePages=false;
                return;
            }
            do
            {
                PointF start=pos;
                item=items[index];
                SizeF sz=item.GetSize(e.Graphics, e.MarginBounds, pos);
                if(item.Alignment==HorizontalAlignment.Right)
                {
                    start.X=e.MarginBounds.Right-sz.Width;
                }
                else if(item.Alignment==HorizontalAlignment.Center)
                {
                    start.X=e.MarginBounds.Left+e.MarginBounds.Width/2-sz.Width/2;
                }
                if((pos.Y-e.MarginBounds.Top)/e.MarginBounds.Height<0.65||(pos.Y+sz.Height<=e.MarginBounds.Bottom))
                {
                    item.Render(e.Graphics, start, sz);
                    if(pos.Y+sz.Height<=e.MarginBounds.Bottom)
                    {
                        index++;
                        split=false;
                        split_y=0;
                    }
                    else
                    {
                        split=true;
                        split_y=sz.Height-(e.MarginBounds.Height-pos.Y);
                    }
                }
                pos.Y+=sz.Height;

            } while(index<items.Count&&pos.Y<e.MarginBounds.Bottom);

            e.HasMorePages=index<items.Count;
        }

        public void AddText(string text, Font font) { AddText(text, font, new StringFormat()); }
        public void AddText(string text, Font font, StringFormat sf)
        {
            items.Add(new TextLine(text, font, sf));
        }

        public void AddParagraph(string text, Font f, Color col, bool draw_border)
        {
            items.Add(new Paragraph(text, f, col, draw_border));
        }

        public void Add(IPrintable item)
        {
            items.Add(item);
        }

        public void AddLine()
        {
            items.Add(new EmptyLine());
        }
        public void AddHorizLine()
        {
            items.Add(new HorizontalLine());
        }

        public void AddPicture(Image image)
        {
            items.Add(new Picture(image));
        }

        //public void AddDrawing(Drawing drawing)
        //{
        //    m_items.Add(new DrawingPrintout(drawing));
        //}

        public bool PrintInColor
        {
            get { return this.DefaultPageSettings.Color; }
            set { this.DefaultPageSettings.Color=value; }
        }

        public void PrintDialog()
        {
            if(pd.ShowDialog()==DialogResult.OK)
            {
                Print();
            }
        }

        public void PrintPreviewDialog()
        {
            if(pd.ShowDialog()==DialogResult.OK)
            {
                pp.ShowDialog();
            }
        }

        public bool Landscape
        {
            get { return DefaultPageSettings.Landscape; }
            set { DefaultPageSettings.Landscape=value; }
        }


    }
}
