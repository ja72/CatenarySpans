using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Xml.Serialization;

namespace JA.Engineering
{
    /// <summary>
    /// NONE,
    /// ALUMINUM,
    /// STEEL
    /// </summary>
    public enum MaterialSpec
    {
        NONE,
        ALUMINUM,
        STEEL,
        COPPER,
        AW,
    }

    /// <summary>
    /// Material properties
    /// </summary>
    public interface IMaterial
    {
        MaterialSpec Spec { get;  }
        double Density { get; }        
        double Elasticity { get; }
        double ThermalCoefficient { get; }
    }

    public interface IAlloy : IMaterial
    {
        string Name { get; }
        double YieldStress { get; }
        double UltimateStress { get;  }
    }

    /// <summary>
    /// Material library for wires
    /// </summary>
    public static class Materials
    {
        // Data taken from:
        // https://www.scribd.com/document/411581152/Southwire-Overhead-Conductor

        static readonly Material Aluminum = new Material(MaterialSpec.ALUMINUM,
            density: 0.0997,
            youngsModulus: 10E6,
            thermalCoef: 12.8e-6);
        static readonly Material Steel = new Material(MaterialSpec.STEEL,
            density: 0.2811,
            youngsModulus: 30E6,
            thermalCoef: 6.4e-6);
        static readonly Material AluminumCladSteel = new Material(MaterialSpec.AW,
            density: 0.2381,
            youngsModulus: 29E6,
            thermalCoef: 7.2e-6);
        static readonly Material Copper = new Material(MaterialSpec.COPPER,
            density: 0.3212,
            youngsModulus: 17e6,
            thermalCoef: 9.4e-6);

        public static Material Standard(MaterialSpec spec)
        {
            switch (spec)
            {
                case MaterialSpec.ALUMINUM:
                    return Aluminum;
                case MaterialSpec.STEEL:
                    return Steel;
                case MaterialSpec.COPPER:
                    return Copper;
                case MaterialSpec.AW:
                    return AluminumCladSteel;
            }
            throw new NotSupportedException();
        }

        public static readonly Alloy AnnealedCopper = new Alloy(MaterialSpec.COPPER, 
            "IACS",
            ultimateStrength: 39500,
            yieldStrength: 35000);
        public static readonly Alloy HardDrawnCopper = new Alloy(MaterialSpec.COPPER,
            "HDCU",
            ultimateStrength: 72500,
            yieldStrength: 65000);
        public static readonly Alloy AL1650O = new Alloy(MaterialSpec.ALUMINUM,
            "AL 1350-O",
            ultimateStrength: 11500,
            yieldStrength: 10000);
        public static readonly Alloy AL1650H19 = new Alloy(MaterialSpec.ALUMINUM,
            "AL 1350-H19",
            ultimateStrength: 27000,
            yieldStrength: 24000);
        public static readonly Alloy AL6201T81 = new Alloy(MaterialSpec.ALUMINUM,
            "AL 6201-T81",
            ultimateStrength: 48500,
            yieldStrength: 46000);
        public static readonly Alloy SteelCore = new Alloy(MaterialSpec.STEEL,
            "ST CORE",
            ultimateStrength: 210000,
            yieldStrength: 185000);
        public static readonly Alloy AWCore = new Alloy(MaterialSpec.AW,
            "AW CORE",
            ultimateStrength: 195000,
            yieldStrength: 175000);
    }


    /// <summary>
    /// A class to hold basic material properties
    /// </summary>
    public class Material : IMaterial, ICloneable
    {

        /// <summary>
        /// Define new material properties
        /// </summary>
        /// <param name="spec">The material name or designation</param>
        /// <param name="density">Mass Density in (lbs/in^3)</param>
        /// <param name="youngsModulus">Young's Modulus in (psi)</param>
        /// <param name="thermalCoef">Coefficient of Thermal Expansion (1/°F)</param>
        /// <param name="ultimateStrength">Ultimate strength (psi)</param>
        /// <param name="yieldStrength">Tensile strength (psi)</param>
        public Material(MaterialSpec spec, double density, double youngsModulus, double thermalCoef)
        {
            this.Spec = spec;
            this.Elasticity = youngsModulus;
            this.Density = density;
            this.ThermalCoefficient = thermalCoef;
        }
        public Material(IMaterial other)
            : this(other.Spec, other.Density, other.Elasticity, other.ThermalCoefficient)
        { }
        [XmlAttribute]
        public MaterialSpec Spec { get; }
        [XmlAttribute]
        public double Density { get; set; }
        [XmlAttribute]
        public double Elasticity { get; set; }
        [XmlAttribute]
        public double ThermalCoefficient { get; set; }

        #region ICloneable Members

        public Material Clone() { return new Material(this); }

        object ICloneable.Clone()
        {
            return Clone();
        }

        #endregion

    }

    /// <summary>
    /// A class to hold specific alloy properties
    /// </summary>
    public class Alloy : Material
    {
        public Alloy(MaterialSpec spec, string name, double ultimateStrength, double yieldStrength)
            : this(Materials.Standard(spec), name, ultimateStrength, yieldStrength)
        { }
        public Alloy(IMaterial material, string name, double ultimateStrength, double yieldStrength)
            : base(material)
        {
            this.Name = name;
            this.UltimateStregth = ultimateStrength;
            this.YieldStrength = yieldStrength;
        }
        public Alloy(IAlloy other)
            : this(other, other.Name, other.UltimateStress, other.YieldStress)
        { }
        [XmlAttribute]
        public string Name { get; }
        [XmlAttribute]
        public double UltimateStregth { get; set; }
        [XmlAttribute]
        public double YieldStrength { get; set; }
    }
}
