using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Runtime.InteropServices;

namespace JA.UI
{
    /// <summary>
    /// Color space with red, green, blue
    /// </summary>
    public readonly struct ColorRGB
    {
        public double R { get; }
        public double G { get; }
        public double B { get; }
        public double A { get; }

        public ColorRGB(double r, double g, double b)
            : this(r, g, b, 1) { }
        public ColorRGB(double r, double g, double b, double a) : this()
        {
            this.R=r;
            this.G=g;
            this.B=b;
            this.A=a;
        }

        #region Conversions
        public static implicit operator Color(ColorRGB rgb)
        {            
            return Color.FromArgb(
                (byte)DoubleEx.ClampMinMax(rgb.A*255, 0, 255),
                (byte)DoubleEx.ClampMinMax(rgb.R*255, 0, 255),
                (byte)DoubleEx.ClampMinMax(rgb.G*255, 0, 255),
                (byte)DoubleEx.ClampMinMax(rgb.B*255, 0, 255));
        }
        public static implicit operator ColorRGB(Color color)
        {
            return new ColorRGB(
                DoubleEx.ClampMinMax(color.R/255.0, 0, 1),
                DoubleEx.ClampMinMax(color.G/255.0, 0, 1),
                DoubleEx.ClampMinMax(color.B/255.0, 0, 1),
                DoubleEx.ClampMinMax(color.A/255.0, 0, 1)
            );
        }
        #endregion

        #region Adjustments

        public ColorRGB MoreOpaque(double amount) //opacify ??
        {
            return new ColorRGB(R, G, B, (1-amount)*A);
        }
        public ColorRGB LessOpaque(double amount) //translucify ??
        {
            return new ColorRGB(R, G, B, A+amount*(1-A));
        }

        public ColorRGB RotateAboutRed(double amount)
        {
            double c=DoubleEx.CosCircle(amount);
            double s=DoubleEx.SinCircle(amount);

            return new ColorRGB(R, c*G-s*B, s*G+c*B, A);
        }
        public ColorRGB RotateAboutGreen(double amount)
        {
            double c=DoubleEx.CosCircle(amount);
            double s=DoubleEx.SinCircle(amount);

            return new ColorRGB(c*R+s*B, G, -s*R+c*B, A);
        }
        public ColorRGB RotateAboutBlue(double amount)
        {
            double c=DoubleEx.CosCircle(amount);
            double s=DoubleEx.SinCircle(amount);

            return new ColorRGB(c*R-s*G, s*R+c*G, B, A);
        }

        #endregion
    }

    /// <summary>
    /// Color space with cyan, magenta and yellow
    /// </summary>
    public readonly struct ColorCMY
    {
        public double C { get; }
        public double M { get; }
        public double Y { get; }
        public double Z { get; }

        public ColorCMY(double c, double m, double y)
            : this(c, m, y, 1) { }
        public ColorCMY(double c, double m, double y, double z) : this()
        {
            this.C=c;
            this.M=m;
            this.Y=y;
            this.Z=z;
        }

        #region Conversions

        public static implicit operator Color(ColorCMY cmy)
        {
            return (Color)(ColorRGB)cmy;
        }

        public static implicit operator ColorCMY(Color color)
        {
            return (ColorCMY)(ColorRGB)color;
        }

        public static implicit operator ColorRGB(ColorCMY cmy)
        {
            return new ColorRGB(            
                -cmy.C+cmy.M+cmy.Y,
                cmy.C+cmy.M-cmy.Y,
                cmy.C-cmy.M+cmy.Y,
                cmy.Z
            );
        }

        public static implicit operator ColorCMY(ColorRGB rgb)
        {
            return new ColorCMY(
                (rgb.G+rgb.B)/2,
                (rgb.R+rgb.G)/2,
                (rgb.B+rgb.R)/2,
                rgb.A
            );
        }

        #endregion

        #region Adjustments
        public ColorCMY RotateAboutCyan(double amount)
        {
            double cs=DoubleEx.CosCircle(amount);
            double sn=DoubleEx.SinCircle(amount);

            return new ColorCMY(C, cs*M-sn*Y, sn*M+cs*Y, Z);
        }
        public ColorCMY RotateAboutMagenta(double amount)
        {
            double cs=DoubleEx.CosCircle(amount);
            double sn=DoubleEx.SinCircle(amount);

            return new ColorCMY(cs*C+sn*Y, M, -sn*C+cs*Y, Z);
        }
        public ColorCMY RotateAboutYellow(double amount)
        {
            double cs=DoubleEx.CosCircle(amount);
            double sn=DoubleEx.SinCircle(amount);

            return new ColorCMY(cs*C-sn*M, sn*C+cs*M, Y, Z);
        }

        #endregion
    }

    /// <summary>
    /// Color space with hue, saturation, lightness
    /// </summary>
    public readonly struct ColorHSL
    {
        public ColorHSL(double h, double s, double l) : this(h, s, l, 1) { }
        public ColorHSL(double h, double s, double l, double a) : this()
        {
            H=h;
            S=s;
            L=l;
            A=a;
        }

        public double H { get; }
        public double S { get;  }
        public double L { get;  }
        public double A { get; }

        #region Conversions
        public static implicit operator Color(ColorHSL hsl)
        {
            return (Color)(ColorRGB)hsl;
        }
        /// <summary>
        /// Convert HSL to RGB
        /// <remarks>
        ///     Code Taken from: http://christogreeff.blogspot.com/2008/06/hsl-to-rgb-conversion-for-gdi.html
        /// </remarks>
        /// </summary>
        /// <param name="hsl"></param>
        /// <returns></returns>
        public static implicit operator ColorRGB(ColorHSL hsl)
        {

            float l=(float)hsl.L;
            float h=(float)hsl.H;
            float s=(float)hsl.S;
            float[] x=new float[3];

            if(s.Equals(0))
            {
                x[0]=x[1]=x[2]=1f;
            }
            else
            {
                float p, q;
                q=l<0.5f?l*(1+s):l+s-(l*s);
                p=2*l-q;
                x[0]=h+(1f/3f);
                x[1]=h;
                x[2]=h-(1f/3f);
                for(byte i=0; i<3; i++)
                {
                    x[i]=x[i]<0?x[i]+1f:x[i]>1?x[i]-1:x[i];
                    if(x[i]*6f<1f)
                    {
                        x[i]=p+((q-p)*6f*x[i]);
                    }
                    else
                    {
                        if(x[i]*2f<1f)
                        {
                            x[i]=q;
                        }
                        else
                        {
                            if(x[i]*3f<2f)
                            {
                                x[i]=p+((q-p)*6f*((2f/3f)-x[i]));
                            }
                            else
                            {
                                x[i]=p;
                            }
                        }
                    }
                }
            }
            
            return new ColorRGB(
                x[0],
                x[1],
                x[2],
                hsl.A
            );
        }
        /// <summary>
        /// Convert RGB to HSL
        /// <remarks>
        ///     Code taken from: http://www.geekymonkey.com/Programming/CSharp/RGB2HSL_HSL2RGB.htm
        /// </remarks>
        /// </summary>
        /// <param name="rgb"></param>
        /// <returns></returns>
        public static implicit operator ColorHSL(ColorRGB rgb)
        {
            float r=(float)rgb.R;
            float g=(float)rgb.G;
            float b=(float)rgb.B;
            float v;
            float m;
            float vm;
            float r2, g2, b2;

            float h=0; // default to black
            float s=0;
            float l=0;
            v=Math.Max(r, g);
            v=Math.Max(v, b);
            m=Math.Min(r, g);
            m=Math.Min(m, b);
            l=(m+v)/2.0f;
            if(l>0.0)
            {
                vm=v-m;
                s=vm;
                if(s>0.0)
                {
                    s/=(l<=0.5f)?(v+m):(2.0f-v-m);
                    r2=(v-r)/vm;
                    g2=(v-g)/vm;
                    b2=(v-b)/vm;
                    if(r==v)
                    {
                        h=(g==m?5.0f+b2:1.0f-g2);
                    }
                    else if(g==v)
                    {
                        h=(b==m?1.0f+r2:3.0f-b2);
                    }
                    else
                    {
                        h=(r==m?3.0f+g2:5.0f-r2);
                    }
                    h/=6.0f;
                }
                else { s=0; }
            }
            else { l=0; }
            return new ColorHSL(            
                h,
                s,
                l,
                rgb.A
            );
        }
        public static implicit operator ColorHSL(Color color)
        {
            return (ColorHSL)(ColorRGB)color;
        }
        #endregion

        #region Adjustments

        /// <summary>
        /// Brightens color such that amount=100% results in luminosity=1.0, and amount=0% does not change anything
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public ColorHSL Brighten(double amount)
        {
            return new ColorHSL(            
                H,
                S,
                L+amount*(1-L),
                A
            );
        }
        /// <summary>
        /// Darkens color such that amount=100% results in luminosity=0.0, and amount=0% does not change anything
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public ColorHSL Darken(double amount)
        {
            return new ColorHSL(
                H,
                S,
                (1-amount)*L,
                A
            );
        }
        public ColorHSL MoreOpaque(double amount)
        {
            return new ColorHSL(            
                H,
                S,
                L,
                A+amount*(1-A)
            );
        }
        public ColorHSL LessOpaque(double amount)
        {
            return new ColorHSL(            
                H,
                S,
                L,
                (1-amount)*A
            );
        }
        public ColorHSL AddHue(double amount)
        {
            return new ColorHSL(
                DoubleEx.WrapAround(H+amount, 0, 1),
                S,
                L,
                A
            );
        }
        public ColorHSL SubtractHue(double amount)
        {
            return new ColorHSL(            
                DoubleEx.WrapAround(H-amount, 0, 1),
                S,
                L,
                A
            );
        }
        public ColorHSL Saturate(double amount)
        {
            return new ColorHSL(            
                H,
                S+amount*(1-S),
                L,
                A
            );
        }
        public ColorHSL DeSaturate(double amount)
        {
            return new ColorHSL(
                H,
                (1-amount)*S,
                L,
                A
            );
        }
        #endregion
    }

    public static class Colors
    {
        static class NativeMethods
        {
            #region Win32 GDI
            [DllImport("Gdi32.dll")]
            internal static extern int GetPixel(
                System.IntPtr hdc,    // handle to DC
                int nXPos,  // x-coordinate of pixel
                int nYPos   // y-coordinate of pixel
            );

            [DllImport("User32.dll")]
            internal static extern IntPtr GetDC(IntPtr wnd);

            [DllImport("User32.dll")]
            internal static extern void ReleaseDC(IntPtr dc);
        }
        #endregion

        static Random RandomGenerator=new Random();

        /// <summary>
        /// Eye dropper function for windows forms and controls
        /// </summary>
        /// <param name="control">The window or control to sample</param>
        /// <param name="point">The location of the pixel</param>
        /// <returns>A color object</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
        public static Color GetColorOfControl(System.Windows.Forms.Control control, Point point)
        {
            System.Drawing.Graphics g=control.CreateGraphics();
            IntPtr dc=g.GetHdc();
            Color res=ColorTranslator.FromWin32(NativeMethods.GetPixel(dc, point.X, point.Y));
            NativeMethods.ReleaseDC(dc);
            return res;
        }
        /// <summary>
        /// Eye dropper function for windows screen
        /// </summary>
        /// <param name="point">The location of the pixel</param>
        /// <returns>A color object</returns>
        public static Color GetColorOfScreen(Point point)
        {
            IntPtr dc=NativeMethods.GetDC(IntPtr.Zero);
            Color res=ColorTranslator.FromWin32(NativeMethods.GetPixel(dc, point.X, point.Y));
            NativeMethods.ReleaseDC(dc);
            return res;
        }

        public static ColorRGB CreateRGB(double red, double green, double blue)
        {
            return CreateRGB(red, green, blue, 1);
        }
        public static ColorRGB CreateRGB(double red, double green, double blue, double alpha)
        {
            return new ColorRGB(red, green, blue, alpha );
        }
        public static ColorHSL CreateHSL(double hue)
        {
            return CreateHSL(hue, 0.5);
        }
        public static ColorHSL CreateHSL(double hue, double luminosty)
        {
            return CreateHSL(hue, 0.5, luminosty);
        }
        public static ColorHSL CreateHSL(double hue, double saturation, double luminosty)
        {
            return CreateHSL(hue, saturation, luminosty, 1);
        }
        public static ColorHSL CreateHSL(double hue, double saturation, double luminosty, float alpha)
        {
            return new ColorHSL(hue, saturation, luminosty, alpha );
        }
        public static ColorHSL Random()
        {
            return Random(0.0);
        }
        public static ColorHSL Random(double alpha)
        {
            return Random(0.5, alpha);
        }
        public static ColorHSL Random(double luminosity, double alpha)
        {
            return Random(luminosity, 0.5, alpha);
        }
        public static ColorHSL Random(double luminosity, double saturation, double alpha)
        {
            double h=RandomGenerator.NextDouble();
            return new ColorHSL(h, saturation, luminosity, alpha );
        }
        public static Color RandomKnownColor()
        {
            return Color.FromKnownColor((KnownColor)RandomGenerator.Next(28, 167));
        }
        /// <summary>
        /// Increase color data such as when amount=1.0 result is RGB=(1.0,1.0,1.0)
        /// </summary>
        /// <arg name="color">The base color to use</arg>
        /// <arg name="amount">A value between 0.0 and 1.0</arg>
        /// <returns>A new <see cref="Color"/> object</returns>
        public static ColorHSL Brighten(Color color, double amount)
        {
            return ((ColorHSL)color).Brighten(amount);
        }
        /// <summary>
        /// Decrease color data such as when amount=1.0 result is RGB=(0.0,0.0,0.0)
        /// </summary>
        /// <arg name="color">The base color to use</arg>
        /// <arg name="amount">A value between 0.0 and 1.0</arg>
        /// <returns>A new <see cref="Color"/> object</returns>
        public static ColorHSL Darken(Color color, double amount)
        {
            return ((ColorHSL)color).Darken(amount);
        }
        public static ColorHSL AddHue(Color color, double amount)
        {
            return ((ColorHSL)color).AddHue(amount);
        }
        public static ColorHSL SubtractHue(Color color, double amount)
        {
            return ((ColorHSL)color).SubtractHue(amount);
        }
        public static ColorRGB RotateBlue(Color color, double amount)
        {
            return ((ColorRGB)color).RotateAboutBlue(amount);
        }
        public static ColorRGB RotateRed(Color color, double amount)
        {
            return ((ColorRGB)color).RotateAboutRed(amount);
        }
        public static ColorRGB RotateGreen(Color color, double amount)
        {
            return ((ColorRGB)color).RotateAboutGreen(amount);
        }
        public static ColorCMY RotateCyan(Color color, double amount)
        {
            return ((ColorCMY)color).RotateAboutCyan(amount);
        }
        public static ColorCMY RotateMagenta(Color color, double amount)
        {
            return ((ColorCMY)color).RotateAboutMagenta(amount);
        }
        public static ColorCMY RotateYellow(Color color, double amount)
        {
            return ((ColorCMY)color).RotateAboutYellow(amount);
        }

        public static ColorHSL Saturate(Color color, double amount)
        {
            return ((ColorHSL)color).Saturate(amount);
        }
        public static ColorHSL DeSaturate(Color color, double amount)
        {
            return ((ColorHSL)color).DeSaturate(amount);
        }

        public static ColorRGB LessOpaque(Color color, double amount)
        {
            return ((ColorRGB)color).LessOpaque(amount);
        }
        public static ColorRGB MoreOpaque(Color color, double amount)
        {
            return ((ColorRGB)color).MoreOpaque(amount);
        }

        public static float[] ToRGBAFloat(Color color)
        {
            return new float[] {
                (float)color.R / 255,
                (float)color.G / 255,
                (float)color.B / 255,
                (float)color.A / 255
            };
        }
        public static int[] ToRGBA(Color color)
        {
            return new int[] {
                color.R,
                color.G,
                color.B,
                color.A
            };
        }
        public static float[] ToRGBFloat(Color color)
        {
            return new float[] {
                (float)color.R / 255,
                (float)color.G / 255,
                (float)color.B / 255
            };
        }
        public static int[] ToRGB(Color color)
        {
            return new int[] {
                color.R,
                color.G,
                color.B
            };
        }
    }
}
