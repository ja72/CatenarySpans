using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace JA.Engineering
{
    public interface ICable
    {
        double Area { get; }
        double Diameter { get; }
        double RatedStrength { get; }
        double Weight { get; }
    }

    public class Cable : ICable, ICloneable, IEquatable<Cable>
    {

        #region Factory
        public Cable(double dia, params (Alloy alloy, double area)[] layers)
        {
            this.Diameter=  dia;
            foreach (var (alloy, area) in layers)
            {
                this.Area += area;
                this.Weight += 12*alloy.Density*area;
                this.RatedStrength += area*alloy.YieldStrength;
            }
        }
        public Cable(double area, double dia, double wt, double rts)
        {
            this.Area = area;
            this.Diameter = dia;
            this.Weight = wt;
            this.RatedStrength = rts;
        }

        public Cable(ICable other)
            : this(other.Area, other.Diameter, other.Weight, other.RatedStrength)
        { }

        public Cable(Cable other)
            : this(other as ICable)
        { }
        #endregion

        #region Properties
        public double Area { get; }
        public double Weight { get; }
        public double Diameter { get; }
        public double RatedStrength { get; }
        #endregion

        #region ICloneable
        public Cable Clone() { return new Cable(this); }
        object ICloneable.Clone() { return Clone(); }
        #endregion

        public override bool Equals(object obj)
        {
            if (obj is Cable cable)
            {
                return Equals(cable);
            }
            return false;
        }
        public bool Equals(Cable other)
        {
            return Area.Equals(other.Area)
                && Diameter.Equals(other.Diameter)
                && Weight.Equals(other.Weight)
                && RatedStrength.Equals(other.RatedStrength);
        }
        public override int GetHashCode()
        {
            unchecked
            {
                int hc = 17*23;
                hc = 23*hc + Area.GetHashCode();
                hc = 23*hc + Diameter.GetHashCode();
                hc = 23*hc + Weight.GetHashCode();
                hc = 23*hc + RatedStrength.GetHashCode();
                return hc;
            }
        }
    }

}
