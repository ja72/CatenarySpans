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
    /// AL1350_O,
    /// AL1350_H19,
    /// AL6201_T81,
    /// STEEL,
    /// HS237,
    /// HS285,
    /// AW,
    /// </summary>
    public enum StrandSpec
    {
        NONE,
        AL1350_O,
        AL1350_H19,
        AL6201_T81,
        STEEL,
        HS237,
        HS285,
        AW,
    }
    /// <summary>
    /// NONE,
    /// AAC,
    /// AAAC,
    /// ACSR,
    /// AACSR,
    /// ACSR_AW,
    /// AACSR_AW,
    /// ACSS,
    /// ACSS_AW,
    /// ACSS_HS,
    /// ACSS_US,
    /// ACAR,
    /// STEEL,
    /// AW
    /// </summary>
    public enum CableSpec
    {
        NONE,
        AAC,
        AAAC,
        ACSR,
        AACSR,
        ACSR_AW,
        AACSR_AW,
        ACSS,
        ACSS_AW,
        ACSS_HS,
        ACSS_US,
        ACAR,
        STEEL,
        AW
    }

    /// <summary>
    /// Contains a collection of <see cref="IStrandGroup"/>
    /// </summary>
    public interface IStrandedCable : ICable
    {
        IStrandGroup[] Strands { get; }
    }

    /// <summary>
    /// Basic stranded cable definnition. The strands are orginized into
    /// two (or more) groups by material. The first group is the "core"
    /// 
    /// </summary>
    public sealed class StrandedCable : IStrandedCable, ICloneable
    {
        CableSpec spec;
        List<StrandGroup> strands;

        #region Factory

        public StrandedCable() : this(CableSpec.STEEL, new StrandGroup()) { }

        public StrandedCable(CableSpec spec, params StrandGroup[] groups)
        {
            this.spec = spec;
            this.strands = new List<StrandGroup>(groups);
        }

        public StrandedCable(StrandedCable other)
        {
            this.spec = other.spec;
            this.strands = new List<StrandGroup>(other.strands);
        }
        public static StrandedCable ComposeCable(CableSpec spec, int strands, double strand_dia)
        {
            StrandSpec core_spec = GetCoreSpecFor(spec);
            StrandSpec outer_spec = GetOuterSpecFor(spec);

            if (core_spec != StrandSpec.NONE)
            {
                StrandGroup core = BuildCoreGroup(spec, strands, strand_dia);
                return new StrandedCable(spec, core);
            }
            throw new NotSupportedException();
        }

        public static StrandedCable ComposeCable(CableSpec spec, int core_strands, double core_strand_dia, int outer_strands, double outer_strand_dia)
        {
            StrandGroup core = BuildCoreGroup(spec, core_strands, core_strand_dia);
            StrandGroup outer = BuildOuterGroup(spec, core.OuterDiameter, outer_strands, outer_strand_dia);

            return new StrandedCable(spec, core, outer);
        }
        public static readonly StrandedCable Drake = StrandedCable.ComposeCable(CableSpec.ACSR, 7, 0.136, 26, 0.1749);
        #endregion

        #region Helpers
        static Material GetMaterialFor(StrandSpec spec)
        {
            switch (spec)
            {
                case StrandSpec.AL1350_O: return new Material(MaterialSpec.ALUMINUM) { YieldStress = 8000, UltimateStress = 12000 };
                case StrandSpec.AL1350_H19: return new Material(MaterialSpec.ALUMINUM) { YieldStress = 24000, UltimateStress = 26000 };
                case StrandSpec.AL6201_T81: return new Material(MaterialSpec.ALUMINUM) { YieldStress = 45000, UltimateStress = 48000 };
                case StrandSpec.STEEL: return new Material(MaterialSpec.STEEL) { YieldStress = 185000, UltimateStress = 205000 };
                case StrandSpec.HS237: return new Material(MaterialSpec.STEEL) { YieldStress = 215000, UltimateStress = 237000 };
                case StrandSpec.HS285: return new Material(MaterialSpec.STEEL) { YieldStress = 265000, UltimateStress = 285000 };
                case StrandSpec.AW: return new Material(MaterialSpec.STEEL) { YieldStress = 175000, UltimateStress = 195000 };
                default:
                    throw new NotSupportedException();
            }
        }

        static StrandSpec GetCoreSpecFor(CableSpec spec)
        {
            switch (spec)
            {
                case CableSpec.ACSR:
                case CableSpec.AACSR:
                case CableSpec.STEEL:
                case CableSpec.ACSS:
                    return StrandSpec.STEEL;
                case CableSpec.ACSS_HS:
                    return StrandSpec.HS237;
                case CableSpec.ACSS_US:
                    return StrandSpec.HS285;
                case CableSpec.AAC:
                    return StrandSpec.AL1350_H19;
                case CableSpec.AAAC:
                case CableSpec.ACAR:
                    return StrandSpec.AL6201_T81;
                case CableSpec.ACSR_AW:
                case CableSpec.AACSR_AW:
                case CableSpec.ACSS_AW:
                case CableSpec.AW:
                    return StrandSpec.AW;
                default:
                    return StrandSpec.NONE;
            }
        }

        static StrandSpec GetOuterSpecFor(CableSpec spec)
        {
            switch (spec)
            {
                case CableSpec.ACSR:
                case CableSpec.ACSR_AW:
                    return StrandSpec.AL1350_H19;
                case CableSpec.AACSR:
                case CableSpec.AACSR_AW:
                    return StrandSpec.AL6201_T81;
                case CableSpec.ACSS:
                case CableSpec.ACSS_AW:
                case CableSpec.ACSS_HS:
                case CableSpec.ACSS_US:
                    return StrandSpec.AL1350_O;
                default:
                    return StrandSpec.NONE;
            }
        }

        static StrandGroup BuildCoreGroup(CableSpec spec, int strands, double strand_dia)
        {
            StrandSpec core_spec = GetCoreSpecFor(spec);
            StrandSpec outer_spec = GetOuterSpecFor(spec);

            if (core_spec == StrandSpec.NONE || strands == 0 || !strand_dia.IsPositive())
            {
                return null;
            }
            int core_layers = (strands - 1) / 6;
            double core_dia = strand_dia * (1 + 2 * core_layers);

            Material mat = GetMaterialFor(core_spec);

            double core_sf = core_spec == StrandSpec.AW ?
                1.377 - 2.936 * strand_dia :
                    outer_spec != StrandSpec.AL1350_O ?
                    1.1277 - 1.096 * strand_dia :
                        strand_dia < 0.1 ?
                        1.048 - 0.375 * strand_dia :
                        1.1277 - 1.096 * strand_dia;

            double core_str = core_sf * (outer_spec == StrandSpec.AL1350_O ?
                                            mat.UltimateStress :
                                            mat.YieldStress);

            return new StrandGroup()
            {
                Specification = core_spec,
                StrandDiameter = strand_dia,
                LayerCount = core_layers,
                StrandCount = strands,
                Material = mat,
                Initial = null,
                Final = 0,
            };
        }

        static StrandGroup BuildOuterGroup(CableSpec spec, double core_dia, int strands, double strand_dia)
        {
            StrandSpec core_spec = GetCoreSpecFor(spec);
            StrandSpec outer_spec = GetOuterSpecFor(spec);

            if (outer_spec == StrandSpec.NONE || strands == 0 || !strand_dia.IsPositive())
            {
                return null;
            }

            int outer_layer_count = (int)Math.Floor(Math.PI / Math.Asin(strand_dia / (core_dia + strand_dia)));
            int outer_layers = 1 - (int)(Math.Log((outer_layer_count + 6.0) / (strands + 6.0), 2));

            Material mat = GetMaterialFor(outer_spec);

            double outer_sf = strand_dia < 0.14 ?
                1.213 - 2.185 * strand_dia :
                0.972 - 0.402 * strand_dia;

            double outer_str = outer_sf * mat.UltimateStress;

            return new StrandGroup()
            {
                Specification = outer_spec,
                StrandDiameter = strand_dia,
                InnerDiameter = core_dia,
                LayerCount = outer_layers,
                StrandCount = strands,
                Material = mat,
                Initial = null,
                Final = 0
            };

            //return new StrandGroup(outer_spec, core_dia, strand_dia, outer_layers, strands, mat.Density, outer_str, 0.744*mat.Elasticity);
        }

        #endregion

        #region Properties
        [XmlArray("Strands"), XmlArrayItem("Group"), Browsable(true), DisplayName("Strands")]
        public StrandGroup[] StrandArray
        {
            get { return strands.ToArray(); }
            set
            {
                strands = new List<StrandGroup>(value);
            }
        }
        IStrandGroup[] IStrandedCable.Strands { get { return strands.ToArray(); } }
        [XmlIgnore(), Browsable(false)]
        public StrandGroup this[int index] { get { return strands[index]; } }
        [XmlIgnore(), Browsable(false)]
        List<StrandGroup> StrandList { get { return strands; } }
        [XmlAttribute()]
        public double Diameter { get { return strands.Last().OuterDiameter.RoundTo(4); } }
        [XmlAttribute()]
        public double Area { get { return strands.Sum((g) => g.TotalStrandArea).RoundTo(4); } }
        [XmlAttribute()]
        public double Weight { get { return strands.Sum((g) => g.TotalStrandWeight).RoundTo(4); } }
        [XmlAttribute()]
        public double RatedStrength
        {
            get
            {
                return (2 * strands.Sum((g) => g.RatedStrength)).CeilingTo(2) / 2;
            }
        }
        public void AssignChart(AlcoaChart chart)
        {
            if (!chart.IsOK)
            {
                throw new ArgumentException();
            }
            if (chart.Core.IsOK && strands.Count >= 1)
            {
                var core_fraction = strands[0].TotalStrandArea / Area;
                strands[0].RefTemperature = chart.RefTemperature;
                strands[0].Initial = new PolynomialStress(0.75, chart.Core.Initial / core_fraction);
                strands[0].Final = chart.Core.Final / core_fraction;
                strands[0].CTE = chart.Core.CTE;
            }
            if (chart.Outer.IsOK && strands.Count >= 2)
            {
                var outer_fraction = strands[1].TotalStrandArea / Area;
                strands[1].RefTemperature = chart.RefTemperature;
                strands[1].Initial = new PolynomialStress(0.75, chart.Outer.Initial / outer_fraction);
                strands[1].Final = chart.Outer.Final / outer_fraction;
                strands[1].CTE = chart.Outer.CTE;
            }
        }
        public bool HasChart => strands.All((st) => st.Initial != null && st.Final > 0);
        #endregion

        #region ICloneable Members

        public StrandedCable Clone() { return new StrandedCable(this); }

        object ICloneable.Clone()
        {
            return Clone();
        }

        #endregion

        #region File I/O

        public void SaveCable(string path)
        {
            XmlSerializer xs = new XmlSerializer(typeof(StrandedCable));
            var fs = System.IO.File.Open(path, System.IO.FileMode.Create, System.IO.FileAccess.Write);
            xs.Serialize(fs, this);
            fs.Close();
        }

        public static StrandedCable OpenCable(string path)
        {
            XmlSerializer xs = new XmlSerializer(typeof(StrandedCable));
            var fs = System.IO.File.Open(path, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            var rs = xs.Deserialize(fs) as StrandedCable;
            fs.Close();
            return rs;
        }

        #endregion
    }

    public class LoadedCable
    {
        Span span;
        ICable cable;
        List<StressGroup> stress;

        public LoadedCable(StrandedCable cable, Span span) : this(cable, span, cable.StrandArray) { }
        public LoadedCable(ICable cable, Span span, params IStrandGroup[] strands)
        //: base(cable)
        {
            this.cable = cable;
            this.span = span;
            this.stress = new List<StressGroup>();
            for (int i = 0; i < strands.Length; i++)
            {
                stress.Add(new StressGroup(strands[i]));
            }
        }
        public IList<StressGroup> Stress { get { return stress; } }
        public Span Span { get { return span; } }
        public ICable Cable { get { return cable; } }
        public double CommonTension { get { return stress.Sum((s) => s.CommonTension); } }
        public double PreStrain { get { return stress.Min((s) => s.PreStrain); } }
        public double GetTension(double strain_pct, double temperature, Condition condition)
        {
            double T_ = 0;
            for (int i = 0; i < stress.Count; i++)
            {
                T_ += stress[i].GetTension(strain_pct, temperature, condition);
            }
            return T_;
        }
        public Catenary GetCatenaryWithTension(double tension)
        {
            return new Catenary(span, cable.Weight) { AverageTension = tension };
        }
        public double GetLengthAtLoading(LoadingCondition loading)
        {
            double wt = loading.GetWeight(this.cable);
            Condition condition = Condition.Final;
            if (loading is DesignCondition design)
            {
                condition = design.Condition;
            }
            Func<double, double> P = (L_) =>
              {
                  double strain_pct = 100 * (L_ / span.SpanLength - 1);
                  double T_ = GetTension(strain_pct, loading.Temperature, condition);
                  double H_ = CatenaryCalculator.SetTotalLength(span.Step, wt, L_, 1e-5);
                  double P_ = CatenaryCalculator.AverageTension(span.Step, wt, H_);
                  return T_ - P_;
              };

            if (P.Bisection(0, span.SpanLength, 2 * span.SpanLength, 1e-5, out double L))
            {
                return L;
            }
            throw new ArgumentException(nameof(loading));
        }
        public double[] GetComponentTensions(double strain_pct, double temperature, Condition condition)
        {
            double[] T = new double[stress.Count];
            for (int i = 0; i < stress.Count; i++)
            {
                T[i] = stress[i].GetTension(strain_pct, temperature, condition);
            }
            return T;
        }
        public Catenary GetCatenaryAtLoading(LoadingCondition loading)
        {
            double wt = loading.GetWeight(cable);
            double L = GetLengthAtLoading(loading);
            double H = CatenaryCalculator.SetTotalLength(span.Step, wt, L, 1e-5);
            return new Catenary(span, wt, H);
        }
        public double FindPreStrain(DesignCondition design)
        {
            return FindPreStrain(design.GetCatenary(this), design.Temperature);
        }
        public double FindPreStrain(Catenary cat, double temperature)
        {
            Func<double, double> P = (R_) =>
              {
                  double T_ = 0;
                  for (int i = 0; i < stress.Count; i++)
                  {
                      T_ += stress[i].GetTension(cat.GeometricStrainPct, temperature, R_, Condition.Initial);
                  }
                  return T_;
              };
            if (P.Bisection(cat.AverageTension, 0, 1e-6, out double R))
            {
                return R;
            }
            return PreStrain;
        }

        public Catenary LoadWith(DesignCondition loading)
        {
            Catenary cat = loading.GetCatenary(cable, span);
            for (int i = 0; i < stress.Count; i++)
            {
                stress[i].SetTension(cat.GeometricStrainPct, loading.Temperature);
            }
            return cat;
        }
        public void SetPreStrain(double pre_strain)
        {
            for (int i = 0; i < stress.Count; i++)
            {
                stress[i].PreStrain = pre_strain;
            }
        }
        public Catenary SetDesignLoad(double H_load)
        {
            Catenary cat = new Catenary(span, cable.Weight, H_load);
            for (int i = 0; i < stress.Count; i++)
            {
                stress[i].SetTension(cat.AverageTension);
            }
            return cat;
        }
    }

}
