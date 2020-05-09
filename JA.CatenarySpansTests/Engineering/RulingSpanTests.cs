using JA.Engineering;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JA.Engineering.Tests
{
    [TestClass()]
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

        [TestMethod]
        public void RS_SaveAndLoad()
        {
            var span1 = new Span(Vector2.UnitY*100, 650, 20);
            var span2 = new Span(span1, 1250, -12);
            var span3 = new Span(span2, 800, -16);

            const int H = 3200;
            const double Wt = 0.75;

            var rs = new RulingSpan(ProjectUnits.Default, span1, span2, span3);
            rs.SetCableWeight(Wt);
            rs.SetHorizontalTension(H);

            const string file = "rs_save.spanx";
            rs.SaveFile(file);

            rs = RulingSpan.OpenFile(file);

            Assert.AreEqual(3, rs.Spans.Count);

            Assert.AreEqual(ProjectUnits.Default.ToString(), rs.UnitSymbols);
            Assert.AreEqual(H, rs.HorizontalTension);
            for (int i = 0; i < rs.Spans.Count; i++)
            {
                Assert.AreEqual(Wt, rs[i].Weight);
            }

        }

        [TestMethod()]
        public void RulingSpanTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void RulingSpanTest1()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void RulingSpanTest2()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void RulingSpanTest3()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void RulingSpanTest4()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void AddArrayTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void AddArrayTest1()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void NewCatenaryTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetRulingSpanCatenaryTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void UpdateAllFromFromTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void UpdateAllCatenaryTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void UpdateSpanEndsTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void SetCableWeightTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void SetHorizontalTensionTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void SetHorizontalTensionFromTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void FindCatenaryFromXTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ClearanceToTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void OpenFileTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void SaveFileTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void CloneTest()
        {
            Assert.Fail();
        }
    }
}


