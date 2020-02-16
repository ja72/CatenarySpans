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
            this.Name = name;
            this.Elasticity = E;
            this.Density = ρ;
            this.CTE = α;
            this.UltimateStress = σU;
            this.YieldStress = σY;
        }
        public Material(IMaterial other)
            : this(other.Name, other.Density, other.Elasticity, other.CTE, other.UltimateStress, other.YieldStress)
        { }
        public Material(MaterialSpec spec) : this(Standard(spec)) { }
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
        public string Name { get; }
        [XmlAttribute]
        public double Density { get; set; }
        [XmlAttribute]
        public double Elasticity { get; set; }
        [XmlAttribute]
        public double CTE { get; set; }
        [XmlAttribute]
        public double UltimateStress { get; set; }
        [XmlAttribute]
        public double YieldStress { get; set; }

        #region ICloneable Members

        public Material Clone() { return new Material(this); }

        object ICloneable.Clone()
        {
            return Clone();
        }

        #endregion

    }
}
