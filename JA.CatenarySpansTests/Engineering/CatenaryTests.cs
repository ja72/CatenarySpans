using JA.Engineering;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JA.Engineering.Tests
{
    [TestClass()]
    public class CatenaryTests
    {
        [TestMethod]
        public void Cat_GivenHTension()
        {
            double H = 5000.0, w = 0.75, S = 1200, h = 100;
            var span = new Span(Vector2.UnitX*h, S, 0);
            var cat = new Catenary(span, w, H);
            double a = H/w, η = w*S/(2*H);
            double D_expect = a*(Math.Cosh(η)-1);
            double L_expect = 2*a*Math.Sinh(η);
            double T_expect = H*Math.Cosh(η);
            double ε_expect = 100*(L_expect/S-1);

            Assert.AreEqual(a, cat.CatenaryConstant);
            Assert.AreEqual(η, cat.Eta);
            Assert.AreEqual(D_expect, cat.MaximumSag, 1e-12);
            Assert.AreEqual(L_expect, cat.TotalLength, 1e-12);
            Assert.AreEqual(T_expect, cat.MaxTension, 1e-12);
            Assert.AreEqual(ε_expect, cat.GeometricStrainPct, 1e-12);
        }

        [TestMethod]
        public void Cat_GivenSag()
        {
            double H = 5000.0, w = 0.75, S = 1400, h = -10;
            var span = new Span(Vector2.UnitY*h, S, h);
            var cat = new Catenary(span, w, H);

            double D_expect = 65;
            cat.MaximumSag  = D_expect;
            Assert.AreEqual(D_expect, cat.MaximumSag, 1e-5);

            double H_expect = 2835.082;
            Assert.AreEqual(H_expect, cat.HorizontalTension, 1e-1);
        }
        [TestMethod]
        public void Cat_GivenAverageTension()
        {
            double H = 5000.0, w = 0.75, S = 1400, h = -10;
            var span = new Span(Vector2.UnitY*h, S, h);
            var cat = new Catenary(span, w, H);

            double P_expect = 3000;
            cat.AverageTension  = P_expect;
            Assert.AreEqual(P_expect, cat.AverageTension, 1e-3);

            double H_expect = 2984.443;
            Assert.AreEqual(H_expect, cat.HorizontalTension, 1e-1);
        }
        [TestMethod]
        public void Cat_GivenLength()
        {
            double H = 5000.0, w = 0.75, S = 1400, h = -10;
            var span = new Span(Vector2.UnitY*h, S, h);
            var cat = new Catenary(span, w, H);
            double L = 1405;
            cat.TotalLength= L;
            Assert.AreEqual(L, cat.TotalLength, 1e-1);

            double H_expect = 3601.178;
            Assert.AreEqual(H_expect, cat.HorizontalTension, 1e-1);

        }

        [TestMethod]
        public void Cat_GivenClearance()
        {
            double H = 5000.0, w = 0.75, S = 1400, h = -10;
            var span = new Span(Vector2.UnitY*100, S, h);
            var cat = new Catenary(span, w, H);

            double C_expect = 64;
            cat.Clearance = C_expect;

            double H_expect = 5970.4;
            Assert.AreEqual(H_expect, cat.HorizontalTension, 1e-1);

            cat.SetClearancePoint(new Vector2(300, 60));

            Assert.AreEqual(3278.9, cat.HorizontalTension, 1e-1);
        }

        [TestMethod()]
        public void CatenaryTest()
        {
            Span s = new Span(new Vector2(15, 100), 300, 50);
            var c = new Catenary(s, 1.15, 770);
            Assert.IsTrue(c.IsOK);
            Assert.IsTrue(s.Equals(c));
            Assert.AreEqual(1.15, c.CableWeight);
            Assert.AreEqual(770, c.HorizontalTension);

            Assert.AreEqual(669.5652, c.CatenaryConstant, 1e-4);
            CollectionAssert.AreEqual(new Vector2(54.832, 98.815), c.SagPoint, Ex.AbsComparer(1e-3));
            Assert.AreEqual(98.8149, c.Clearance, 1e-4);
            CollectionAssert.AreEqual(new Vector2(-770, 45.833), c.StartTension, Ex.AbsComparer(1e-3));
            CollectionAssert.AreEqual(new Vector2(770, 306.78), c.EndTension, Ex.AbsComparer(1e-3));
            Assert.AreEqual(0.2240, c.Eta, 1e-4);
            Assert.AreEqual(0.8160, c.GeometricStrainPct, 1e-4);
            Assert.IsFalse(c.IsStartTowerUplift);
            Assert.IsFalse(c.IsEndTowerUplift);
            CollectionAssert.AreEqual(new Vector2(54.832, 98.815), c.LowestPosition, Ex.AbsComparer(1e-3));
            Assert.AreEqual(17.1019, c.MaximumSag, 1e-4);
            Assert.AreEqual(17.1012, c.MidSag, 1e-4);
            Assert.AreEqual(306.6199, c.TotalLength, 1e-4);
            Assert.AreEqual(352.6128, c.TotalWeight, 1e-4);

        }

        [TestMethod()]
        public void CatenaryTest1()
        {
            Span s = new Span(new Vector2(15, 100), 300, 50);
            var c = new Catenary(new Vector2(15, 100),300, 50, 1.15, 770);
            Assert.IsTrue(c.IsOK);
            Assert.IsTrue(s.Equals(c));
            Assert.AreEqual(1.15, c.CableWeight);
            Assert.AreEqual(770, c.HorizontalTension);
        }

        [TestMethod()]
        public void CatenaryTest2()
        {
            Span s = new Span(new Vector2(15, 100), 300, 50);
            var c = new Catenary(new Vector2(15, 100), new Vector2(300, 50), 1.15, 770);
            Assert.IsTrue(c.IsOK);
            Assert.IsTrue(s.Equals(c));
            Assert.AreEqual(1.15, c.CableWeight);
            Assert.AreEqual(770, c.HorizontalTension);
        }

        [TestMethod()]
        public void CatenaryTest3()
        {
            var c = new Catenary(new Vector2(15, 100), 300, 50, 1.15, 770);
            var g = new Catenary(c);
            Assert.IsTrue(g.Equals(c));            
        }

        [TestMethod()]
        public void SetClearancePointTest()
        {
            var c = new Catenary(new Vector2(15, 100), 300, 50, 1.15, 770);
            var point = new Vector2(50.0, 95.0);
            c.SetClearancePoint(point);
            var x = point.X;
            var actual = new Vector2(x, c.CatenaryFunction(x));
            CollectionAssert.AreEqual(point, actual, Ex.AbsComparer(1e-3));
        }

        [TestMethod()]
        public void ToStringTest()
        {
            var c = new Catenary(new Vector2(15.55555555, 100), 300, 50, 1.155555555, 770.050610824);
            var t = c.ToString();
            var e = "Start=(15.556,100), Step=(300,50), H=770.051, w=1.156";
            Assert.AreEqual(e, t);            
        }

    }
}

