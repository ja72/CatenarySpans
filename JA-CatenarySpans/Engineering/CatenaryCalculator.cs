using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JA.Engineering
{
    public static class CatenaryCalculator
    {
        #region Catenary Properties
        /// <summary>
        /// Calculate the lower point on the catenary (center point)
        /// </summary>
        /// <param name="span">The span</param>
        /// <param name="w">The unit weight</param>
        /// <param name="H">The horizontal tension</param>
        /// <returns>A position vector in (x,y) coordinates</returns>
        public static Vector2 CenterPosition(Vector2 span, double w, double H)
        {
            double a=H/w;
            double η=span.x/(2*a);

            double xc=span.x/2+a*DoubleEx.Asinh((span.y*Math.Exp(η))/(a*(1-Math.Exp(2*η))));
            double yc=-a*(Math.Cosh(-xc/a)-1);

            return new Vector2(xc, yc);
        }
        /// <summary>
        /// Calculate the x-coordinate for the point with maximum separation from the diagonal (sag point)
        /// </summary>
        /// <param name="span">The span</param>
        /// <param name="center">The center of the catenary (lower point)</param>
        /// <param name="w">The unit weight</param>
        /// <param name="H">The horizontal tension</param>
        /// <returns>A x-coordinate value</returns>
        public static double MaximumSagX(Vector2 span, Vector2 center, double w, double H)
        {
            double a=H/w;
            return center.x+a*DoubleEx.Asinh(span.y/span.x);
        }

        /// <summary>
        /// Calculate the maximum separation from the diagonal (sag point)
        /// </summary>
        /// <param name="span">The span</param>
        /// <param name="center">The center of the catenary (lower point)</param>
        /// <param name="w">The unit weight</param>
        /// <param name="H">The horizontal tension</param>
        /// <returns>A sag value</returns>
        public static double MaximumSag(Vector2 span, Vector2 center, double w, double H)
        {
            double a=H/w;
            double x=center.x+a*DoubleEx.Asinh(span.y/span.x);
            double y=center.y+a*(Math.Cosh((x-center.x)/a)-1);
            return span.y/span.x*x-y;
        }
        /// <summary>
        /// Calculate the maximum separation from the diagonal (sag point)
        /// <remarks>The lowest point on the catenary is calculated first.</remarks>
        /// </summary>
        /// <param name="span">The span</param>
        /// <param name="w">The unit weight</param>
        /// <param name="H">The horizontal tension</param>
        /// <returns>A sag value</returns>
        public static double MaximumSag(Vector2 span, double w, double H)
        {
            return MaximumSag(span, CenterPosition(span, w, H), w, H);
        }
        /// <summary>
        /// Calculate the separation from the diagonal at the mid point across the span.
        /// </summary>
        /// <param name="span">The span</param>
        /// <param name="center">The center of the catenary (lower point)</param>
        /// <param name="w">The unit weight</param>
        /// <param name="H">The horizontal tension</param>
        /// <returns>A sag value</returns>
        public static double MidSag(Vector2 span, Vector2 center, double w, double H)
        {
            return SagAtX(span, center, w, H, span.x/2);
        }
        /// <summary>
        /// Calculate the separation from the diagonal at a specified x-coordinate.
        /// </summary>
        /// <param name="span">The span</param>
        /// <param name="center">The center of the catenary (lower point)</param>
        /// <param name="w">The unit weight</param>
        /// <param name="H">The horizontal tension</param>
        /// <param name="x">The x-coordinate to use</param>
        /// <returns>A sag value</returns>
        public static double SagAtX(Vector2 span, Vector2 center, double w, double H, double x)
        {
            double a=H/w;
            double y=center.y+a*(Math.Cosh((x-center.x)/a)-1);
            return span.y/span.x*x-y;
        }
        /// <summary>
        /// Calculate the separation from the diagonal at a specified x-coordinate. 
        /// <remarks>The lowest point on the catenary is calculated first.</remarks>
        /// </summary>
        /// <param name="span">The span</param>
        /// <param name="w">The unit weight</param>
        /// <param name="H">The horizontal tension</param>
        /// <param name="x">The x-coordinate to use</param>
        /// <returns>A sag value</returns>
        public static double SagAtX(Vector2 span, double w, double H, double x)
        {
            return SagAtX(span, CenterPosition(span, w, H), w, H, x);
        }
        /// <summary>
        /// Calculate the total length of the curve.
        /// </summary>
        /// <param name="span">The span</param>
        /// <param name="center">The center of the catenary (lower point)</param>
        /// <param name="w">The unit weight</param>
        /// <param name="H">The horizontal tension</param>
        /// <returns>The length value</returns>
        public static double TotalLength(Vector2 span, Vector2 center, double w, double H)
        {
            return LengthSegmentAtX(span, center, w, H, span.x);
        }
        /// <summary>
        /// Calculate the total length of the curve.
        /// <remarks>The lowest point on the catenary is calculated first.</remarks>
        /// </summary>
        /// <param name="span">The span</param>
        /// <param name="w">The unit weight</param>
        /// <param name="H">The horizontal tension</param>
        /// <returns>The length value</returns>
        public static double TotalLength(Vector2 span, double w, double H)
        {
            return TotalLength(span, CenterPosition(span, w, H), w, H);
        }
        /// <summary>
        /// Calculate the position on the curve from the start to the x-coordinates specified.
        /// </summary>
        /// <param name="span">The span</param>
        /// <param name="center">The center of the catenary (lower point)</param>
        /// <param name="w">The unit weight</param>
        /// <param name="H">The horizontal tension</param>
        /// <param name="x">The x-coordinate</param>
        /// <returns>The position vector in (x,y) coordinates</returns>
        public static Vector2 PositionAtX(Vector2 span, Vector2 center, double w, double H, double x)
        {
            double a=H/w;
            return new Vector2(x, center.y+a*(Math.Cosh((x-center.x)/a)-1));
        }
        /// <summary>
        /// Calculate the position on the curve based on paramter <c>t</c> ranging from 0 to 1.
        /// </summary>
        /// <param name="span">The span</param>
        /// <param name="center"></param>
        /// <param name="w">The unit weight</param>
        /// <param name="H">The horizontal tension</param>
        /// <param name="t">The parameter from 0..1</param>
        /// <returns>The position vector in (x,y) coordinates</returns>
        public static Vector2 PositionAtT(Vector2 span, Vector2 center, double w, double H, double t)
        {
            double x=ParameterToX(span, center, w, H, t);
            return PositionAtX(span, center, w, H, x);
        }
        /// <summary>
        /// Calculate the length of the curve from the start to the x-coordinate specified.
        /// </summary>
        /// <param name="span">The span</param>
        /// <param name="center">The center of the catenary</param>
        /// <param name="w">The unit weight</param>
        /// <param name="H">The horizontal tension</param>
        /// <param name="x">The x-coordinate</param>
        /// <returns>The length value</returns>
        public static double LengthSegmentAtX(Vector2 span, Vector2 center, double w, double H, double x)
        {
            double a=H/w;
            return a*(Math.Sinh((x-center.x)/a)+Math.Sinh(center.x/a));
        }
        /// <summary>
        /// Calculate the vertical tension value at the x-coordinate specified.
        /// </summary>
        /// <param name="span">The span</param>
        /// <param name="center">The center of the catenary</param>
        /// <param name="w">The unit weight</param>
        /// <param name="H">The horizontal tension</param>
        /// <param name="x">The x-coordinate</param>
        /// <returns>The vertical tension value</returns>
        public static double VertricalTensionAtX(Vector2 span, Vector2 center, double w, double H, double x)
        {
            return H*Math.Sinh(w*(x-center.x)/H);
        }
        /// <summary>
        /// Calculate the total tension value at the x-coordinate specified.
        /// </summary>
        /// <param name="span">The span</param>
        /// <param name="center">The center of the catenary</param>
        /// <param name="w">The unit weight</param>
        /// <param name="H">The horizontal tension</param>
        /// <param name="x">The x-coordinate</param>
        /// <returns>The total tension value</returns>
        public static double TotalTensionAtX(Vector2 span, Vector2 center, double w, double H, double x)
        {
            return H*Math.Cosh(w*(x-center.x)/H);
        }
        /// <summary>
        /// Calculate the avarage tension along the curve, averaged across the length of the curve.
        /// </summary>
        /// <param name="span">The span</param>
        /// <param name="center">The center of the catenary</param>
        /// <param name="w">The unit weight</param>
        /// <param name="H">The horizontal tension</param>
        /// <returns>The average tension value</returns>
        public static double AverageTension(Vector2 span, Vector2 center, double w, double H)
        {
            double ξ=center.x/span.x;
            double η=w*span.x/(2*H);
            return H*(η+0.25*(Math.Sinh(4*η*(1-ξ))+Math.Sinh(4*η*ξ)))/(Math.Sinh(2*η*(1-ξ))+Math.Sinh(2*η*ξ));
        }
        /// <summary>
        /// Calculate the avarage tension along the curve, averaged across the length of the curve.
        /// <remarks>The lowest point on the catenary is calculated first.</remarks>
        /// </summary>
        /// <param name="span">The span</param>
        /// <param name="w">The unit weight</param>
        /// <param name="H">The horizontal tension</param>
        /// <returns>The average tension value</returns>
        public static double AverageTension(Vector2 span, double w, double H)
        {
            return AverageTension(span, CenterPosition(span, w, H), w, H);
        }
        
        #endregion        /// <summary>

        #region Catenrary Solvers

        public const double MinTension=1;
        public const double MinSag=1e-2;
        public const double MinExtension=1e-2;

        /// <summary>
        /// Calculates the horizontal tension needed to achieve specified average tension
        /// </summary>
        /// <param name="span">The span</param>
        /// <param name="w">The unit weight</param>
        /// <param name="P">The specified average tension</param>
        /// <param name="tol">The tension tolerance for numeric solution (default 0.001)</param>
        /// <returns></returns>
        public static double SetAverageTension(Vector2 span, double w, double P, double tol)
        {
            if (tol<=0) tol=1e-8;
            P=Math.Max(P, MinTension);
            double σ=span.x*w;
            double H_init=P/2+Math.Sqrt(DoubleEx.Sqr(P/2)-DoubleEx.Sqr(σ)/24);

            Func<double, double> f=(H_) => AverageTension(span, w, H_);
            if (f.Bisection(P, H_init, tol, out double H))
            {
                return H;
            }
            return H_init;
        }
        /// <summary>
        /// Calculates the horizontal tension needed to achieve specified maximum sag
        /// </summary>
        /// <param name="span">The span</param>
        /// <param name="w">The unit weight</param>
        /// <param name="D">The specified sag</param>
        /// <param name="tol">The tension tolerance for numeric solution (default 0.001)</param>
        /// <returns>The horizontal tension value</returns>
        public static double SetMaximumSag(Vector2 span, double w, double D, double tol)
        {
            if (tol<=0) tol=1e-4;
            D=Math.Max(D, MinSag);
            double H_init=DoubleEx.Sqr(span.x)*w/(8*D);

            Func<double, double> f=(H_) => MaximumSag(span, w, H_);

            if (f.Bisection(D, H_init, tol, out double H))
            {
                return H;
            }
            return H_init;
        }
        /// <summary>
        /// Calculates the horizontal tension needed to achieve specified curve length
        /// </summary>
        /// <param name="span">The span</param>
        /// <param name="w">The unit weight</param>
        /// <param name="L">The specified length</param>
        /// <param name="tol">The tension tolerance for numeric solution (default 0.001)</param>
        /// <returns>The horizontal tension value</returns>
        public static double SetTotalLength(Vector2 span, double w, double L, double tol)
        {
            if (tol<=0) tol=1e-6;
            L=Math.Max(L, MinExtension+span.Manitude);
            double H_init=L>span.Manitude?w*Math.Sqrt(DoubleEx.Cub(span.Manitude)/(24*(L-span.Manitude))):100000;

            Func<double, double> f=(H_) => TotalLength(span, w, H_);


            if (f.Bisection(L, H_init, tol, out double H))
            {
                return H;
            }
            return H_init;
        }
        /// <summary>
        /// Calculates the horizontal tension needed to get specified sag at a specified x-coordinate
        /// </summary>
        /// <param name="span">The span</param>
        /// <param name="w">The unit weight</param>
        /// <param name="D">The specified sag</param>
        /// <param name="x">The x-coordinate</param>
        /// <param name="tol">The tension tolerance for numeric solution (default 0.001)</param>
        /// <returns>The horizontal tension value</returns>
        public static double SetSagAtX(Vector2 span, double w, double D, double x, double tol)
        {
            if (tol<=0) tol=1e-4;
            D=Math.Max(D, MinSag);
            double H_init=(span.x*w/2)*x*(span.x-x)/(D*span.x-x*span.y);

            Func<double, double> f=(H_) => SagAtX(span, w, H_, x);

            if (f.Bisection(D, H_init, tol, out double H))
            {
                return H;
            }
            return H_init;
        }
        /// <summary>
        /// Finds the horizontal tension to set the lowest point on the catenary.
        /// </summary>
        /// <param name="span"></param>
        /// <param name="w"></param>
        /// <param name="C">The specified vertical separation between the left 
        ///                 support and the lowest point on the curve. Always positive.</param>
        /// <param name="tol"></param>
        /// <returns></returns>
        public static double SetClearance(Vector2 span, double w, double C, double tol)
        {
            if (tol<=0) tol=1e-4;
            C=Math.Min(C, MinSag);
            double H_init=DoubleEx.Sqr(span.x)*w/(8*C);
            Func<double, double> f=(H_) => -CenterPosition(span, w, H_).y;

            if (f.Bisection(C, H_init, tol, out double H))
            {
                return H;
            }
            return H_init;
        } 
        #endregion

        #region Other Methods
        /// Convert a parameter t ranging from 0..1 to the x-coordinate along the curve
        /// </summary>
        /// <param name="span">The span</param>
        /// <param name="center">The center of the catenary</param>
        /// <param name="w">The unit weight</param>
        /// <param name="H">The horizontal tension</param>
        /// <param name="t">A parameter from 0 to 1</param>
        /// <returns>The x-coordinate value</returns>
        public static double ParameterToX(Vector2 span, Vector2 center, double w, double H, double t)
        {
            double a=H/w;
            double L=TotalLength(span, center, w, H);
            return center.x+a*DoubleEx.Asinh(t*L/a-Math.Sinh(center.x/a));
        }
        
        #endregion

    }

}
