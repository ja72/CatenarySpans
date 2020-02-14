using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using JA.Gdi;
using JA.Engineering;
using System.Windows.Forms;

namespace JA.CatenarySpans
{
    public class CatenaryTable : JA.Printing.IPrintable
    {
        Style style=Style.Default;
        readonly RulingSpan rs;
        readonly Vector2 clearance_pt;
        HorizontalAlignment align;

        public CatenaryTable(RulingSpan rs, Vector2 clearance_pt)
        {
            this.rs=rs;
            this.style.string_format.Alignment=StringAlignment.Center;
            this.style.smoothing_mode=System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            this.clearance_pt=clearance_pt;
            this.align=HorizontalAlignment.Center;
        }
        public Vector2 ClearancePoint { get { return clearance_pt; } }
        public RulingSpan RulingSpan { get { return rs; } }
        public string TableFooter()
        {
            string lu=rs.Units.Length;
            string fu=rs.Units.Force;
            string footer=string.Format("Total Length = {0:0.##} {1}, Horizontal Tension = {2:0.###} {3}",
                rs.TotalLength, lu, rs.HorizontalTension, fu);
            if(!clearance_pt.IsZero)
            {

                double dy=rs.ClearanceTo(clearance_pt, false);
                return footer+string.Format(", Clearance to ({0:0.##},{1:0.##}) is {2:0.###} {3}", clearance_pt.x, clearance_pt.y, dy, lu);
            }
            return footer;
        }
        public string[] TableHeaders()
        {
            string lu=rs.Units.Length;
            string fu=rs.Units.Force;
            return new string[] {
                string.Format("Start X ({0})",lu),
                string.Format("Start Y ({0})",lu),
                string.Format("Span X ({0})",lu),
                string.Format("Span Y ({0})",lu),
                string.Format("Weight ({0}/{1})",fu,lu),
                string.Format("Ave. Tension ({0})",fu),
                string.Format("Total Length ({0})",lu),
                string.Format("Max. Sag ({0})",lu), 
                string.Format("Clearance ({0})",lu),
                string.Format("Cat. Constant ({0})",lu),
                "Geom. Strain (%)",
                "Uplift (y/n)"
            };
        }
        public int[] ColumnWidths()
        {
            return new int[] {
                4,
                4,
                4,
                4,
                4,
                5,
                5,
                4,
                5,
                5,
                4,
                4
            };
        }

        public string[] TableRow(Catenary item)
        {
            return new string[] {
                item.StartX.ToString("0.#"),
                item.StartY.ToString("0.#"),
                item.SpanX.ToString("0.#"),
                item.SpanY.ToString("0.#"),
                item.Weight.ToString("0.###"),
                item.AverageTension.ToString("0.#"),
                item.TotalLength.ToString("0.#"),
                item.MaximumSag.ToString("0.##"),
                item.Clearance.ToString("0.#"),
                item.CatenaryConstant.ToString("0.#"),
                (item.GeometricStrainPct/100).ToString("0.###%"),
                item.IsUpliftCondition?"y":"n"
            };
        }

        #region IPrintable Members

        public void Render(System.Drawing.Graphics g, System.Drawing.PointF pos, System.Drawing.SizeF bounds)
        {
            style.SetGraphicsQuality(g);
            SizeF sz1=GetHeaderSize(g);
            SizeF sz2=GetTableSize(g);
            SizeF sz3=GetFooterSize(g);
            SizeF em = SizeF.Empty;
            string[] headers=TableHeaders();
            int[] widths=ColumnWidths();
            using(var brush=style.MakeSolidBrush(style.text_color))
            {
                float x=pos.X, y=pos.Y, ht=0;
                style.font_family=System.Drawing.FontFamily.GenericSerif;
                style.string_format.Alignment=StringAlignment.Center;
                style.string_format.LineAlignment=StringAlignment.Center;
                using(var font=style.MakeFont())
                {
                    em=g.MeasureString("M", font);
                    for(int j=0; j<headers.Length; j++)
                    {
                        SizeF sz=g.MeasureString(headers[j], font, (int)(widths[j]*em.Width), style.string_format);
                        if(ht<sz.Height) { ht=sz.Height; }
                        RectangleF rect=new RectangleF(x, y, widths[j]*em.Width, sz1.Height-1);
                        g.DrawString(headers[j], font, brush, rect, style.string_format);
                        g.DrawLine(Pens.Gray, x, pos.Y, x, pos.Y+sz1.Height+4+sz2.Height-1);
                        x+=widths[j]*em.Width+4;
                    }
                }
                style.font_family=System.Drawing.FontFamily.GenericSansSerif;
                style.string_format.Alignment=StringAlignment.Center;
                y=pos.Y+sz1.Height+4;
                x=pos.X;
                g.DrawLine(Pens.Gray, x, y-5, x+sz1.Width, y-5);
                using(var font=style.MakeFont())
                {
                    //SizeF em=g.MeasureString("M", font);
                    foreach(var item in rs.Spans)
                    {
                        string[] row=TableRow(item);
                        x=pos.X;
                        for(int j=0; j<row.Length; j++)
                        {
                            RectangleF rect=new RectangleF(x, y, widths[j]*em.Width, em.Height);
                            g.DrawString(row[j], font, brush, rect, style.string_format);
                            x+=widths[j]*em.Width+4;
                        }
                        y+=em.Width+4;
                    }

                }
                style.font_family=System.Drawing.FontFamily.GenericSerif;
                style.string_format.Alignment=StringAlignment.Center;
                style.string_format.LineAlignment=StringAlignment.Near;

                y=pos.Y+sz1.Height+4+sz2.Height+4;
                x=pos.X;
                g.DrawLine(Pens.Gray, x, y-5, x+sz1.Width, y-5);
                using(var font=style.MakeFont())
                {
                    //SizeF em=g.MeasureString("M", font);
                    int wt=ColumnWidths().Sum();
                    {
                        RectangleF rect=new RectangleF(x, y, wt*em.Width, em.Height);
                        g.DrawString(TableFooter(), font, brush, rect, style.string_format);
                        x+=wt*em.Width+4;
                        y+=em.Width+4;
                    }
                }
            }
            using(var pen=style.MakePen())
            {
                g.DrawRectangle(pen, pos.X, pos.Y, bounds.Width, bounds.Height);
            }
        }

        public System.Drawing.SizeF GetSize(System.Drawing.Graphics g, System.Drawing.Rectangle page, System.Drawing.PointF pos)
        {
            SizeF sz1=GetHeaderSize(g);
            SizeF sz2=GetTableSize(g);
            SizeF sz3=GetFooterSize(g);
            return new SizeF(sz1.Width, sz1.Height+4+sz2.Height+4+sz3.Height+4);
            //return new SizeF(sz1.Width, sz1.Height+sz2.Height);
        }
        SizeF GetFooterSize(Graphics g)
        {
            string footer=TableFooter();
            style.SetGraphicsQuality(g);
            style.font_family=System.Drawing.FontFamily.GenericSerif;
            style.string_format.Alignment=StringAlignment.Near;
            style.string_format.LineAlignment=StringAlignment.Near;
            float ht=0;
            float x=0;
            using(var font=style.MakeFont())
            {
                SizeF em=g.MeasureString("M", font);
                int wt=ColumnWidths().Sum();
                SizeF sz=g.MeasureString(footer, font, (int)(wt*em.Width), style.string_format);
                if(ht<sz.Height) { ht=sz.Height; }
                x+=wt*em.Width+4;
            }
            return new SizeF(x, ht+4);
        }
        SizeF GetHeaderSize(Graphics g)
        {
            float ht=0;
            string[] headers=TableHeaders();
            int[] widths=ColumnWidths();
            float x=0;
            style.SetGraphicsQuality(g);
            style.font_family=System.Drawing.FontFamily.GenericSerif;
            style.string_format.Alignment=StringAlignment.Center;
            style.string_format.LineAlignment=StringAlignment.Center;
            using(var font=style.MakeFont())
            {
                SizeF em=g.MeasureString("M", font);

                for(int j=0; j<headers.Length; j++)
                {
                    SizeF sz=g.MeasureString(headers[j], font, (int)(widths[j]*em.Width), style.string_format);
                    if(ht<sz.Height) { ht=sz.Height; }
                    x+=widths[j]*em.Width+4;
                }
            }
            return new SizeF(x, ht+4);
        }

        SizeF GetTableSize(Graphics g)
        {
            float wt=0, ht=0;
            int[] widths=ColumnWidths();
            style.font_family=System.Drawing.FontFamily.GenericSansSerif;
            style.string_format.Alignment=StringAlignment.Center;
            int N=rs.Spans.Count();
            using(var font=style.MakeFont())
            {
                SizeF em=g.MeasureString("M", font);
                for(int j=0; j<widths.Length; j++)
                {
                    wt+=widths[j]*em.Width+4;
                }
                ht=N*(em.Height+4);
            }
            return new SizeF(wt, ht+4);
        }

        public HorizontalAlignment Alignment
        {
            get { return align; }
            set { align=value; }
        }
        #endregion
    }
}
