using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;
using System.Collections;

namespace JA
{
    public interface IVector2 : ICollection<double>, ICollection
    {
        bool IsZero { get; }
        double Manitude { get; }
        double X { get; }
        double Y { get; }
    }

    [DebuggerDisplay("({X},{Y})")]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public struct Vector2 : 
        IVector2, 
        IEquatable<Vector2>, 
        IFormattable
    {
        public static string DefaultFormat="0.###";

        #region Factory
        [DebuggerStepThrough()]
        public Vector2(double x, double y)
        {
            this.X=Math.Round(x, 12, MidpointRounding.AwayFromZero);
            this.Y=Math.Round(y, 12, MidpointRounding.AwayFromZero);
        }
        public static Vector2 Cartesian(double x, double y)
        {
            return new Vector2(x, y);
        }
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
        public double X { get; }
        [XmlAttribute(), Browsable(true), Bindable(BindableSupport.Yes)]
        public double Y { get; }
        [ReadOnly(true), XmlIgnore(), Browsable(false)]
        public double SumSquares { get { return X*X+Y*Y; } }
        [ReadOnly(true), XmlIgnore(), Browsable(true)]
        public double Manitude { get { return Math.Sqrt(X*X+Y*Y); } }
        [ReadOnly(true), XmlIgnore(), Browsable(false)]
        public double Angle { get { return Math.Atan2(Y, X); } }
        [ReadOnly(true), XmlIgnore(), Browsable(true)]
        public double AngleDegrees { get { return Math.Atan2(Y, X)*180/Math.PI; } }
        [ReadOnly(true), XmlIgnore(), Browsable(false)]
        public bool IsZero
        {
            get { return X.IsZero()&&Y.IsZero(); }
        }
        #endregion

        #region Operators
        public Vector2 Add(IVector2 other)
        {
            return new Vector2(X+other.X, Y+other.Y);
        }
        public Vector2 Scale(double factor)
        {
            return new Vector2(factor*X, factor*Y);
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
            return X.Equals(other.X)&&Y.Equals(other.Y);
        }

        /// <summary>
        /// Calculates the hash code for the <see cref="Vector2"/>
        /// </summary>
        /// <returns>The int hash value</returns>
        public override int GetHashCode()
        {
            return (17*23+X.GetHashCode())*23+Y.GetHashCode();
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
            return string.Format(provider, "({0:"+format+"},{1:"+format+"})", X, Y);
        }

        public IEnumerator<double> GetEnumerator()
        {
            yield return X;
            yield return Y;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public double[] ToArray() => new[] { X, Y };

        public void Add(double item)
        {
            throw new NotSupportedException();
        }

        public void Clear()
        {
            throw new NotSupportedException();
        }

        public bool Contains(double item)
        {
            throw new NotSupportedException();
        }

        public void CopyTo(double[] array, int arrayIndex)
        {
            Array.Copy(ToArray(), 0, array, arrayIndex, Count);
        }
        public void CopyTo(Array array, int arrayIndex)
        {
            Array.Copy(ToArray(), 0, array, arrayIndex, Count);
        }

        public bool Remove(double item)
        {
            throw new NotSupportedException();
        }
        object ICollection.SyncRoot => null;
        bool ICollection.IsSynchronized => false;
        public int Count { get => 2; }
        public bool IsReadOnly { get => true; }
        #endregion
    }
}
