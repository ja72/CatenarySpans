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
        readonly double area, dia, wt, rts;

        #region Factory
        public Cable(double area, double dia, double wt, double rts)
        {
            this.area = area;
            this.dia = dia;
            this.wt = wt;
            this.rts = rts;
        }

        public Cable(ICable other)
            : this(other.Area, other.Diameter, other.Weight, other.RatedStrength)
        {        } 

        public Cable(Cable other)
            : this(other.area, other.dia, other.wt, other.rts)
        {         }
        #endregion

        #region Properties
        public double Area { get { return area; } }
        public double Weight { get { return wt; } }
        public double Diameter { get { return dia; } }
        public double RatedStrength { get { return rts; } }

        #endregion

        #region ICloneable
        public Cable Clone() { return new Cable(this); }
        object ICloneable.Clone() { return Clone(); }
        #endregion

    }

}
