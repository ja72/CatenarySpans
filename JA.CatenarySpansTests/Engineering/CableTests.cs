using Microsoft.VisualStudio.TestTools.UnitTesting;
using JA.Engineering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JA.Engineering.Tests
{
    [TestClass()]
    public class CableTests
    {


        [TestMethod()]
        public void CableTest()
        {
            Cable cable = new Cable(1.0, 0.75, 1.25, 18000);
            Assert.AreEqual(1.0, cable.Diameter);
            Assert.AreEqual(0.75, cable.Area);
            Assert.AreEqual(1.25, cable.Weight);
            Assert.AreEqual(18000, cable.RatedStrength);
        }

        [TestMethod()]
        public void CableTest1()
        {
            ICable other = new Cable(1.0, 0.75, 1.25, 18000);
            Cable cable = new Cable(other);
            Assert.AreEqual(1.0, cable.Diameter);
            Assert.AreEqual(0.75, cable.Area);
            Assert.AreEqual(1.25, cable.Weight);
            Assert.AreEqual(18000, cable.RatedStrength);
        }

        [TestMethod()]
        public void CableTest2()
        {
            Cable other = new Cable(1.0, 0.75, 1.25, 18000);
            Cable cable = new Cable(other);
            Assert.AreEqual(1.0, cable.Diameter);
            Assert.AreEqual(0.75, cable.Area);
            Assert.AreEqual(1.25, cable.Weight);
            Assert.AreEqual(18000, cable.RatedStrength);
        }

        [TestMethod()]
        public void CableTest3()
        {
            Cable cable = new Cable(1.0,
                (Materials.SteelCore, 0.15),
                (Materials.AL6201T81, 0.60));

            Assert.AreEqual(1.0, cable.Diameter);
            Assert.AreEqual(0.75, cable.Area);
            Assert.AreEqual(1.22382, cable.Weight);
            Assert.AreEqual(55350.0, cable.RatedStrength);
        }

        [TestMethod()]
        public void Cable_CloneAndEqualsTest()
        {
            var c1 = new Cable(1.0, 0.75, 1.25, 18000);
            var c2 = c1.Clone();

            Assert.AreEqual(c1, c2);
        }
    }
}