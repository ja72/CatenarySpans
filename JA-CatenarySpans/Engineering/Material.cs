using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Xml.Serialization;

namespace JA.Engineering
{
    /// <summary>
    /// Material properties
    /// </summary>
    public interface IMaterial
    {
        string Name { get;  }
        double Density { get; set; }        
        double Elasticity { get; set; }
        double YieldStress { get; set; }
        double UltimateStress { get; set; }
        double CTE { get; set; }
    }

    /// <summary>
    /// NONE,
    /// ALUMINUM,
    /// STEEL
    /// </summary>
    public enum MaterialSpec
    {
        NONE,
        ALUMINUM,
        STEEL
    }
    
    /// <summary>
    /// A class to hold material properties
    /// </summary>
    public class Material : IMaterial, ICloneable
    {
        double ρ;
        double α;
        double E;
        double σU;
        double σY;
        readonly string name;

        /// <summary>
        /// Define new material properties
        /// </summary>
        /// <param name="name">The material name or designation</param>
        /// <param name="ρ">Mass Density in (lbs/in^3)</param>
        /// <param name="E">Young's Modulus in (psi)</param>
        /// <param name="α">Coefficient of Thermal Expansion (1/°F)</param>
        /// <param name="σU">Ultimate stress (psi)</param>
        public Material(string name, double ρ, double E, double α, double σU, double σY)
        {
            this.name = name;
            this.E = E;
            this.ρ = ρ;
            this.α = α;
            this.σU = σU;
            this.σY = σY;
        }
        public Material(IMaterial other)
            : this(other.Name, other.Density, other.Elasticity, other.CTE, other.UltimateStress, other.YieldStress)
        { }
        public Material(MaterialSpec spec) : this(Standard(spec)) { }
        //public static readonly Material Aluminum = Standard(MaterialSpec.ALUMINUM);
        //public static readonly Material Steel = Standard(MaterialSpec.STEEL);
        public static Material Standard(MaterialSpec spec)
        {
            switch (spec)
            {
                case MaterialSpec.ALUMINUM:
                    return new Material("ALUMINUM", 0.0997, 10E6, 12.8e-6, 27000, 24000 );
                case MaterialSpec.STEEL:
                    return new Material("STEEL", 0.284, 30E6, 6.4e-6, 205000, 185000 );
            }
            throw new NotImplementedException();
        }
        [XmlAttribute]
        public string Name { get { return name; } }
        [XmlAttribute]
        public double Density { get { return ρ; } set { ρ = value; } }
        [XmlAttribute]
        public double Elasticity { get { return E; } set { E = value; } }
        [XmlAttribute]
        public double CTE { get { return α; } set { α = value; } }
        [XmlAttribute]
        public double UltimateStress { get { return σU; } set { σU = value; } }
        [XmlAttribute]
        public double YieldStress { get { return σY; } set { σY = value; } }

        #region ICloneable Members

        public Material Clone() { return new Material(this); }

        object ICloneable.Clone()
        {
            return Clone();
        }

        #endregion

    }
}
