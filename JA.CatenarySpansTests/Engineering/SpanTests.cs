using Microsoft.VisualStudio.TestTools.UnitTesting;
using JA.Engineering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Math;

namespace JA.Engineering.Tests
{
    [TestClass()]
    public class SpanTests
    {
        [TestMethod()]
        public void SpanTest()
        {
            Span s = new Span(new Vector2(15,100), 300, 50);
            Assert.IsTrue(s.IsOK);
            CollectionAssert.AreEqual(new Vector2(15, 100), s.StartPosition);
            Assert.AreEqual(15, s.StartX);
            Assert.AreEqual(100, s.StartY);
            CollectionAssert.AreEqual(new Vector2(300, 50), s.Step);
            Assert.AreEqual(300, s.StepX);
            Assert.AreEqual(50, s.StepY);
            Assert.AreEqual(Sqrt(300*300+50*50), s.DiagonalLength);
            CollectionAssert.AreEqual(new Vector2(315, 150), s.EndPosition);
            CollectionAssert.AreEqual(new Vector2(15, 0), s.StartBase);
            CollectionAssert.AreEqual(new Vector2(315, 0), s.EndBase);            
        }

        [TestMethod()]
        public void SpanTest1()
        {
            ISpan g = new Span(new Vector2(15, 100), 300, 50);
            Assert.IsTrue(g.IsOK);
            Span s = new Span(g);
            Assert.IsTrue(s.IsOK);
            Assert.AreEqual(new Vector2(15, 100), s.StartPosition);
            Assert.AreEqual(15, s.StartX);
            Assert.AreEqual(100, s.StartY);
            Assert.AreEqual(new Vector2(300, 50), s.Step);
            Assert.AreEqual(300, s.StepX);
            Assert.AreEqual(50, s.StepY);
            Assert.AreEqual(Sqrt(300*300+50*50), s.DiagonalLength);
            Assert.AreEqual(new Vector2(315, 150), s.EndPosition);
            Assert.AreEqual(new Vector2(15, 0), s.StartBase);
            Assert.AreEqual(new Vector2(315, 0), s.EndBase);
        }

        [TestMethod()]
        public void SpanTest2()
        {
            Span s = new Span(new Vector2(15, 100), new Vector2(300, 50));
            Assert.IsTrue(s.IsOK);
            Assert.AreEqual(new Vector2(15, 100), s.StartPosition);
            Assert.AreEqual(15, s.StartX);
            Assert.AreEqual(100, s.StartY);
            Assert.AreEqual(new Vector2(300, 50), s.Step);
            Assert.AreEqual(300, s.StepX);
            Assert.AreEqual(50, s.StepY);
            Assert.AreEqual(Sqrt(300*300+50*50), s.DiagonalLength);
            Assert.AreEqual(new Vector2(315, 150), s.EndPosition);
            Assert.AreEqual(new Vector2(15, 0), s.StartBase);
            Assert.AreEqual(new Vector2(315, 0), s.EndBase);

        }

        [TestMethod()]
        public void SpanTest3()
        {
            Span g = new Span();
            Assert.IsTrue(g.IsOK);
            Span s = new Span(g, 300, -25);
            Assert.IsTrue(s.IsOK);
            Assert.AreEqual(g.EndPosition, s.StartPosition);
            Assert.AreEqual(new Vector2(300, -25), s.Step);
        }

        [TestMethod()]
        public void SpanTest4()
        {
            Span s = new Span();
            Assert.IsTrue(s.IsOK);
            Assert.AreEqual(0, s.StartX);
            Assert.AreEqual(Span.DefaultTowerHeight, s.StartY);
            Assert.AreEqual(Span.DefaultSpanLength, s.StepX);
            Assert.AreEqual(Span.DefaultSpanRise, s.StepY);
        }

        [TestMethod()]
        public void GetBoundsTest()
        {
            Vector2 start = new Vector2(15,10), end = start;
            Span s = new Span(start, 300, 50);
            s.GetBounds(ref start, ref end);
            Assert.AreEqual(start, s.StartPosition);
            Assert.AreEqual(end, s.EndPosition);
        }

        [TestMethod()]
        public void ContainsXTest()
        {
            Span s = new Span(new Vector2(15, 100), new Vector2(300, 50));
            Assert.IsFalse(s.ContainsX(0));
            Assert.IsTrue(s.ContainsX(15));
            Assert.IsTrue(s.ContainsX(150));
            Assert.IsFalse(s.ContainsX(350));
        }

        [TestMethod()]
        public void CloneTest()
        {
            Span g = new Span(new Vector2(15, 100), new Vector2(300, 50));
            Span s = g.Clone();

            Assert.AreEqual(g, s);
        }

        [TestMethod()]
        public void ToStringTest()
        {
            Span s = new Span(new Vector2(15, 100), new Vector2(300, 50));
            string t = s.ToString();
            Assert.AreEqual("Start=(15,100), Step=(300,50)", t);
        }

    }
}