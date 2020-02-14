using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using JA.Gdi;
using JA.Engineering;

namespace JA.CatenarySpans
{

    #region Graphics
    public class CatenaryGraphics : JA.Printing.IPrintable
    {
        Style style=Style.Default;
        readonly RulingSpan rs;
        Vector2 mouse_over, mouse_dn;
        HorizontalAlignment align;

        public CatenaryGraphics(RulingSpan rs)
        {
            this.rs=rs;
            this.style.string_format.Alignment=StringAlignment.Center;
            this.style.smoothing_mode=System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            this.style.compositing_quality=System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            this.mouse_over=Vector2.Origin;
            this.mouse_dn=Vector2.Origin;
            this.align=HorizontalAlignment.Left;
        }
        public Style Style { get { return style; } set { style=value; } }
        public RulingSpan RulingSpan { get { return rs; } }
        public void GetBounds(out Vector2 min_position, out Vector2 max_position)
        {
            min_position=Vector2.Origin;
            max_position=Vector2.Origin;
            foreach (var item in rs.Spans)
            {
                item.GetBounds(ref min_position, ref max_position);
            }
        }
        public Canvas GetCanvasFor(RectangleF target)
        {
            //Vector2 minPosition, maxPosition;
            GetBounds(out Vector2 minPosition, out Vector2 maxPosition);

            return new Canvas(target, minPosition, maxPosition);
        }

        public RectangleF GetDrawArea(Control control)
        {
            return new RectangleF(
                control.ClientRectangle.Left+control.Margin.Left,
                control.ClientRectangle.Top+control.Margin.Top,
                control.ClientRectangle.Width-control.Margin.Left-control.Margin.Right,
                control.ClientRectangle.Height-control.Margin.Top-control.Margin.Bottom);
        }

        public Vector2 MouseDownPosition { get { return mouse_dn; } }
        public Vector2 MousePosition { get { return mouse_over; } }

        public void Render(Graphics g, RectangleF target)
        {
            Canvas canvas=GetCanvasFor(target);
            style.SetGraphicsQuality(g);
            Catenary catenary=RulingSpan.FindCatenaryFromX(mouse_over.x);
            foreach (var item in rs.Spans)
            {
                RenderOne(g, canvas, item, style, catenary == item);
            }
            //if (button==MouseButtons.Right&&!mouse_dn.IsZero)
            if (!mouse_dn.IsZero)
            {
                catenary=RulingSpan.FindCatenaryFromX(mouse_dn.x);
                if (catenary!=null)
                {
                    double y=catenary.CatenaryFunction(mouse_dn.x);
                    PointF A=canvas.Map(mouse_dn.x, y);
                    PointF M=canvas.Map(mouse_dn);
                    string text=Math.Abs(y-mouse_dn.y).ToString("0.#");
                    style.VerticalArrow(g, M.X, M.Y, A.Y, text, false);
                }
                using (Pen pen=style.MakePen(Color.Red))
                {
                    PointF M=canvas.Map(mouse_dn);
                    g.DrawEllipse(pen,
                        M.X-style.point_width/2,
                        M.Y-style.point_width/2,
                        style.point_width,
                        style.point_width);

                    using (Font font=style.MakeFont(SystemFonts.DialogFont))
                    {
                        string text=string.Format("x:{0:0}\ny:{1:0.#}", mouse_dn.x, mouse_dn.y);
                        StringFormat sf=new StringFormat();
                        //sf.Alignment = StringAlignment.Center;
                        SizeF sz=g.MeasureString(text, font, M, sf);

                        g.FillRectangle(Brushes.White, M.X-sz.Width-4, M.Y-4, sz.Width+2, sz.Height+2);
                        g.DrawString(text, font, Brushes.Red, M.X-sz.Width-2, M.Y-2, sf);
                    }
                }
            }

        }

        #region Rendering
        public static void RenderOne(Graphics g, Canvas canvas, Catenary catenary, Style style, bool selected=false)
        {
            PointF A=canvas.Map(catenary.StartPosition);
            PointF B=canvas.Map(catenary.EndPosition);
            PointF A0=canvas.Map(catenary.StartBase);
            PointF B0=canvas.Map(catenary.EndBase);
            PointF C=canvas.Map(catenary.LowestPosition);
            PointF C0=canvas.Map(new Vector2(catenary.CenterX, 0));
            SizeF em;
            // Ground
            using (Font font=style.MakeFont(SystemFonts.DialogFont))
            {
                g.DrawLine(Pens.Black, A0, B0);
                var txt=string.Format("{0:0.#}", catenary.SpanX);
                em=g.MeasureString("M", font);
                float y=canvas.Target.Top+em.Height;
                style.HorizontalArrow(g, y, A.X, B.X, txt, true);
                g.DrawLine(Pens.Black, A.X, A.Y-style.point_width/2, A.X, y-0.75f*em.Height);
                g.DrawLine(Pens.Black, B.X, B.Y-style.point_width/2, B.X, y-0.75f*em.Height);
            }
            // Top dimensions

            // Diagonals
            using (Pen pen=style.MakePen(Color.LightGray))
            {
                pen.DashStyle=System.Drawing.Drawing2D.DashStyle.Dash;
                g.DrawLine(pen, A, B);
            }
            // Poles
            using (Pen pen=style.MakePen(Color.BurlyWood))
            {
                pen.Width=3;

                g.DrawLine(pen, A0, A);
                g.DrawLine(pen, A0.X-4, A0.Y, A0.X+4, A0.Y);
                g.DrawLine(pen, B0, B);
                g.DrawLine(pen, B0.X-4, B0.Y, B0.X+4, B0.Y);
                using (Font font=style.MakeFont(SystemFonts.DialogFont))
                {

                    var txt1=string.Format("{0:0.#}", catenary.StartPosition.y);
                    var txt2=string.Format("{0:0.#}", catenary.EndPosition.y);
                    var sf = new StringFormat(style.string_format)
                    {
                        FormatFlags = StringFormatFlags.DirectionVertical
                    };
                    SizeF sz1=g.MeasureString(txt1, font, A0, sf);
                    SizeF sz2=g.MeasureString(txt2, font, B0, sf);

                    g.DrawString(txt1, font, Brushes.Black, A0.X-sz1.Width/2, A0.Y+style.point_width/2);
                    g.DrawString(txt2, font, Brushes.Black, B0.X-sz2.Width/2, B0.Y+style.point_width/2);
                }
            }
            // Curves
            Func<double, Vector2> f=catenary.ParametricCurve;
            using (Pen pen=style.MakePen(Color.Blue))
            {
                const int N=64;
                PointF[] points=new PointF[N+1];
                for (int i=0; i<=N; i++)
                {
                    points[i]=canvas.Map(f(((double)i)/N));
                }
                pen.Width=selected?4f:style.pen_width;
                g.DrawCurve(pen, points);
            }
            // Points
            using (SolidBrush brush=style.MakeSolidBrush(Color.Red))
            {
                g.FillEllipse(brush, A.X-style.point_width/2, A.Y-style.point_width/2, style.point_width, style.point_width);
                g.FillEllipse(brush, B.X-style.point_width/2, B.Y-style.point_width/2, style.point_width, style.point_width);
                if (catenary.IsCenterInSpan)
                {
                    brush.Color=Color.Plum;
                    g.FillEllipse(brush, C.X-style.point_width/2, C.Y-style.point_width/2, style.point_width, style.point_width);
                }
            }
            // Clearance


            using (Pen pen=style.MakePen(Color.Black))
            {
                g.DrawEllipse(pen, A.X-style.point_width/2, A.Y-style.point_width/2, style.point_width, style.point_width);
                g.DrawEllipse(pen, B.X-style.point_width/2, B.Y-style.point_width/2, style.point_width, style.point_width);
                if (catenary.IsCenterInSpan)
                {
                    g.DrawEllipse(pen, C.X-style.point_width/2, C.Y-style.point_width/2, style.point_width, style.point_width);

                    var txt=string.Format("{0:0.#}", catenary.CenterY);
                    style.VerticalArrow(g, C.X, C0.Y, C.Y+style.point_width/2, txt, false);

                }
            }
            // Sag
            {
                Vector2 d=catenary.SagPosition;
                double t=(d.x-catenary.StartPosition.x)/catenary.SpanX;
                Vector2 h=catenary.StartPosition+t*(catenary.EndPosition-catenary.StartPosition);
                PointF D=canvas.Map(d);
                PointF H=canvas.Map(h);
                var txt=string.Format("{0:0.#}", catenary.MaximumSag);
                style.VerticalArrow(g, D.X, H.Y, D.Y, txt, false);
            }
        }
        #endregion

        #region IPrintable Members

        public void Render(Graphics g, PointF pos, SizeF bounds)
        {
            Render(g, new RectangleF(pos, bounds));
        }

        public SizeF GetSize(Graphics g, Rectangle page, PointF pos)
        {
            float wt=page.Width+page.Left-pos.X;
            float ht=0.2f*wt;
            return new SizeF(wt, ht);
        }
        public HorizontalAlignment Alignment
        {
            get { return align; }
            set { align=value; }
        }

        #endregion

        #region Mouse

        public void ClearMousePosition()
        {
            this.mouse_dn=Vector2.Origin;            
        }

        public void MouseDown(Control control, PointF point, MouseButtons button)
        {
            this.mouse_dn=Vector2.Origin;
            if (button==MouseButtons.Right)
            {
                RectangleF target=GetDrawArea(control);
                Canvas canvas=GetCanvasFor(target);
                mouse_dn=canvas.InvMap(point);
                mouse_over=mouse_dn;
            }
        }
        public void MouseUp(Control control, PointF point, MouseButtons button)
        {
        }
        public void MouseMove(Control control, PointF point, MouseButtons button)
        {
            RectangleF target=GetDrawArea(control);
            Canvas canvas=GetCanvasFor(target);
            mouse_over=canvas.InvMap(point);
            if (button==MouseButtons.Right)
            {
                mouse_dn=mouse_over;
            }
        }
        public void MouseLeave(Control control)
        {
        }
        #endregion



    }
    #endregion

}
