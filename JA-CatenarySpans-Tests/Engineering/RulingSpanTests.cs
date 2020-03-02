using JA.Engineering;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JA.Engineering
{
    [TestClass]
    public class RulingSpanTests
    {

        [TestMethod]
        public void RS_CalcSpan()
        {
            // Define three spans
            var span1 = new Span(Vector2.UnitY*100, 650, 20);
            var span2 = new Span(span1, 1250, -12);
            var span3 = new Span(span2, 800, -16);

            const int H = 3200;
            const double Wt = 0.75;

            var rs = new RulingSpan(ProjectUnits.Default, span1, span2, span3);
            rs.SetCableWeight(Wt);
            rs.SetHorizontalTension(H);

            for (int i = 0; i < rs.Spans.Count; i++)
            {
                Assert.AreEqual(Wt, rs.Spans[i].Weight);
                Assert.AreEqual(H, rs.Spans[i].HorizontalTension);
            }
            CollectionAssert.AreEqual(new double[] { 2700, 92 }, rs.Last.EndPosition);

            Assert.AreEqual(2706.80, rs.TotalLength, 1e-2);

            var ave = new[] { 3204.62, 3211.64, 3205.34 };

            for (int i = 0; i < rs.Spans.Count; i++)
            {
                Assert.AreEqual(ave[i], rs.Spans[i].AverageTension, 1e-2);
            }

        }
    }
}
