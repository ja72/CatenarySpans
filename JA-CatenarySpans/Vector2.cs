using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace JA
{
    public interface IVector2
    {
        bool IsZero { get; }
        double Manitude { get; }
        double X { get; set; }
        double Y { get; set; }
    }

    [DebuggerDisplay("({X},{Y})")]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public struct Vector2 : IVector2, IEquatable<Vector2>, ICloneable, IFormattable
    {
        public static string DefaultFormat="0.###";
        public double x, y;

        #region Factory
        [DebuggerStepThrough()]
        public Vector2(double x, double y)
        {
            this.x=Math.Round(x, 12, MidpointRounding.AwayFromZero);
            this.y=Math.Round(y, 12, MidpointRounding.AwayFromZero);
        }
        [DebuggerStepThrough()]
        public Vector2(Vector2 other) { this=other; }
        /// <summary>
        /// Vector from polar coordinates.
        /// </summary>
        /// <param name="r">The distance frrom the origin (radius)</param>
        /// <param name="θ">The angle from horizontal in radians</param>
        /// <returns>A vector</returns>
        public static Vector2 Polar(double r, double θ)
        {
            return new Vector2(r*Math.Cos(θ), r*Math.Sin(θ));
        }
        public static readonly Vector2 Origin=new Vector2(0, 0);
        public static readonly Vector2 UnitX=new Vector2(1, 0);
        public static readonly Vector2 UnitY=new Vector2(0, 1);
        #endregion

        #region Properties
        [XmlAttribute(), Browsable(true), Bindable(BindableSupport.Yes)]
        public double X
        {
            get { return x; }
            set { x=Math.Round(value, 12, MidpointRounding.AwayFromZero); }
        }
        [XmlAttribute(), Browsable(true), Bindable(BindableSupport.Yes)]
        public double Y
        {
            get { return y; }
            set { y=Math.Round(value, 12, MidpointRounding.AwayFromZero); }
        }
        [ReadOnly(true), XmlIgnore(), Browsable(false)]
        public double SumSquares { get { return x*x+y*y; } }
        [ReadOnly(true), XmlIgnore(), Browsable(true)]
        public double Manitude { get { return Math.Sqrt(x*x+y*y); } }
        [ReadOnly(true), XmlIgnore(), Browsable(false)]
        public double Angle { get { return Math.Atan2(y, x); } }
        [ReadOnly(true), XmlIgnore(), Browsable(true)]
        public double AngleDegrees { get { return Math.Atan2(y, x)*180/Math.PI; } }
        [ReadOnly(true), XmlIgnore(), Browsable(false)]
        public bool IsZero
        {
            get { return x.IsZero()&&y.IsZero(); }
        }
        #endregion

        #region Operators
        public Vector2 Add(IVector2 other)
        {
            return new Vector2(x+other.X, y+other.Y);
        }
        public Vector2 Scale(double factor)
        {
            return new Vector2(factor*x, factor*y);
        }
        public static Vector2 operator+(Vector2 a, Vector2 b)
        {
            return a.Add(b);
        }
        public static Vector2 operator-(Vector2 a)
        {
            return a.Scale(-1);
        }
        public static Vector2 operator-(Vector2 a, Vector2 b)
        {
            return a.Add(-b);
        }
        public static Vector2 operator*(double a, Vector2 b)
        {
            return b.Scale(a);
        }
        public static Vector2 operator*(Vector2 a, double b)
        {
            return a.Scale(b);
        }
        public static Vector2 operator/(Vector2 a, double b)
        {
            return a.Scale(1/b);
        }
        #endregion

        #region IEquatable Members

        public static bool operator==(Vector2 lhs, Vector2 rhs) { return lhs.Equals(rhs); }
        public static bool operator!=(Vector2 lhs, Vector2 rhs) { return !lhs.Equals(rhs); }

        /// <summary>
        /// Equality overrides from <see cref="System.Object"/>
        /// </summary>
        /// <param name="obj">The object to compare this with</param>
        /// <returns>False if object is a different type, otherwise it calls <code>Equals(Vector2)</code></returns>
        public override bool Equals(object obj)
        {
            if (obj is Vector2)
            {
                return Equals((Vector2)obj);
            }
            return false;
        }

        /// <summary>
        /// Checks for equality among <see cref="Vector2"/> classes
        /// </summary>
        /// <param name="other">The other <see cref="Vector2"/> to compare it to</param>
        /// <returns>True if equal</returns>
        public bool Equals(Vector2 other)
        {
            return x.Equals(other.x)&&y.Equals(other.y);
        }

        /// <summary>
        /// Calculates the hash code for the <see cref="Vector2"/>
        /// </summary>
        /// <returns>The int hash value</returns>
        public override int GetHashCode()
        {
            return (17*23+x.GetHashCode())*23+y.GetHashCode();
        }

        #endregion

        #region ICloneable Members

        public Vector2 Clone() { return new Vector2(this); }

        object ICloneable.Clone()
        {
            return Clone();
        }

        #endregion

        #region Formatting
        public override string ToString()
        {
            return ToString(DefaultFormat);
        }
        public string ToString(string format)
        {
            return ToString(format, null);
        }
        public string ToString(string format, IFormatProvider provider)
        {
            return string.Format(provider, "({0:"+format+"},{1:"+format+"})", x, y);
        }
        #endregion
    }
}
