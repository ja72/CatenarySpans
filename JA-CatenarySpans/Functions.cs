using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JA
{
    public interface IUnaryFunction
    {
        Func<double, double> Function { get; }
        IUnaryFunction Derivative();
    }

    public static class FunctionsEx
    {

        public static Polynomial Poly(this double[] coefficients) { return coefficients; }

    }

    #region Polynomials
    public class Polynomial : IUnaryFunction, IEquatable<Polynomial>, ICloneable
    {
        //readonly double[] Coefficients;

        #region Factory
        public Polynomial(params double[] coefficients)
        {
            this.Coefficients=coefficients;
        }
        public Polynomial(Polynomial other)
        {
            this.Coefficients=new double[other.Coefficients.Length];
            Array.Copy(other.Coefficients, Coefficients, Coefficients.Length);
        }
        public static readonly Polynomial Empty=new Polynomial();
        public static implicit operator Polynomial(double constant)
        {
            return new Polynomial(constant);
        }

        public static implicit operator Polynomial(double[] coefficients)
        {
            return new Polynomial(coefficients);
        }
        public static implicit operator double[](Polynomial poly) { return poly.Coefficients; }

        public int Order { get { return Coefficients.Length - 1; } }
        public double[] Coefficients { get; }
        #endregion

        #region Methods
        /// <summary>
        /// Evaluate a polynomial using Homer's method
        /// </summary>
        public double Evaluate(double x)
        {
            double y=0;
            for (int i=Coefficients.Length-1; i>=0; i--)
            {
                y=x*y+Coefficients[i];
            }
            return y;
        }

        public Func<double, double> Function
        {
            get { return Evaluate; }
        }
        IUnaryFunction IUnaryFunction.Derivative()
        {
            return Derivative();
        }
        public Polynomial Derivative()
        {
            if (Coefficients.Length>0)
            {
                double[] derivative=new double[Coefficients.Length-1];
                for (int i=0; i<derivative.Length; i++)
                {
                    derivative[i]=(i+1)*Coefficients[i+1];
                }
                return new Polynomial(derivative);
            }
            return Empty;
        }
        public Polynomial Integrate(double constant)
        {
            double[] integral=new double[Coefficients.Length+1];
            integral[0]=constant;
            for (int i=1; i<integral.Length; i++)
            {
                integral[i]=Coefficients[i-1]/i;
            }
            return new Polynomial(integral);
        }

        public bool FindRoot(double y_target, out double x)
        {
            if (Coefficients.Length>1)
            {
                x=(y_target-Coefficients[0])/Coefficients[1];
                if (Coefficients.Length>2)
                {
                    return Function.Bisection(y_target, x, 1e-6, out x);
                }
                return true;
            }
            x=0;
            return false;
        }


        #endregion

        #region Algebra
        public Polynomial Add(Polynomial other, double factorB)
        {
            return Coefficients.VectorAddition(other.Coefficients, factorB);
        }
        public Polynomial Scale(double factor)
        {
            return Coefficients.VectorScale(factor);
        }
        public double Dot(Polynomial other) { return Coefficients.InnerProduct(other.Coefficients); }

        #region Operators
        public static Polynomial operator+(Polynomial lhs, Polynomial rhs) { return lhs.Add(rhs, 1); }
        public static Polynomial operator-(Polynomial rhs) { return rhs.Scale(-1); }
        public static Polynomial operator-(Polynomial lhs, Polynomial rhs) { return lhs.Add(rhs, -1); }
        public static Polynomial operator*(double lhs, Polynomial rhs) { return rhs.Scale(lhs); }
        public static Polynomial operator*(Polynomial lhs, double rhs) { return lhs.Scale(rhs); }
        public static Polynomial operator/(Polynomial lhs, double rhs) { return lhs.Scale(1/rhs); }
        public static double operator*(Polynomial lhs, Polynomial rhs)
        {
            return lhs.Dot(rhs);
        }
        #endregion


        #endregion

        #region ICloneable Members

        public Polynomial Clone() { return new Polynomial(this); }

        object ICloneable.Clone()
        {
            return Clone();
        }

        #endregion

        #region IEquatable Members

        /// <summary>
        /// Equality overrides from <see cref="System.Object"/>
        /// </summary>
        /// <param name="obj">The object to compare this with</param>
        /// <returns>False if object is a different type, otherwise it calls <code>Equals(Polynomial)</code></returns>
        public override bool Equals(object obj)
        {
            if (obj is Polynomial)
            {
                return Equals((Polynomial)obj);
            }
            return false;
        }

        /// <summary>
        /// Checks for equality among <see cref="Polynomial"/> classes
        /// </summary>
        /// <param name="other">The other <see cref="Polynomial"/> to compare it to</param>
        /// <returns>True if equal</returns>
        public bool Equals(Polynomial other)
        {
            return Coefficients.VectorAreEqual(other.Coefficients, 1e-15);
        }

        /// <summary>
        /// Calculates the hash code for the <see cref="Polynomial"/>
        /// </summary>
        /// <returns>The int hash value</returns>
        public override int GetHashCode()
        {           
            return Coefficients.CalcHashCode();
        }

        #endregion

    }
    #endregion
}
