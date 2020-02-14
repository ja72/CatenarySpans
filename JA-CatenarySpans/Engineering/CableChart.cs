using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace JA.Engineering
{
    public class AlcoaChart
    {
        public class ChartComponent
        {
            [XmlAttribute("Initial")]
            public string InitialString
            {
                get { return Initial.Coefficients.ToCSV(); }
                set { Initial=value.FromCSV(); }
            }
            [XmlIgnore()]
            public Polynomial Initial { get; set; }
            [XmlAttribute()]
            public double Final { get; set; }
            [XmlAttribute()]
            public double CTE { get; set; }
            [XmlIgnore()]
            public bool IsOK { get { return Final>0&&Initial.Order>0&&CTE>0; } }
        }

        public AlcoaChart()
        {
            Code=string.Empty;
            RefTemperature=70;
            Core=new ChartComponent() { Initial=Polynomial.Empty, Final=0, CTE=0 };
            Outer=new ChartComponent() { Initial=Polynomial.Empty, Final=0, CTE=0 };
        }

        [XmlAttribute()]
        public string Code { get; set; }
        [XmlAttribute()]
        public double RefTemperature { get; set; }
        [XmlElement()]
        public ChartComponent Core { get; set; }
        [XmlElement()]
        public ChartComponent Outer { get; set; }
        [XmlIgnore()]
        public bool IsOK { get { return Core.IsOK; } }

        public static readonly AlcoaChart ACSR_26_7=new AlcoaChart()
        {
            Code="1-537",
            RefTemperature=70,
            Outer=new ChartComponent()
            {
                InitialString="-1213.0, 44308.1, -14004.4, -37618.0, 30676.0",
                Final=64000,
                CTE=0.00128,
            },
            Core=new ChartComponent()
            {
                InitialString="-69.3, 38629.0, 3998.1, -45713.0, 27892.0",
                Final=37000,
                CTE=0.00064,
            }
        };
    }
}
