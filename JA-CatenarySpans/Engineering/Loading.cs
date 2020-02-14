using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Diagnostics;

namespace JA.Engineering
{
    /// <summary>
    /// Initial,
    /// Final,
    /// Creep
    /// </summary>
    public enum Condition
    {
        Initial,
        Final,
        Creep
    }
    /// <summary>
    /// Initial,
    /// Final,
    /// Creep
    /// </summary>
    public enum NescConstant
    {
        None,
        Old,
        New
    }
    /// <summary>
    /// Heavy,
    /// Medium,
    /// Light
    /// </summary>
    public enum NescLoading
    {
        Heavy,
        Medium,
        Light
    }  

    [DebuggerDisplay("{temperature} {wind_pressure} {ice_thickness} {nesc_K}")]
    public class LoadingCondition : ICloneable, IComparable<LoadingCondition>, IEquatable<LoadingCondition>
    {
        double temperature, nesc_K;
        double wind_pressure, ice_thickness;

        public LoadingCondition(double temp)
            : this(temp, 0, 0, 0)
        {
        }        
        public LoadingCondition(NescLoading loading, NescConstant K)
        {            

            switch (loading)
            {
                case NescLoading.Heavy:
                    this.temperature=0;
                    this.ice_thickness=0.5;
                    this.wind_pressure=4;
                    // ignore old copper constant
                    this.nesc_K=K==NescConstant.New?0.3:K==NescConstant.Old?0.31:0;
                    break;
                case NescLoading.Medium:
                    this.temperature=15;
                    this.ice_thickness=0.25;
                    this.wind_pressure=4;
                    // ignore old copper constant
                    this.nesc_K=K==NescConstant.New?0.2:K==NescConstant.Old?0.32:0;
                    break;
                case NescLoading.Light:
                    this.temperature=30;
                    this.ice_thickness=0.0;
                    this.wind_pressure=9;
                    this.nesc_K=0.05;
                    break;
                default:
                    throw new ArgumentException("Unknown Loading Limit.");
            }

        }
        public static implicit operator LoadingCondition(NescLoading loading) { return new LoadingCondition(loading, NescConstant.New); }
        public LoadingCondition(double temp, double wind, double ice, double nesc_K)
        {
            this.temperature = temp;
            this.ice_thickness = ice;
            this.wind_pressure = wind;
            this.nesc_K = nesc_K;
        }
        public LoadingCondition(LoadingCondition other)
            : this(other.temperature, other.wind_pressure, other.ice_thickness, other.nesc_K)
        { }

        public static implicit operator LoadingCondition(double temperature) { return new LoadingCondition(temperature); }

        public static readonly LoadingCondition Default = new LoadingCondition(60);

        public static LoadingCondition[] NESC_Heavy=new LoadingCondition[] {
            new DesignCondition(NescLoading.Heavy, Condition.Initial, 0.6),
            new LoadingCondition(32, 0, 0.5, 0.0), 
            -20,
            0,
            new DesignCondition(60, Condition.Initial, 0.35),
            new DesignCondition(60, Condition.Final, 0.25)
        };
        public static LoadingCondition[] NESC_Medium=new LoadingCondition[] {
            new DesignCondition(NescLoading.Medium, Condition.Initial, 0.6),
            new LoadingCondition(32, 0, 0.25, 0.0), 
            0,
            15,
            new DesignCondition(60, Condition.Initial, 0.35),
            new DesignCondition(60, Condition.Final, 0.25)
        };
        public static LoadingCondition[] NESC_Light=new LoadingCondition[] {
            new DesignCondition(NescLoading.Light, Condition.Final, 0.6),
            new DesignCondition(60, Condition.Initial, 0.35),
            new DesignCondition(60, Condition.Final, 0.25)
        };
        public static LoadingCondition[] StandardTemperatures=new LoadingCondition[] { 30, 60, 90, 120, 167, 212 };

        public double Temperature { get { return temperature; } set { temperature = value; } }
        public double IceThickness { get { return ice_thickness; } set { ice_thickness = value; } }
        public double WindPressure { get { return wind_pressure; } set { wind_pressure = value; } }
        public double NESC_K { get { return nesc_K; } set { nesc_K = value; } }

        #region ICloneable Members

        public LoadingCondition Clone() { return new LoadingCondition(this); }

        object ICloneable.Clone()
        {
            return Clone();
        }

        #endregion

        #region Equality
        // override object.Equals
        public override bool Equals(object obj)
        {
            if (obj is LoadingCondition l)
            {
                return Equals(l);
            }
            return false;
        }
        public bool Equals(LoadingCondition other)
        {
            return temperature == other.temperature
                && ice_thickness == other.ice_thickness
                && wind_pressure == other.wind_pressure
                && nesc_K == other.nesc_K;
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            return temperature.GetHashCode()
                ^ ice_thickness.GetHashCode()
                ^ wind_pressure.GetHashCode()
                ^ nesc_K.GetHashCode();
        }
        public int CompareTo(LoadingCondition other)
        {
            return temperature.CompareTo(other.temperature);
        }

        #endregion
        #region Calculations
        public double GetWeight(ICable cable)
        {
            return Math.Sqrt(
                Math.Pow(cable.Weight + TubeIceWeight(cable.Diameter, ice_thickness), 2)
                + Math.Pow(WindWeight(cable.Diameter, ice_thickness, wind_pressure), 2))
                + NESC_K;
        }

        public static double TubeIceWeight(double cable_dia, double th_ice)
        {
            const double density_ice = 57;
            return density_ice * (Math.PI / 4.0) / Math.Pow(12.0, 2) * (Math.Pow((cable_dia + 2.0 * th_ice), 2) - Math.Pow(cable_dia, 2));
        }
        public static double WindWeight(double cable_dia, double th_ice, double wind_pressure)
        {
            return wind_pressure * (cable_dia + 2 * th_ice) / 12.0;
        }
        public Catenary GetCatenary(ICable cable, ISpan span, double limit)
        {
            double wt = GetWeight(cable);

            if (limit < 0)
            {
                return new Catenary(span, wt) { MaximumSag = -limit };
            }
            else if (limit < 1 && limit > 0)
            {
                return new Catenary(span, wt) { AverageTension = limit * cable.RatedStrength };
            }
            else if (limit > 0)
            {
                return new Catenary(span, wt) { AverageTension = limit };
            }
            throw new ArgumentException("Invalid Limit");
        }


        #endregion
    }

    [DebuggerDisplay("{temperature} {wind_pressure} {ice_thickness} {nesc_K} {condition} {limit}")]
    public class DesignCondition : LoadingCondition, ICloneable, IEquatable<DesignCondition>
    {
        Condition condition;
        double limit;

        public DesignCondition(LoadingCondition loading, Condition condition, double tension)
            : base(loading)
        {
            this.condition = condition;
            this.limit = tension;
        }

        public DesignCondition(DesignCondition other)
            : base(other)
        {
            this.condition = other.condition;
            this.limit = other.limit;
        }

        public Condition Condition { get { return condition; } set { condition = value; } }
        public double Limit { get { return limit; } set { limit = value; } }
        
        public Catenary GetCatenary(ICable cable, ISpan span)
        {
            return GetCatenary(cable, span, limit);
        }
        public Catenary GetCatenary(LoadedCable loaded_cable)
        {
            return GetCatenary(loaded_cable.Cable, loaded_cable.Span, limit);
        }

        #region ICloneable Members

        public new DesignCondition Clone() { return new DesignCondition(this); }

        object ICloneable.Clone()
        {
            return Clone();
        }

        #endregion

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
        public bool Equals(DesignCondition other)
        {
            return base.Equals(other)
                && condition == other.condition
                && limit == other.limit;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode()
                ^ condition.GetHashCode()
                ^ limit.GetHashCode();
        }
    }

}
