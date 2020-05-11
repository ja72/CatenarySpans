using JA.Engineering;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JA.Engineering.Tests
{
    [TestClass()]
    public class RulingSpanTests
    {

        public static RulingSpan TestRulingSpan(double weight, double H, bool flat=false)
        {
            var span1 = new Span(Vector2.UnitY*100, 650, flat ? 0 : 20);
            var span2 = new Span(span1, 1250, flat ? 0 : -12);
            var span3 = new Span(span2, 800, flat ? 0 : -16);
            return new RulingSpan(new[] { span1, span2, span3 }, weight, H);
        }

        [TestMethod]
        public void RS_CalcSpan()
        {
            double Wt = 0.75, H = 3200;
            var rs = TestRulingSpan(Wt, H);

            for (int i = 0; i < rs.Spans.Count; i++)
            {
                Assert.AreEqual(Wt, rs.Spans[i].CableWeight);
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
            double Wt = 0.75, H = 3200;
            var rs = TestRulingSpan(Wt, H);

            const string file = "rs_save.spanx";
            rs.SaveFile(file);

            var gs = RulingSpan.OpenFile(file);

            File.Delete(file);
            Assert.IsTrue(gs.Equals(rs));
        }

        [TestMethod()]
        public void RulingSpanTest()
        {
            var projectUnits = new ProjectUnits(ProjectUnitSystem.NewtonMeterSecond);
            var cat1 = Catenary.Default(projectUnits);
            var cat2 = Catenary.Default(projectUnits);
            var cat3 = Catenary.Default(projectUnits);

            var rs = new RulingSpan(
                projectUnits, cat1, cat2, cat3);

            Assert.IsTrue(rs.IsOk);
        }

        [TestMethod()]
        public void NewCatenaryTest()
        {
            double Wt = 0.75, H = 3200;
            var rs = TestRulingSpan(Wt, H);

            var cat = rs.NewCatenary();

            Assert.AreEqual(rs.Last.StepX, cat.StepX);
            Assert.AreEqual(0, cat.StepY);
            Assert.AreEqual(rs.Last.CableWeight, cat.CableWeight);
            Assert.AreEqual(rs.Last.HorizontalTension, cat.HorizontalTension);
        }

        [TestMethod()]
        public void GetRulingSpanCatenaryTest()
        {
            var rs = TestRulingSpan(1.15, 2750, flat: true);
            var cat = rs.GetRulingSpanCatenary();

            Assert.AreEqual(rs.CableWeight, cat.CableWeight);
            Assert.AreEqual(rs.HorizontalTension, cat.HorizontalTension);

            var numerator = rs.Spans.Sum((s) => s.StepX.Cub());
            var denominator = rs.Spans.Sum((s) => s.StepX);
            var rs_calc = Math.Sqrt(numerator/denominator);

            Assert.AreEqual(rs_calc, cat.StepX, 0.1);            
        }

        [TestMethod()]
        public void UpdateAllFromFromTest()
        {
            var rs = TestRulingSpan(1.15, 2750);

            rs.RaiseListChangedEvents = false;
            rs.Spans[1].HorizontalTension = 3500;
            Assert.AreNotEqual(rs.Spans[1].HorizontalTension, rs.Spans[0].HorizontalTension);
            Assert.AreNotEqual(rs.Spans[1].HorizontalTension, rs.Spans[2].HorizontalTension);
            rs.UpdateAllFromFrom(1);
            rs.RaiseListChangedEvents = true;

            Assert.AreEqual(rs.Spans[1].HorizontalTension, rs.Spans[0].HorizontalTension);
            Assert.AreEqual(rs.Spans[1].HorizontalTension, rs.Spans[2].HorizontalTension);            
        }

        [TestMethod()]
        public void SetCableWeightTest()
        {
            var rs = TestRulingSpan(1.15, 2750);

            var x = rs.Spans[1].CenterX;

            rs.SetCableWeight(1.75);

            for (int i = 0; i < rs.Spans.Count; i++)
            {
                Assert.AreEqual(1.75, rs.Spans[i].CableWeight);
            }

            Assert.AreNotEqual(x, rs.Spans[1].CenterX);            
        }

        [TestMethod()]
        public void FindCatenaryFromXTest()
        {
            var rs = TestRulingSpan(1.15, 2750);

            Assert.AreEqual(rs.Spans[0], rs.FindCatenaryFromX(500));
            Assert.AreEqual(rs.Spans[1], rs.FindCatenaryFromX(700));
            Assert.AreEqual(rs.Spans[2], rs.FindCatenaryFromX(2000));
        }

        [TestMethod()]
        public void ClearanceToTest()
        {
            var rs = TestRulingSpan(1.15, 2750);
            var point = new Vector2(2000, 35);
            var h = rs.ClearanceTo(point, true);

            Assert.AreEqual(56.30, h, 0.01);
        }

        [TestMethod()]
        public void ToStringTest()
        {
            var rs = TestRulingSpan(1.15, 2750);
            var t = rs.ToString();

            var s = "RS(650, 1250, 800), Weight=1.15, H=2750";

            Assert.AreEqual(s, t);
        }

        [TestMethod()]
        public void CloneTest()
        {
            double Wt = 0.75, H = 3200;
            var rs = TestRulingSpan(Wt, H);

            var gs = new RulingSpan(rs);

            Assert.IsTrue(rs.Equals(gs));
        }
    }
}


