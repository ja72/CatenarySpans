using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace JA.Engineering
{
    public class SagAndTension 
    {
        public SagAndTension(StrandedCable cable, RulingSpan rulingSpan, params LoadingCondition[] loading_table)
            : this(rulingSpan.Units, cable, rulingSpan.Spans.ToArray(), loading_table)
        {  }
        public SagAndTension(ProjectUnits units, StrandedCable cable, Span[] spans, params LoadingCondition[] loading_table)            
        {
            this.Units=units;
            this.Cable = cable;
            this.Loading=new List<LoadingCondition>(loading_table);
            this.Spans=spans.Select((span)=> new LoadedCable(cable, span)).ToArray();
        }
        [XmlElement()]
        public ProjectUnits Units { get; }
        [XmlArray()]
        public List<LoadingCondition> Loading { get; }
        [XmlIgnore()]
        public DesignCondition Design { get { return Loading.FirstOrDefault() as DesignCondition; } }
        [XmlIgnore()]
        public LoadedCable[] Spans { get; }
        [XmlIgnore()]
        public StrandedCable Cable { get; }

        public SagAndTensionResults CalculateTable()
        {
            return new SagAndTensionResults(this);
        }
    }

    public class SagAndTensionResults
    {
        public SagAndTensionResults(SagAndTension sag)
        {
            this.Information = sag;
            this.Loading = sag.Loading.ToArray();
            this.Units = sag.Units;
            this.Cable = sag.Cable;
            this.Spans = sag.Spans.Select((l)=>l.Span).ToArray();
            var init = new Dictionary<LoadingCondition, Catenary>[sag.Spans.Length];
            var fini = new Dictionary<LoadingCondition, Catenary>[sag.Spans.Length];
            for (int i = 0; i < sag.Spans.Length; i++)
            {
                var span = sag.Spans[i];
                var R = span.FindPreStrain(sag.Design);
                span.SetPreStrain(R);
                init[i] = new Dictionary<LoadingCondition, Catenary>();
                foreach (var load in sag.Loading)
                {
                    init[i].Add(load, span.GetCatenaryAtLoading(load));
                }
                fini[i] = new Dictionary<LoadingCondition, Catenary>();
                span.LoadWith(sag.Design);
                foreach (var load in sag.Loading)
                {
                    fini[i].Add(load, span.GetCatenaryAtLoading(load));
                }
            }
            Initial = init;
            Final = fini;
        }
        public ProjectUnits Units { get; }
        public SagAndTension Information { get; }
        public StrandedCable Cable { get; }
        public LoadingCondition[] Loading { get; }
        public Span[] Spans { get; }
        public Dictionary<LoadingCondition, Catenary>[] Initial { get; }
        public Dictionary<LoadingCondition, Catenary>[] Final { get; }

        public string GetReport()
        {
            var sw = new StringWriter();
            sw.WriteLine($"Sag and Tension Table. Generated on {DateTime.Now.ToShortDateString()} by {Environment.UserName}");
            sw.WriteLine();
            sw.WriteLine($"Cable Area={Cable.Area:F2}sqin, Diameter={Cable.Diameter:F2} in, Weight={Cable.Weight:F2} lb/ft, RTS={Cable.RatedStrength:F0} lb");
            for (int index = 0; index < Spans.Length; index++)
            {
                sw.WriteLine();
                sw.WriteLine($"Span={Spans[index].SpanLength:F0}, Design Limit={Information.Design.Limit}");
                sw.WriteLine($"{"Design Points",34} {"Final",22} {"Initial",22}");
                sw.WriteLine($"{"Temp",6} {"Ice",6} {"Wind",6} {"K",6} {"Wt",6} {"Sag",6} {"Tension",8} {"RTS",6} {"Sag",6} {"Tension",8} {"RTS",6}");
                sw.WriteLine($"{"°F",6} {"in",6} {"psf",6} {"lb/ft",6} {"lb/ft",6} {"ft",6} {"lb",8} {"%",6} {"ft",6} {"lb",8} {"%",6}");
                foreach (var key in Loading)
                {
                    var init = Initial[index][key];
                    var fini = Final[index][key];
                    sw.WriteLine($"{key.Temperature,6:F0} {key.IceThickness,6:G2} {key.WindPressure,6:G2} {key.NESC_K,6:G2} {init.Weight,6:F2} {fini.MidSag,6:F2} {fini.AverageTension,8:F0} {100 * fini.AverageTension / Cable.RatedStrength,6:F1} {init.MidSag,6:F2} {init.AverageTension,8:F0} {100 * init.AverageTension / Cable.RatedStrength,6:F1}");
                }
            }
            return sw.ToString();
        }
    }
}
