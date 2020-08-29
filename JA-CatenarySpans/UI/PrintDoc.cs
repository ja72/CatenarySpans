using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Windows.Forms;

namespace JA.UI
{
    using JA.Engineering;

    /// <summary>
    /// When implemented, 2D objects can draw themselves on a print output via the <see cref="PrintDoc"/> class.
    /// </summary>
    public interface IPrintable
    {
        void Render(Graphics g, PointF pos, SizeF bounds);
        SizeF GetSize(Graphics g, Rectangle page, PointF pos);
        HorizontalAlignment Alignment { get; }
    }


    public class PrintDoc : System.Drawing.Printing.PrintDocument
    {
        int index;
        bool split;
        float split_y;
        PointF pos;

        public PrintDoc()
        {
            this.BeginPrint+=new PrintEventHandler(PrintDoc_BeginPrint);
            this.PrintPage+=new System.Drawing.Printing.PrintPageEventHandler(PrintDoc_PrintPage);

            this.Items=new List<IPrintable>();

            PrintDialogBox = new PrintDialog
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

            PrintPreviewDialogBox = new PrintPreviewDialog
            {
                Document = this
            };
            PrintPreviewDialogBox.Scale(new SizeF(1.4f, 1.4f));
            PrintPreviewDialogBox.Load+=new EventHandler(m_pp_Load);

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
            if(Items.Count==0)
            {
                e.HasMorePages=false;
                return;
            }
            do
            {
                PointF start=pos;
                item=Items[index];
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

            } while(index<Items.Count&&pos.Y<e.MarginBounds.Bottom);

            e.HasMorePages=index<Items.Count;
        }

        public void AddText(string text, Font font) { AddText(text, font, new StringFormat()); }
        public void AddText(string text, Font font, StringFormat sf)
        {
            Items.Add(new TextLine(text, font, sf));
        }

        public void AddParagraph(string text, Font f, Color col, bool draw_border)
        {
            Items.Add(new Paragraph(text, f, col, draw_border));
        }

        public void Add(IPrintable item)
        {
            Items.Add(item);
        }

        public void AddLine()
        {
            Items.Add(new EmptyLine());
        }
        public void AddHorizLine()
        {
            Items.Add(new HorizontalLine());
        }

        public void AddPicture(Image image)
        {
            Items.Add(new Picture(image));
        }

        public bool PrintInColor
        {
            get { return this.DefaultPageSettings.Color; }
            set { this.DefaultPageSettings.Color=value; }
        }

        public void PrintDialog()
        {
            if(PrintDialogBox.ShowDialog()==DialogResult.OK)
            {
                Print();
            }
        }

        public void PrintPreviewDialog()
        {
            if(PrintDialogBox.ShowDialog()==DialogResult.OK)
            {
                PrintPreviewDialogBox.ShowDialog();
            }
        }

        public bool Landscape
        {
            get { return DefaultPageSettings.Landscape; }
            set { DefaultPageSettings.Landscape=value; }
        }

        public List<IPrintable> Items { get; }

        public PrintDialog PrintDialogBox { get; }

        public PrintPreviewDialog PrintPreviewDialogBox { get; }
    }
}
