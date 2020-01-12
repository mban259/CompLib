using System;
using CompLib.Mathematics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest.Mathematics
{
    [TestClass]
    public class EquationTest
    {
        [TestMethod]
        public void RandomTest1()
        {
            var r = new Random();
            for (int i = 0; i < 10000000; i++)
            {
                double a = r.NextDouble() * 100;
                double b = r.NextDouble() * 100;

                double x = Equation.LinearEquation(a, b);

                Assert.IsTrue(Math.Abs(a * x + b) < 0.000001);
            }
        }

        [TestMethod]
        public void RandomTest2()
        {
            var r = new Random();
            for (int i = 0; i < 1000000; i++)
            {
                double a = r.NextDouble() * 100;
                double b = r.NextDouble() * 100;
                double c = r.NextDouble() * 100;
                double d = r.NextDouble() * 100;
                double e = r.NextDouble() * 100;
                double f = r.NextDouble() * 100;
                double x, y;
                Equation.SimultaneousEquations(a, b, c, d, e, f, out x, out y);
                Assert.IsTrue(Math.Abs(a * x + b * y + c) < 0.000001, $"{a * x + b * y + c}");
                Assert.IsTrue(Math.Abs(d * x + e * y + f) < 0.000001, $"{d * x + e * y + f}");
            }
        }
    }
}