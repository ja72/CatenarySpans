using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Xml;
using System.Xml.Serialization;

namespace JA.Engineering
{
    /// <summary>
    /// Generic stress strain curve
    /// </summary>
    public interface IStress
    {
        double MaxStrain { get; }
        double MaxStress { get; }
        double StressOf(double strain);
        double StrainOf(double stress);
    }
    /// <summary>
    /// Stress strain curve based on the curve <c>y=S*(1-EXP(-Q*x^m))</c>
    /// </summary>
    public class ExponentialStress : IStress, ICloneable
    {
        const double break_threshold=0.99d;
        double σU;
        double Q;
        double m;

        #region Factory
        public ExponentialStress(ExponentialStress other)
            : this(other.MaxStress, other.Q, other.m)
        { }

        public ExponentialStress(PolynomialStress other)
            : this(other.MaxStress, other.StressOf(0.0035), 0.0035, other.StressOf(0.0075), 0.0075)
        { }

        /// <summary>
        /// Create a new stress-strain curve
        /// </summary>
        /// <param name="σU">The maximum theoretical stress</param>
        /// <param name="Q">Coefficient of stress-strain curve</param>
        /// <param name="m">Exponent of stress-strain curve</param>
        public ExponentialStress(double σU, double Q, double m)
        {
            σU/=break_threshold;
            this.σU=σU;
            this.Q=Q;
            this.m=m;
        }

        /// <summary>
        /// Build a new stress-strain curve from two control points
        /// </summary>
        /// <param name="σU">The maximum theoretical stress</param>
        /// <param name="σ_1">The first stress (psi)</param>
        /// <param name="ε_1">The first strain (in/in)</param>
        /// <param name="σ_2">The second stress (psi)</param>
        /// <param name="ε_2">The second strain (in/in)</param>
        public ExponentialStress(double σU, double σ_1, double ε_1, double σ_2, double ε_2)
        {
            σU/=break_threshold;
            this.σU=σU;
            double k1=1d-σ_1/σU;
            double k2=1d-σ_2/σU;
            // convert strain to %
            ε_1*=100;
            ε_2*=100;
            this.m=Math.Log(Math.Log(k1)/Math.Log(k2))/(Math.Log(ε_1)-Math.Log(ε_2));
            this.Q=Math.Log(k1/k2)/(Math.Pow(ε_2, m)-Math.Pow(ε_1, m));
        }

        /// <summary>
        /// Build a new generic stress-strain curve
        /// </summary>
        /// <param name="σU">The maximum theoretical stress</param>
        /// <returns></returns>
        public ExponentialStress(double σU) : this(σU, 2.684063652, 1.193882071) { }
        #endregion

        #region Properties
        [XmlAttribute()]
        public double PeakStress { get { return σU; } set { σU=value; } }
        [XmlAttribute()]
        public double Exponent { get { return m; } set { m=value; } }
        [XmlAttribute()]
        public double Coefficient { get { return Q; } set { Q=value; } }
        [XmlIgnore()]
        public double MaxStress { get { return break_threshold*σU; } }
        [XmlIgnore()]
        public double MaxStrain { get { return StrainOf(MaxStress); } }
        #endregion

        #region Methods
        /// <summary>
        /// Get stress curve value
        /// </summary>
        /// <param name="strain">The strain in in/in</param>
        /// <returns>Stress value</returns>
        public double StressOf(double strain)
        {
            if (strain<=0d)
            {
                return 0d;
            }
            else
            {
                // convert strain to %
                strain*=100;
                return σU*(1d-Math.Exp(-Q*Math.Pow(strain, m)));
            }
        }

        public double StrainOf(double stress)
        {
            if (stress<=0d)
            {
                return 0;
            }
            else if (stress>=MaxStress)
            {
                return Math.Pow(-Math.Log(1d-break_threshold)/Q, 1d/m)/100;
            }
            else
            {
                return Math.Pow(-Math.Log(1d-stress/σU)/Q, 1d/m)/100;
            }
        }

        public override string ToString()
        {
            return string.Format("Exponential Curve, MaxStress={0:0.#} MaxStrain={1:0.###}",
                MaxStress, MaxStrain);
        }
        #endregion

        #region ICloneable Members

        public ExponentialStress Clone() { return new ExponentialStress(this); }

        object ICloneable.Clone()
        {
            return Clone();
        }

        #endregion

    }
    /// <summary>
    /// Strain strain curve based on a polynomial function, in the spirit of the Alcoa charts
    /// </summary>
    public class PolynomialStress : IStress, ICloneable
    {
        Polynomial curve;
        double max_strain;

        #region Factory
        public PolynomialStress()
        {
            curve=0;
            this.max_strain=1;
        }
        public PolynomialStress(PolynomialStress other)
        {
            this.max_strain=other.max_strain;
            this.curve=new Polynomial(other.curve);
        }
        //public PolynomialStress(params double[] coefficients) : this(0.008, coefficients) { }
        public PolynomialStress(double max_strain, params double[] coefficients)
        {
            this.max_strain=max_strain;
            this.curve=coefficients;
        }
        public PolynomialStress(double max_strain, Polynomial poly)
        {
            this.max_strain=max_strain;
            this.curve=poly;
        }
        #endregion

        #region Properties
        public double StrainOffset
        {
            get
            {
                if (curve.FindRoot(0, out double strain))
                {
                    return strain / 100;
                }
                return 0;
            }
        }
        #endregion

        #region ICloneable Members

        public PolynomialStress Clone() { return new PolynomialStress(this); }

        object ICloneable.Clone()
        {
            return Clone();
        }

        #endregion

        #region IStressStrain Members
        [XmlAttribute]
        public string Coefficients
        {
            get { return curve.Coefficients.ToCSV(); }
            set { curve=value.FromCSV(); }
        }
        [XmlIgnore()]
        public Polynomial Curve { get { return curve; } set { curve=value; } }
        [XmlAttribute()]
        public double MaxStrain
        {
            get { return max_strain; }
            set { max_strain=value; }
        }
        [XmlIgnore()]
        public double MaxStress
        {
            get { return InnerStressOf(max_strain); }
            set
            {
                max_strain=StrainOf(value);
            }
        }
        double InnerStressOf(double strain)
        {
            // keep only positive (tensile) strain
            strain=Math.Max(strain, 0);
            double σ=curve.Evaluate(strain);
            // keep only positive (tensile) stress
            return Math.Max(σ, 0);
        }
        public double StressOf(double strain)
        {
            strain=Math.Max(0, Math.Min(max_strain, strain));
            return InnerStressOf(strain);
        }

        public double StrainOf(double stress)
        {
            if (curve.FindRoot(stress, out double strain))
            {
                return strain;
            }
            throw new ArgumentException();
        }

        #endregion

        #region Operations

        public static PolynomialStress operator-(PolynomialStress other)
        {
            return new PolynomialStress(other.max_strain, -other.curve);
        }
        public static PolynomialStress operator+(PolynomialStress one, PolynomialStress two)
        {
            return new PolynomialStress(one.max_strain, one.curve+two.curve);
        }
        public static PolynomialStress operator-(PolynomialStress one, PolynomialStress two)
        {
            return new PolynomialStress(one.max_strain, one.curve-two.curve);
        }
        public static PolynomialStress operator*(double factor, PolynomialStress one)
        {
            return new PolynomialStress(one.max_strain, factor*one.curve);
        }
        public static PolynomialStress operator/(PolynomialStress one, double divisor)
        {
            return new PolynomialStress(one.max_strain, one.curve/divisor);
        }
        #endregion
    }
}
