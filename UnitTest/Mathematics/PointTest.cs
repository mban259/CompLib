using System;
using CompLib.Mathematics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest.Mathematics
{
    [TestClass]
    public class PointTest
    {
        [TestMethod]
        public void RandomCircleTest()
        {
            var r = new Random();
            int cnt = 0;
            while (cnt < 10000)
            {
                double x1 = r.NextDouble() * 100;
                double y1 = r.NextDouble() * 100;
                double x2 = r.NextDouble() * 100;
                double y2 = r.NextDouble() * 100;
                double x3 = r.NextDouble() * 100;
                double y3 = r.NextDouble() * 100;

                double cx, cy, cr;

                if (!Point.Circle(x1, y1, x2, y2, x3, y3, out cx, out cy, out cr))
                    continue;

                cnt++;
                double dx1 = x1 - cx;
                double dy1 = y1 - cy;
                Assert.IsTrue(Math.Abs(dx1 * dx1 + dy1 * dy1 - cr * cr) < 0.1,
                    $"1 {dx1 * dx1 + dy1 * dy1 - cr * cr} {cx} {cy} {cr}");

                double dx2 = x2 - cx;
                double dy2 = y2 - cy;
                Assert.IsTrue(Math.Abs(dx2 * dx2 + dy2 * dy2 - cr * cr) < 0.1,
                    $"2 {dx2 * dx2 + dy2 * dy2 - cr * cr} {cx} {cy} {cr}");

                double dx3 = x3 - cx;
                double dy3 = y3 - cy;
                Assert.IsTrue(Math.Abs(dx3 * dx3 + dy3 * dy3 - cr * cr) < 0.1,
                    $"3 {dx3 * dx3 + dy3 * dy3 - cr * cr} {cx} {cy} {cr}");
            }
        }

        [TestMethod]
        public void CircleTest()
        {
            // 直角二等辺三角形

            double x, y, r;

            Point.Circle(1, 1, 1, -1, -1, 1, out x, out y, out r);

            bool a = Math.Abs(x) < 0.0000001;
            bool b = Math.Abs(y) < 0.0000001;
            bool c = Math.Abs(r - Math.Sqrt(2)) < 0.0000001;
            Assert.IsTrue(a && b && c, $"actual 0,0,{Math.Sqrt(2)} expect {x},{y},{r}");
        }

        [TestMethod]
        public void AngleTest1()
        {
            // 60°
            // O (1,2)
            // A (101,2)
            // B (11,10√3 + 2)

            double r = Point.Angle(101, 2, 1, 2, 11, 10 * Math.Sqrt(3) + 2);
            double sixty = Math.PI / 3;

            Assert.IsTrue(Math.Abs(r - sixty) < 0.000001);
        }

        [TestMethod]
        public void AngleTest2()
        {
            double r = Point.Angle(-1, 1, 1, 1, 1, -1);
            Assert.IsTrue(Math.Abs(r - Math.PI / 2) < 0.000001, $"actual {Math.PI / 2} expect{r}");
        }

        [TestMethod]
        public void DistTest1()
        {
            double d = Point.Dist(1, -1, -1, 1);
            Assert.IsTrue(Math.Abs(d - Math.Sqrt(8)) < 0.000001, $"actual {Math.Sqrt(8)} {d}");
        }
    }
}