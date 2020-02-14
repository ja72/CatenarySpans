using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace JA.Gdi
{
    public class Canvas
    {
        Vector2 min_position, max_position;
        PointF origin = new PointF(0,0);
        SizeF scale = new SizeF(1,1);
        bool iso_scale=false;
        RectangleF target;
        float padding=18f;

        public Canvas(RectangleF target, Vector2 min_position, Vector2 max_position)
        {
            this.min_position=min_position;
            this.max_position=max_position;
            this.target=target;
            FitDrawing();

        }
        void FitDrawing()
        {
            // ox+sx*fx =L, oy-sy*fy = T
            // ox+sx*tx =R, oy-sy*ty = B
            float L=target.Left+padding;
            float R=target.Right-padding;
            float T=target.Top+padding;
            float B=target.Bottom-padding;
            B=Math.Max(B, T+1); // allow for at least one pixel to display
            R=Math.Max(R, L+1); // allow for at least one pixel

            double dx=max_position.x-min_position.x;
            double dy=max_position.y-min_position.y;

            if(dx>0&&dy>0)
            {
                origin=new PointF(
                    L+(float)(min_position.x*(L-R)/dx),
                    B+(float)(min_position.y*(B-T)/dy));

                scale=new SizeF(
                    (float)((R-L)/dx),
                    (float)((B-T)/dy));

                if(iso_scale)
                {
                    float s=(float)Math.Sqrt(scale.Width*scale.Width+scale.Height*scale.Height);
                    scale=new SizeF(s, s);
                }
            }
            else
            {
                origin=new PointF(0, 0);
                scale=new SizeF(1, 1);
            }
            
        }
        public RectangleF Target { get { return target; } set { target=value; } }
        public float Padding { get { return padding; } set { padding=value; } }
        public Vector2 MinPosition { get { return min_position; } set { min_position=value; FitDrawing(); } }
        public Vector2 MaxPosition { get { return max_position; } set { max_position=value; FitDrawing(); } }
        public bool Isometric { get { return iso_scale; } set { iso_scale=value; FitDrawing(); } }
        public PointF Origin { get { return origin; } }
        public SizeF Scale { get { return scale; } }

        public PointF Map(double x, double y)
        {
            return new PointF(
                origin.X + (float)(scale.Width * x), 
                origin.Y - (float)(scale.Height * y));
        }
        public PointF Map(IVector2 position)
        {
            return Map(position.X, position.Y);
        }

        public Vector2 InvMap(PointF point)
        {
            return new Vector2(
                (point.X - origin.X) / scale.Width,
                -(point.Y - origin.Y) / scale.Height);
        }
    }
}
