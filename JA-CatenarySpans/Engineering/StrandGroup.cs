using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Xml.Serialization;

namespace JA.Engineering
{
    /// <summary>
    /// A strand group contains multiple strands in one or more layers
    /// and has weight and strength. 
    /// </summary>
    public interface IStrandGroup
    {
        StrandSpec Specification { get; }
        int LayerCount { get; }
        int StrandCount { get; }
        double StrandDiameter { get;}
        double TotalStrandArea { get; }
        double Derating { get; }
        double InnerDiameter { get; }
        bool IsCore { get; }
        double OuterDiameter { get; }
        double RatedStrength { get; }
        double TotalStrandWeight { get; }
        IMaterial Material { get; }
        IStress Initial { get;}
        double Final { get; }
        double RefTemperature { get; }
    }

    /// <summary>
    /// A strand group may have one or more layers of strands of
    /// the same material. Each strand has diameter and the
    /// strand group has an inner diameter it is resting upon.
    /// </summary>
    [ImmutableObject(true)]
    public class StrandGroup : IStrandGroup, ICloneable
    {
        StrandSpec spec;
        int layers;
        int strands;
        double strand_dia;
        double id;
        Material material;
        PolynomialStress initial;
        double final;
        double T_ref;

        #region Factory
        public StrandGroup(IStrandGroup other) 
        {
            this.spec = other.Specification;
            this.layers = other.LayerCount;
            this.strands = other.StrandCount;
            this.strand_dia = other.StrandDiameter;
            this.id = other.InnerDiameter;
            this.material = other.Material as Material;
            this.initial= other.Initial as PolynomialStress;
            this.final = other.Final;
            this.T_ref = other.RefTemperature;
        }

        public StrandGroup() 
        {
            this.spec = StrandSpec.NONE;
            this.layers = 0;
            this.strands = 0;
            this.strand_dia = 0;
            this.id = 0;
            this.material = new Material(MaterialSpec.ALUMINUM);
            this.initial=null;
            this.final = 0;
            this.T_ref = 70;
        }

        #endregion

        #region Properties
        [XmlAttribute()]
        public StrandSpec Specification { get { return spec; } set { spec = value; } }
        [XmlAttribute()]
        public int LayerCount { get { return layers; } set { layers = value; } }
        [XmlAttribute()]
        public int StrandCount { get { return strands; } set { strands = value; } }
        [XmlAttribute()]
        public double StrandDiameter { get { return strand_dia; } set { strand_dia = value; } }
        [XmlIgnore()]
        public bool IsCore { get { return id == 0; } }
        [XmlAttribute()]
        public double InnerDiameter { get { return id; } set { id = value; } }
        [XmlIgnore()]
        public double OuterDiameter { get { return id == 0 ? (2 * layers + 1) * strand_dia : id + 2 * layers * strand_dia; } }
        [XmlIgnore()]
        public double StrandArea { get { return Math.PI * strand_dia * strand_dia / 4; } }
        [XmlIgnore()]
        public double TotalStrandArea { get { return strands*StrandArea; } }
        [XmlIgnore()]
        public double TotalStrandWeight
        {
            get { return material.Density * 12 * TotalStrandArea; } // 12 = in/ft
        }
        [XmlIgnore()]
        public double RatedStrength { get { return initial.MaxStress * TotalStrandArea * Derating; } }
        /// <summary>
        /// Strand group strength derating based on the number of layers.
        /// </summary>
        [XmlIgnore()]
        public double Derating
        {
            get
            {
                switch (layers)
                {
                    case 1: return 0.96;
                    case 2: return 0.93;
                    case 3: return 0.91;
                    case 4: return 0.90;
                    case 5: return 0.89;
                    default:
                        throw new NotSupportedException("Cannot have more than 5 layers.");
                }
            }
        }
        IMaterial IStrandGroup.Material { get { return material; } }
        [XmlIgnore()]
        public Material Material { get { return material; } set { material = value; } }
        IStress IStrandGroup.Initial { get { return initial; } }
        public PolynomialStress Initial { get { return initial; } set { initial=value; } }
        [XmlElement()]
        public double Final { get { return final; } set { final = value; } }
        [XmlElement()]
        public double RefTemperature { get { return T_ref; } set { T_ref = value; } }
        [XmlElement()]
        public double CTE { get { return material.CTE*100; } set { material.CTE=value/100; } }
        #endregion

        #region ICloneable Members

        public StrandGroup Clone() { return new StrandGroup(this); }

        object ICloneable.Clone()
        {
            return Clone();
        }

        #endregion

    }

    /// <summary>
    /// A stressed strand group with strain hardening and pre-stress.
    /// </summary>
    public class StressGroup : StrandGroup
    {
        double common_tension, pre_strain;
        public StressGroup(IStrandGroup group) : this(group, 0, 0) { }
        public StressGroup(IStrandGroup group, double pre_strain) : this(group, 0, pre_strain) { }
        public StressGroup(IStrandGroup group, double tension, double pre_strain) : base(group)   
        {
            this.common_tension=tension;
            this.pre_strain=pre_strain;
        }
        public double PreStrain { get { return pre_strain; } set { pre_strain=value; } }
        //public double FreeLengthAtRef { get { return free_length; } set { free_length=value; } }
        public double CommonTension { get { return common_tension; } set { common_tension=value; } }
        public double GetTension(double strain_pct, double temperature, Condition condition)
        {
            return GetTension(strain_pct, temperature, this.pre_strain, condition);
        }
        public double GetTension(double strain_pct, double temperature, double pre_strain, Condition condition = Condition.Initial)
        {
            double ε = strain_pct - 100 * Material.CTE * (temperature - RefTemperature) - pre_strain;
            double T = Initial.StressOf(ε) * TotalStrandArea;
            if (condition == Condition.Final || condition == Condition.Creep)
            {
                if (T < common_tension)
                {
                    double TY = common_tension;
                    T = TY + TotalStrandArea * Final * (ε - Initial.StrainOf(TY / TotalStrandArea));
                }
            }
            return Math.Max(T, 0);
        }
        public void SetTension(double tension)
        {
            if (tension>common_tension)
            {
                this.common_tension=tension;
            }
        }
        public void SetTension(double strain_pct, double temperature)
        {
            double ε=strain_pct-100*Material.CTE*(temperature-RefTemperature)  -pre_strain;
            double T=Initial.StressOf(ε)*TotalStrandArea;
            if (T>common_tension)
            {
                this.common_tension=T;
            }
        }
    }

}
