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

    public class Cable : ICable, ICloneable
    {

        #region Factory
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
            : this(other.Area, other.Diameter, other.Weight, other.RatedStrength)
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

    }

}
