using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JA.Engineering
{
    [TestClass]
    public class CatenaryTests
    {
        [TestMethod]
        public void Cat_GivenHTension()
        {
            double H = 5000.0, w = 0.75, S=1200, h=100;
            var span=new Span(Vector2.UnitX*h, S, 0);
            var cat=new Catenary(span, w, H);
            double a=H/w, η=w*S/(2*H);
            double D_expect=a*(Math.Cosh(η)-1);
            double L_expect=2*a*Math.Sinh(η);
            double T_expect=H*Math.Cosh(η);
            double ε_expect=100*(L_expect/S-1);

            Assert.AreEqual(a, cat.CatenaryConstant);
            Assert.AreEqual(η, cat.Eta);
            Assert.AreEqual(D_expect, cat.MaximumSag, 1e-12);
            Assert.AreEqual(L_expect, cat.TotalLength,1e-12);
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
    }
}
