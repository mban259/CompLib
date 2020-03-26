using System.Collections.Generic;
using System.Linq;
using CompLib.Mathematics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest.Mathematics
{
    [TestClass]
    public class EuclideanGeometryTest
    {
        [TestMethod]
        public void AGC021BSample1()
        {
            var ps = new EuclideanGeometry.P[2];
            ps[0] = new EuclideanGeometry.P(0, 0);
            ps[1] = new EuclideanGeometry.P(1, 1);
            List<EuclideanGeometry.P> l = new List<EuclideanGeometry.P>();
            var r = EuclideanGeometry.ConvexHull(ps, l);
            Assert.AreEqual(2, r.Length);
            Assert.AreEqual(2, l.Count);
            Assert.IsTrue(r.Contains(0) && r.Contains(1));
            Assert.IsTrue(l.Contains(ps[0]) && l.Contains(ps[1]));
        }

        [TestMethod]
        public void AGC021BSample2()
        {
            var ps = new EuclideanGeometry.P[5];
            ps[0] = new EuclideanGeometry.P(0, 0);
            ps[1] = new EuclideanGeometry.P(2, 8);
            ps[2] = new EuclideanGeometry.P(4, 5);
            ps[3] = new EuclideanGeometry.P(2, 6);
            ps[4] = new EuclideanGeometry.P(3, 10);
            List<EuclideanGeometry.P> l = new List<EuclideanGeometry.P>();
            var r = EuclideanGeometry.ConvexHull(ps, l);
            Assert.AreEqual(4, r.Length);
            Assert.AreEqual(4, l.Count);
            Assert.IsTrue(r.Contains(0) && r.Contains(1) && r.Contains(2) && r.Contains(4) && !r.Contains(3));
            Assert.IsTrue(l.Contains(ps[0]) && l.Contains(ps[1]) && l.Contains(ps[2]) && l.Contains(ps[4]) &&
                          !l.Contains(ps[3]));
        }
    }
}