using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Diagnostics;
using System.Xml.Serialization;
using System.ComponentModel;
using System.Globalization;

namespace JA.Engineering
{
    /// <summary>
    /// Mass, Length, Time
    /// </summary>
    public enum Dimension
    {
        Mass,
        Length,
        Time
    }

    /// <summary>
    /// NewtonMeterSecond,
    /// NewtonMillimeterSecond,
    /// InchPoundSecond,
    /// FeetPoundSecond,
    /// InchOunceSecond,
    /// FeetOunceSecond,
    /// </summary>
    public enum ProjectUnitSystem
    {
        NewtonMeterSecond,
        NewtonMillimeterSecond,
        InchPoundSecond,
        FeetPoundSecond,
        InchOunceSecond,
        FeetOunceSecond,
    };

    #region Units
    [ImmutableObject(true)]
    public abstract class Unit : IEquatable<Unit>
    {
        public static readonly Unit m = new Base(BaseUnit.meter);
        public static readonly Unit @in = 0.0254 * m;
        public static readonly Unit ft = 12.0 * @in;
        public static readonly Unit mi = 5280.0 * ft;
        public static readonly Unit s = new Base(BaseUnit.second);
        public static readonly Unit min = 60.0 * s;
        public static readonly Unit hr = 60.0 * min;
        public static readonly Unit day = 24.0 * hr;
        public static readonly Unit wk = 7.0 * day;
        public static readonly Unit yr = 365.0 * day;
        public static readonly Unit mo = 30.0 * day;
        public static readonly Unit kg = new Base(BaseUnit.kilogram);
        public static readonly Unit g = 0.001 * kg;
        public static readonly Unit lbm = 0.45359237 * kg;
        public static readonly Unit oz = (1 / 16.0) * lbm;
        public static readonly Unit gee = 9.80665 * m / (s ^ 2);
        public static readonly Unit N = kg * m / (s ^ 2);
        public static readonly Unit kgf = kg * gee;
        public static readonly Unit tonne = 1000.0 * kg;
        public static readonly Unit ton = 2000.0 * lbm;
        public static readonly Unit lbf = 4.44822162 * N;
        public static readonly Unit ozf = (1 / 16.0) * lbf;

        #region Factory
        public static implicit operator Unit(string symbol) { return Parse(symbol); }
        public static implicit operator Unit(double factor) { return new Scalar(factor); }
        public static implicit operator double(Unit unit)
        {
            return unit.Scale;
        }

        public static Unit Parse(string symbol)
        {
            Unit a = null, b = null;
            if (ParseTwo(symbol, '/', ' ', ref a, ref b))
            {
                return a / b;
            }
            if (ParseTwo(symbol, '*', ' ', ref a, ref b))
            {
                return a * b;
            }
            if (ParseTwo(symbol, ' ', ' ', ref a, ref b))
            {
                return a * b;
            }
            if (ParseTwo(symbol, '^', '^', ref a, ref b))
            {
                return a ^ (int)b.Scale;
            }
            if (double.TryParse(symbol, out double x))
            {
                return new Scalar(x);
            }
            var fi = FindField(symbol);
            if (fi != null)
            {
                return fi.GetValue(null) as Unit;
            }
            var bu = BaseUnit.Parse(symbol);
            if (bu.IsOk)
            {
                return new Base(bu);
            }
            x = 1;
            if (EngValue.IsPrefixedSymbol(ref symbol, out int exp))
            {
                x *= Math.Pow(10, exp);
            }
            fi = FindField(symbol);
            if (fi != null)
            {
                return x * (fi.GetValue(null) as Unit);
            }

            return null;
        }

        static bool ParseTwo(string symbol, char token, char rem_token, ref Unit a, ref Unit b)
        {
            var parts = symbol.Split(token);
            if (parts.Length >= 2)
            {
                string sym1 = parts[0];
                string sym2 = string.Join(rem_token.ToString(), parts.Skip(1).ToArray());
                a = Unit.Parse(sym1);
                b = Unit.Parse(sym2);
                return a != null && b != null;
            }
            return false;
        }

        static FieldInfo FindField(string symbol)
        {
            Type u = typeof(Unit);
            var fields = u.GetFields(BindingFlags.Public | BindingFlags.Static);
            return fields.FirstOrDefault((fi) => fi.FieldType == u && fi.Name.Equals(symbol));
        }

        #endregion

        #region Properties & Methods
        public abstract string Symbol { get; }
        public abstract double Scale { get; }
        public abstract int Mass { get; }
        public abstract int Length { get; }
        public abstract int Time { get; }
        public bool IsOk { get { return Scale > 0; } }
        public bool IsScalar { get { return Mass == 0 && Length == 0 && Time == 0; } }

        public bool IsCompatible(Unit other)
        {
            return Time == other.Time
                && Length == other.Length
                && Mass == other.Mass;
        }

        public double FactorTo(Unit other)
        {
            if (IsCompatible(other))
            {
                return Scale / other.Scale;
            }
            throw new ArgumentException("Units must be combatible for conversion.");
        }

        public abstract bool Equals(Unit other);
        /// <summary>
        /// Equality overrides from <see cref="System.Object"/>
        /// </summary>
        /// <param name="obj">The object to compare this with</param>
        /// <returns>False if object is a different type, otherwise it calls <code>Equals(Unit)</code></returns>
        public override bool Equals(object obj)
        {
            if (obj is Unit unit)
            {
                return Equals(unit);
            }
            return false;
        }
        /// <summary>
        /// Calculates the hash code for the <see cref="Unit"/>
        /// </summary>
        /// <returns>The int hash value</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                if (string.IsNullOrEmpty(Symbol))
                {
                    int hc = 17*23;
                    hc = 23*hc + Mass.GetHashCode();
                    hc = 23*hc + Length.GetHashCode();
                    hc = 23*hc + Time.GetHashCode();
                    hc = 23*hc + Scale.GetHashCode();
                    return hc;
                }
                return Symbol.GetHashCode();
            }
        }

        #endregion

        #region Operators
        public static Unit operator *(Unit a, Unit b)
        {
            return new Product(a, b);
        }
        public static Unit operator *(double a, Unit b)
        {
            return new Product(a, b);
        }
        public static Unit operator *(Unit a, double b)
        {
            return new Product(a, b);
        }
        public static Unit operator /(Unit a, Unit b)
        {
            return new Ratio(a, b);
        }
        public static Unit operator /(double a, Unit b)
        {
            return new Ratio(a, b);
        }
        public static Unit operator /(Unit a, double b)
        {
            return new Ratio(a, b);
        }
        public static Unit operator ^(Unit a, int exp)
        {
            return new Exponent(a, exp);
        }
        #endregion

        #region Implementations

        sealed class Scalar : Unit, IEquatable<Scalar>
        {
            readonly double scalar;
            public Scalar(double scalar) { this.scalar = scalar; }
            public override double Scale { get { return scalar; } }
            public override int Mass { get { return 0; } }
            public override int Length { get { return 0; } }
            public override int Time { get { return 0; } }
            public override string Symbol { get { return scalar.ToString(); } }

            #region IEquatable Members

            public override bool Equals(Unit other)
            {
                if (other is Scalar)
                {
                    return Equals(other as Scalar);
                }
                return false;
            }

            public bool Equals(Scalar other)
            {
                return Math.Abs(scalar - other.scalar) <= 1e-12;
            }

            #endregion

        }

        sealed class Base : Unit, IEquatable<Base>
        {
            readonly BaseUnit u;
            public Base(BaseUnit unit) { this.u = unit; }
            public override double Scale { get { return u.Scale; } }
            public override int Length { get { return u.Dimension == Dimension.Length ? 1 : 0; } }
            public override int Mass { get { return u.Dimension == Dimension.Mass ? 1 : 0; } }
            public override int Time { get { return u.Dimension == Dimension.Time ? 1 : 0; } }
            public override string Symbol
            {
                get
                {
                    var fi = FindFieldFromDimension(u.Dimension);
                    if (fi != null)
                    {
                        return fi.Name;
                    }
                    return u.Symbol;
                }
            }
            static FieldInfo FindFieldFromDimension(Dimension dim)
            {
                var fields = typeof(Unit).GetFields(BindingFlags.Public | BindingFlags.Static);
                return fields.FirstOrDefault((fi) =>
                {
                    if (fi.FieldType == typeof(Unit))
                    {
#pragma warning disable S1066 // Collapsible "if" statements should be merged
                        if (fi.GetValue(null) is Base @base)
#pragma warning restore S1066 // Collapsible "if" statements should be merged
                        {
                            return @base.u.Dimension == dim;
                        }
                    }
                    return false;
                });
            }

            #region IEquatable Members

            public override bool Equals(Unit other)
            {
                if (other is Base)
                {
                    return Equals(other as Base);
                }
                return false;
            }

            public bool Equals(Base other)
            {
                return u.Equals(other.u);
            }

            #endregion

        }


        sealed class Product : Unit, IEquatable<Product>
        {
            public Product(Unit a, Unit b) { this.A = a; this.B = b; }
            public override double Scale { get { return A.Scale * B.Scale; } }
            public override int Mass { get { return A.Mass + B.Mass; } }
            public override int Length { get { return A.Length + B.Length; } }
            public override int Time { get { return A.Time + B.Time; } }
            public override string Symbol { get { return string.Format("{0} {1}", A.Symbol, B.Symbol); } }
            public Unit A { get; }
            public Unit B { get; }

            #region IEquatable Members

            public override bool Equals(Unit other)
            {
                if (other is Product)
                {
                    return Equals(other as Product);
                }
                return false;
            }

            public bool Equals(Product other)
            {
                return A.Equals(other.A) && B.Equals(other.B);
            }

            #endregion

        }
        sealed class Ratio : Unit, IEquatable<Ratio>
        {
            public Ratio(Unit a, Unit b) { this.A = a; this.B = b; }
            public override double Scale { get { return A.Scale / B.Scale; } }
            public override int Mass { get { return A.Mass - B.Mass; } }
            public override int Length { get { return A.Length - B.Length; } }
            public override int Time { get { return A.Time - B.Time; } }
            public override string Symbol { get { return string.Format("{0}/{1}", A.Symbol, B.Symbol); } }
            public Unit A { get; }
            public Unit B { get; }

            #region IEquatable Members

            public override bool Equals(Unit other)
            {
                if (other is Ratio)
                {
                    return Equals(other as Ratio);
                }
                return false;
            }

            public bool Equals(Ratio other)
            {
                return A.Equals(other.A) && B.Equals(other.B);
            }

            #endregion

        }

        sealed class Exponent : Unit, IEquatable<Exponent>
        {
            public Exponent(Unit a, int exp) { this.A = a; this.Exp = exp; }
            public override double Scale { get { return Math.Pow(A.Scale, Exp); } }
            public override int Mass { get { return Exp * A.Mass; } }
            public override int Length { get { return Exp * A.Length; } }
            public override int Time { get { return Exp * A.Time; } }
            public override string Symbol { get { return string.Format("{0}^{1}", A.Symbol, Exp); } }
            public Unit A { get; }
            public int Exp { get; }

            #region IEquatable Members

            public override bool Equals(Unit other)
            {
                if (other is Exponent)
                {
                    return Equals(other as Exponent);
                }
                return false;
            }

            public bool Equals(Exponent other)
            {
                return A.Equals(other.A) && Exp.Equals(other.Exp);
            }

            #endregion
        }
        #endregion

        #region Event Helpers

        public static bool AreEqual(string old_unit, string new_unit)
        {
            Unit prev = Unit.Parse(old_unit);
            Unit next = Unit.Parse(new_unit);
            if (prev != null && next != null)
            {
                return prev.Equals(next);
            }
            return false;
        }

        #endregion

    }
    #endregion

    #region Base Units
    [ImmutableObject(true)]
    sealed class BaseUnit : IEquatable<BaseUnit>
    {
        #region Basic Units
        public static readonly BaseUnit meter = Dimension.Length;
        public static readonly BaseUnit second = Dimension.Time;
        public static readonly BaseUnit kilogram = Dimension.Mass;

        #endregion

        #region Factory
        public BaseUnit(double scale, Dimension dim)
        {
            this.Scale = new EngValue(scale, ExponentGroup.Engineering);
            this.Dimension = dim;
        }
        public static readonly BaseUnit Undefined = new BaseUnit(0, 0);
        public static implicit operator BaseUnit(Dimension dim) { return new BaseUnit(1, dim); }
        public static BaseUnit operator *(double factor, BaseUnit unit)
        {
            return new BaseUnit(factor * unit.Scale, unit.Dimension);
        }
        public static BaseUnit operator /(BaseUnit unit, double div)
        {
            return new BaseUnit(unit.Scale / div, unit.Dimension);
        }
        #endregion

        #region Properties
        public bool IsOk { get { return Scale > 0; } }
        public Dimension Dimension { get; }
        public EngValue Scale { get; }
        public string Symbol
        {
            get
            {
                // 1000 MASS = 1000 kg = 1000,0000 g = 1 Mg
                string sym = FindBaseSymbol(Dimension);

                double x = Scale.Value;
                EngValue.IsPrefixedSymbol(ref sym, out int exp);
                string si = string.Empty;
                if (Scale.IsPrefixed)
                {
                    exp += Scale.Exponent;
                    x *= Math.Pow(10, -Scale.Exponent);
                    si = EngValue.SI[exp];
                }
                if (x == 1)
                {
                    return string.Format("{0}{1}", si, sym);
                }
                else
                {
                    return string.Format("{0} {1}{2}", x, si, sym);
                }
            }
        }

        public override string ToString()
        {
            return Symbol;
        }
        #endregion

        #region Parsing

        static string FindBaseSymbol(Dimension dimension)
        {
            var fi = FindField(dimension);
            return fi.Name;
        }
        public static BaseUnit Parse(string symbol)
        {
            if (FindUnit(symbol, out BaseUnit unit))
            {
                return unit;
            }
            if (EngValue.IsPrefixedSymbol(ref symbol, out int exp))
            {
                return Math.Pow(10, exp) * Parse(symbol);
            }
            return Undefined;
        }

        #region Helper Functions
        internal static FieldInfo FindField(Dimension dimension)
        {
            Type bu = typeof(BaseUnit);
            var fields = bu.GetFields(BindingFlags.Public | BindingFlags.Static);
            return fields.FirstOrDefault((fi) => fi.FieldType == bu && ((BaseUnit)fi.GetValue(null)).Dimension == dimension);
        }
        internal static FieldInfo FindField(string symbol)
        {
            Type bu = typeof(BaseUnit);
            var fields = bu.GetFields(BindingFlags.Public | BindingFlags.Static);
            return fields.FirstOrDefault((fi) => fi.FieldType == bu && fi.Name.Equals(symbol));
        }
        internal static bool FindUnit(string symbol, out BaseUnit unit)
        {
            var fi = FindField(symbol);
            if (fi != null)
            {
                unit = (BaseUnit)fi.GetValue(null);
                return true;
            }
            unit = null;
            return false;
        }

        #endregion

        #endregion

        #region IEquatable Members

        /// <summary>
        /// Equality overrides from <see cref="System.Object"/>
        /// </summary>
        /// <param name="obj">The object to compare this with</param>
        /// <returns>False if object is a different type, otherwise it calls <code>Equals(BaseUnit)</code></returns>
        public override bool Equals(object obj)
        {
            if (obj is BaseUnit unit)
            {
                return Equals(unit);
            }
            return false;
        }

        /// <summary>
        /// Checks for equality among <see cref="BaseUnit"/> classes
        /// </summary>
        /// <param name="other">The other <see cref="BaseUnit"/> to compare it to</param>
        /// <returns>True if equal</returns>
        public bool Equals(BaseUnit other)
        {
            return Scale.Equals(other.Scale) && Dimension == other.Dimension;
        }

        /// <summary>
        /// Calculates the hash code for the <see cref="BaseUnit"/>
        /// </summary>
        /// <returns>The int hash value</returns>
        public override int GetHashCode()
        {
            return (17 * 23 + Scale.GetHashCode()) * 23 + Dimension.GetHashCode();
        }

        #endregion


    }
    #endregion

    #region Events
    public class UnitEventArgs : EventArgs
    {
        readonly Unit units;

        public UnitEventArgs(Unit units)
        {
            this.units = units;
        }

        public Unit Units { get { return units; } }
    }

    public class UnitChangeEventArgs : EventArgs
    {
        readonly string old_symbol, new_symbol;
        readonly Unit old_units, new_units;
        readonly double factor;

        public UnitChangeEventArgs(string old_symbol, string new_symbol)
        {
            this.old_symbol = old_symbol;
            this.new_symbol = new_symbol;
            this.old_units = old_symbol;
            this.new_units = new_symbol;
            this.factor = old_units.Scale / new_units.Scale;
        }

        public string OldSymbol { get { return old_symbol; } }
        public string NewSynmbol { get { return new_symbol; } }
        public Unit OldUnits { get { return old_units; } }
        public Unit NewUnits { get { return new_units; } }
        public double Factor { get { return factor; } }
    }
    #endregion

}
