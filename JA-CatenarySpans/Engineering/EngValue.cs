using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;

namespace JA.Engineering
{
    using num=Double;

    /// <summary>
    /// Scientific,
    /// Engineering,
    /// </summary>
    public enum ExponentGroup
    {
        /// <summary>
        /// Any exponent is possible
        /// </summary>
        Scientific,
        /// <summary>
        /// Group exponents by 3
        /// </summary>
        Engineering,
        Default=Engineering
    }

    [DebuggerDisplay("{ToString()}")]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public struct EngValue : IEquatable<num>, ICloneable
    {
        public static string DefaultFormat="G";

        int exponent;
        double scalar;
        int sign;

        #region Factory
        EngValue(int sign, double value, int exponent)
        {
            this.sign=sign;
            this.scalar=value;
            this.exponent=exponent;
        }
        public EngValue(EngValue other)
        {
            this.sign=other.sign;
            this.scalar=other.scalar;
            this.exponent=other.exponent;
        }

        public EngValue(int amount) : this((double)amount, ExponentGroup.Default) { }
        public EngValue(int amount, ExponentGroup groups) : this((double)amount, groups) { }
        public EngValue(float amount) : this((double)amount, ExponentGroup.Default) { }
        public EngValue(float amount, ExponentGroup groups) : this((double)amount, groups) { }
        public EngValue(double amount) : this(amount, ExponentGroup.Default) { }
        public EngValue(double amount, ExponentGroup groups)
        {
            this.sign=Math.Sign(amount);
            amount=Math.Abs(amount);
            if (num.IsInfinity(amount)||num.IsNaN(amount))
            {
                this.scalar=amount;
                this.exponent=0;
            }
            else
            {
                if (amount>0.0)
                {
                    if (amount>1.0)
                    {
                        this.exponent=(int)(Math.Floor(Math.Log10(amount)/3.0)*3.0);
                    }
                    else
                    {
                        this.exponent=(int)(Math.Ceiling(Math.Log10(amount)/3.0)*3.0);
                    }
                    this.scalar=amount*Math.Pow(10.0, -exponent);
                    // Make adjustments and groups
                    if (groups==ExponentGroup.Engineering)
                    {
                        while (this.scalar>1e3)
                        {
                            this.scalar/=1e3;
                            this.exponent+=3;
                        }
                        while (this.scalar<0.1)
                        {
                            this.scalar*=1e3;
                            this.exponent-=3;
                        }
                    }
                    else if (groups==ExponentGroup.Scientific)
                    {
                        while (this.scalar>10)
                        {
                            this.scalar/=10;
                            this.exponent++;
                        }
                        while (this.scalar<0.1)
                        {
                            this.scalar*=10;
                            this.exponent--;
                        }
                    }
                    else // no grouping
                    {
                        // Do nothing
                    }
                }
                else
                {
                    this.exponent=0;
                    this.scalar=0;
                }
            }
        }
        /// <summary>
        /// Remove exponent from value. For example it coverts 10^3 into 1000.
        /// </summary>
        public EngValue ToScalar()
        {
            return new EngValue(sign, scalar*Math.Pow(10.0, exponent), 0);
        }
        public EngValue Round(int decimals)
        {
            return new EngValue(sign, Math.Round(scalar, decimals, MidpointRounding.AwayFromZero), exponent);
        }
        public static implicit operator EngValue(double x) { return new EngValue(x); }
        public static implicit operator double(EngValue rhs) { return rhs.Value; }
        public static implicit operator EngValue(float x) { return new EngValue(x); }
        public static implicit operator float(EngValue rhs) { return (float)rhs.Value; }
        #endregion

        #region Properties
        /// <summary>
        /// The scaled value (absolute)
        /// </summary>
        public double Scalar { get { return scalar; } }
        /// <summary>
        /// The scaled value (signed)
        /// </summary>
        public double SignedScalar { get { return sign*scalar; } }
        /// <summary>
        /// The value exponent
        /// </summary>
        public int Exponent { get { return exponent; } }
        /// <summary>
        /// The value sign
        /// </summary>
        public int Sign { get { return sign; } }
        /// <summary>
        /// The value represented by this EngValue (signed)
        /// </summary>
        public double Value { get { return sign*scalar*Math.Pow(10, exponent); } }
        /// <summary>
        /// The value represented by thus EngValue (absolute)
        /// </summary>
        public double AbsValue { get { return scalar*Math.Pow(10, exponent); } }

        /// <summary>
        /// Checks if values is a finite number, or infinity/NaN
        /// </summary>        
        public bool IsFinite
        {
            get
            {
                return !num.IsInfinity(scalar)&&!num.IsNaN(scalar);
            }
        }
        public bool IsPrefixed { get { return SI.ContainsKey(exponent); } }
        public string SiPrefix { get { return IsPrefixed?SI[exponent]:string.Empty; } }
        #endregion

        #region Functions

        #endregion

        #region IFormattable Members
        public override string ToString()
        {
            return ToString(DefaultFormat);
        }

        /// <summary>
        /// Format a value using max. width and significant digits
        /// </summary>
        /// <example>
        ///     Format("S10.4",-12345678.9) = " -12.34e-6"
        ///     10-column wide with 4 significant digits
        /// </example>
        /// <arg name="format">The format specifier</arg>
        /// <returns>A string representing the value formatted according to the format specifier</returns>
        public string ToString(string format)
        {
            if (IsFinite&&format.StartsWith("S"))
            {
                num value=Math.Round(scalar, 12); // remove any roundoff errors
                int max_width=0;
                int k=format.IndexOf('.');
                if (k>=0)
                {
                    int.TryParse(format.Substring(1, k-1), out max_width);
                    format=format.Substring(0, 1)+format.Substring(k+1);
                }
                string dg=format.Substring(1);
                int.TryParse(dg, out int significant_digits);
                if (significant_digits==0) significant_digits=4;

                string t1=ToScalar().ToString(max_width, significant_digits);
                string t2=ToString(max_width, significant_digits);
                string t=t1.Length<=t2.Length?t1:t2;
                if (max_width==0)
                {
                    return t;
                }
                else
                {
                    return string.Format("{0,"+max_width.ToString()+"}", t);
                }
            }
            else
            {
                return Value.ToString(format);
            }
        }

        /// <summary>
        /// Calls ToString(format) to display the value if format starts with "S", or defers to default number format
        /// </summary>
        /// <param name="format">The format to use. "S" formats use custom formatter</param>
        /// <param name="formatProvider"></param>
        /// <example>"S10.4" uses 10 wide field with 4 significant digits</example>
        /// <returns>A formatted string</returns>
        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (format.StartsWith("S"))
            {
                return ToString(format);
            }
            else
            {
                return string.Format(formatProvider, "{0:"+format+"}", Value);
            }
        }

        /// <summary>
        /// Format a value using max. width and significant digits
        /// </summary>
        /// <param name="max_width">The max. field width</param>
        /// <param name="significant_digits">The number of significant digits to show</param>
        /// <remarks>A formatted string</remarks>
        public string ToString(int max_width, int significant_digits)
        {
            int expsign=Math.Sign(exponent);
            exponent=Math.Abs(exponent);
            int digits=scalar>0?(int)Math.Log10(scalar)+1:0;
            int decimals=Math.Max(significant_digits-digits, 0);
            double round=Math.Pow(10, -decimals);
            digits=scalar>0?(int)Math.Log10(scalar+0.5*round)+1:0;
            decimals=Math.Max(significant_digits-digits, 0);
            string t;
            string f="0:F";
            if (exponent==0)
            {
                t=string.Format("{"+f+decimals+"}", sign*scalar);
            }
            else if (SI.ContainsKey(expsign*exponent))
            {
                t=string.Format("{"+f+decimals+"}{1}", sign*scalar, SI[expsign*exponent]);
            }
            else
            {
                t=string.Format("{"+f+decimals+"}e{1}", sign*scalar, expsign*exponent);
            }
            // Adjust decimal digits to fit column
            if (t.Length>max_width&&max_width!=0)
            {
                decimals=Math.Max(0, decimals-t.Length+max_width);
                if (exponent==0)
                {
                    t=string.Format("{"+f+decimals+"}", sign*scalar);
                }
                else if (SI.ContainsKey(expsign*exponent))
                {
                    t=string.Format("{"+f+decimals+"}{1}", sign*scalar, SI[expsign*exponent]);
                }
                else
                {
                    t=string.Format("{"+f+decimals+"}e{1}", sign*scalar, expsign*exponent);
                }
            }
            return t;
        }

        #endregion

        #region SI Prefixes
        internal readonly static Dictionary<int, string> SI=new Dictionary<int, string>()
        {
            { -21, "z" },
            { -18, "a" },
            { -15, "f" },
            { -12, "p" },
            { -9, "n" },
            { -6, "μ" },
            { -3, "m" },
            { -2, "c" },
            { -1, "d" },
            { 0, string.Empty },
            { 1, "da" },
            { 2, "h" },
            { 3, "k" },
            { 6, "M" },
            { 9, "G" },
            { 12, "T" },
            { 15, "P" },
            { 18, "E" },
            { 21, "Z" },
        };

        internal static bool FindExponent(ref string symbol, out int exponent)
        {
            exponent=0;

            // Check for correct case
            foreach (var item in SI)
            {
                if (item.Value.Length>0&&symbol.StartsWith(item.Value, StringComparison.InvariantCulture))
                {
                    symbol=symbol.Substring(item.Value.Length);
                    exponent=item.Key;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Check symbol if it might contain an SI prefix like "ms" for millisecond.
        /// First case-sensitive search is done, and then a case-insesitive search.
        /// </summary>
        /// <param name="symbol">The symbol to check</param>
        /// <param name="exponent">returns the exponent for the found prefix (if applicable)</param>
        /// <returns>True if a prefix fits the symbol</returns>
        internal static bool IsPrefixedSymbol(ref string symbol, out int exponent)
        {
            exponent=0;

            // Check for correct case
            foreach (var item in SI)
            {
                if (item.Value.Length>0&&symbol.Length>item.Value.Length&&symbol.StartsWith(item.Value, StringComparison.InvariantCulture))
                {
                    symbol=symbol.Substring(item.Value.Length);
                    exponent=item.Key;
                    return true;
                }
            }
            return false;
        }

        #endregion

        #region IScalarAlgebra<EngValue,double> Members

        public EngValue Scale(double factor)
        {
            return new EngValue(factor*Value);
        }

        public EngValue Reciprocal(double numerator)
        {
            return new EngValue(numerator/Value);
        }

        public EngValue Multiply(EngValue other)
        {
            return new EngValue(Value*other.Value);
        }

        public EngValue Divide(EngValue other)
        {
            return new EngValue(Value/other.Value);
        }

        #endregion

        #region IAlgebra<EngValue> Members

        public EngValue Add(EngValue other)
        {
            return new EngValue(Value+other.Value);
        }

        public EngValue Subtract(EngValue other)
        {
            return new EngValue(Value-other.Value);
        }

        public EngValue Negate()
        {
            return new EngValue(-sign, scalar, exponent);
        }

        #endregion

        #region IEquatable<EngValue> Members

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
        public static bool operator==(EngValue lhs, EngValue rhs) { return lhs.Equals(rhs); }
        public static bool operator!=(EngValue lhs, EngValue rhs) { return !lhs.Equals(rhs); }
        public override bool Equals(object obj)
        {
            if (obj is EngValue)
            {
                return Equals((EngValue)obj);
            }
            if (obj is IConvertible)
            {
                IConvertible cobj=obj as IConvertible;
                return Equals(cobj.ToDouble(CultureInfo.CurrentCulture));
            }
            return false;
        }
        public bool Equals(EngValue other)
        {
            return Equals(other.Value);
        }
        public bool Equals(EngValue other, double relative_tolerance)
        {
            return Equals(other.Value, relative_tolerance);
        }

        #endregion

        #region IEquatable<double> Members

        public bool Equals(double other)
        {
            return Math.Abs(this.Value-other)<1e-12;
        }
        public bool Equals(double other, double tolerance)
        {
            return Math.Abs(this.Value-other)<tolerance;
        }

        #endregion

        #region ICloneable Members

        public EngValue Clone() { return new EngValue(this); }

        object ICloneable.Clone()
        {
            return Clone();
        }

        #endregion

    }
}
