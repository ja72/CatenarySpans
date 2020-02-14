using JA.Engineering;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JA
{
    public class SagTensTests
    {
        public static void TestSagTension()
        {
            var drake = StrandedCable.Drake;
            drake.AssignChart(AlcoaChart.ACSR_26_7);
            var loads = LoadingCondition.NESC_Heavy.Concat(LoadingCondition.StandardTemperatures).ToArray();
            var rs = RulingSpan.OpenFile(@"Support\full.spanx");
            SagAndTension sag = new SagAndTension(drake, rs, loads);
            sag.Design.Limit = 15000.0;

            var table = sag.CalculateTable();
            var report = table.GetReport();

            const string fn = @"Support\report_0.txt";
            File.WriteAllText(fn, report);
            Process.Start(fn);
        }


        public static void TestCatSol()
        {

            var drake = StrandedCable.Drake;

            // SAG10 chart 1-597

            drake.AssignChart(AlcoaChart.ACSR_26_7);

            var span = new Span(50 * Vector2.UnitY, 1000 * Vector2.UnitX);
            var lc = new LoadedCable(drake, span);
            var loads = LoadingCondition.NESC_Heavy.Concat(LoadingCondition.StandardTemperatures).ToList();
            
            var design = loads[0] as DesignCondition;
            design.Limit = 15000;
            var units = new ProjectUnits(ProjectUnitSystem.FeetPoundSecond);
            var sag = new SagAndTension(units, drake, new[] { span }, loads.ToArray());
            var table = sag.CalculateTable();
            var report = table.GetReport();

            var item = loads[4];
            var cat = table.Initial[0][item];
        }
    }
}
