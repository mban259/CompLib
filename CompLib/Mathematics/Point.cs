namespace CompLib.Mathematics
{
    using System;

    public static partial class Point
    {
        /// <summary>
        /// 原点から (x,y)までの距離
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static double Dist(double x, double y)
        {
            return Math.Sqrt(x * x + y * y);
        }

        /// <summary>
        /// 点1 (x1,y1) から 点2 (x2,y2)までの距離
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <returns></returns>
        public static double Dist(double x1, double y1, double x2, double y2)
        {
            return Dist(x2 - x1, y2 - y1);
        }

        /// <summary>
        /// 原点 (x1,y1) (x2,y2)を頂点とする三角形の面積
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <returns></returns>
        public static double TriangleArea(double x1, double y1, double x2, double y2)
        {
            return Math.Abs(x1 * y2 - x2 * y1) / 2;
        }

        /// <summary>
        /// (x1,y1) (x2,y2) (x3,y3)を頂点とする三角形の面積
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="x3"></param>
        /// <param name="y3"></param>
        /// <returns></returns>
        public static double TriangleArea(double x1, double y1, double x2, double y2, double x3, double y3)
        {
            return TriangleArea(x1 - x3, y1 - y3, x2 - x3, y2 - y3);
        }

        /// <summary>
        /// 原点O 点A (x1,y1) B (x2,y2)
        /// ∠AOB 
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <returns></returns>
        public static double Angle(double x1, double y1, double x2, double y2)
        {
            double r = Math.Abs(Math.Atan2(y1, x1) - Math.Atan2(y2, x2));
            return r > Math.PI ? 2 * Math.PI - r : r;
        }

        /// <summary>
        /// 点 A (x1,y1) B(x2,y2) C(x3,y3)
        /// ∠ABC
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="x3"></param>
        /// <param name="y3"></param>
        /// <returns></returns>
        public static double Angle(double x1, double y1, double x2, double y2, double x3, double y3)
        {
            return Angle(x1 - x2, y1 - y2, x3 - x2, y3 - y2);
        }

        /// <summary>
        /// 2点を結ぶ直線 y = ax + bを求める
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        public static void Line(double x1, double y1, double x2, double y2, out double a, out double b)
        {
            Equation.SimultaneousEquations(x1, 1, -y1, x2, 1, -y1, out a, out b);
        }

        /// <summary>
        /// y = ax + b, y = cx + dの交点
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="d"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool Intersection(double a, double b, double c, double d, out double x, out double y)
        {
            if (a == c)
            {
                x = -1;
                y = -1;
                return false;
            }

            Equation.SimultaneousEquations(a, -1, b, c, -1, d, out x, out y);
            return true;
        }

        /// <summary>
        /// 線分 12の 垂直二等分線 y = ax + b
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        public static bool PerpendicularBisector(double x1, double y1, double x2, double y2, out double a, out double b)
        {
            a = -1;
            b = -1;
            if (x1 == x2) return false;
            double medX = (x1 + x2) / 2;
            double medY = (y1 + y2) / 2;

            a = -1 / (y1 - y2) / (x1 - x2);

            b = Equation.LinearEquation(1, a * medX - medY);
            return true;
        }


        // 誤差がすごい　なんとかする
        /// <summary>
        /// 点 1,2,3を頂点とする三角形の外接円
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="x3"></param>
        /// <param name="y3"></param>
        public static bool Circle(double x1, double y1, double x2, double y2, double x3, double y3, out double x,
            out double y, out double r)
        {
            x = -1;
            y = -1;
            r = -1;
            // 三角形が作れない
            if (x1 == x2 && x2 == x3)
            {
                return false;
            }

            double ta, tb, tc, td;
            Line(x1, y1, x2, y2, out ta, out tb);
            Line(x1, y1, x3, y3, out tc, out td);

            if (ta == tc)
            {
                return false;
            }


            // 2r = a/sinA
            double thetaA = Angle(x3, y3, x1, y1, x2, y2);

            double a = Dist(x2, y2, x3, y3);
            r = a / Math.Sin(thetaA) / 2;

            // r^2 = (x1-x)^2 + (y1-y)^2
            //     = x1^2 - 2・x1・x + x^2 + y1^2 - 2・y1・y + y^2
            /*
             * x1^2 - 2*x1*x + y1^2 - 2・y1・y = x2^2 - 2*x2*x + y2^2 - 2*y2*y
             * x1^2 - 2・x1・x + y2^2 - 2・y1・y = x3^2 - 2・x3・x + y3^2 - 2・y3・y
             *
             * 上の2元1次方程式解く
             *
             * x1^2 + y1^2 - x2^2 - y2^2 = 2*x1*x + 2*y1*y - 2*x2*x - 2*y2*y
             * x1^2 + y1^2 - x3^2 - y3^2 = 2*x1*x + 2*y1*y - 2*x3*x - 2*y3*y
             *
             * x1^2 + y1^2 - x2^2 - y2^2 = A
             * x1^2 + y1^2 - x3^2 - y3^2 = B
             * 2*x1 = C
             * 2*y1 = D
             * 2*x2 = E
             * 2*y2 = F
             * 2*x3 = G
             * 2*y3 = H
             * 
             *  (C-E)x + (D-F)y - A = 0
             *  (C-G)x + (D-H)y - B = 0
             *
             * 
             */

            double A = 2 * x1 - 2 * x2;
            double B = 2 * y1 - 2 * y2;
            double C = x2 * x2 + y2 * y2 - x1 * x1 - y1 * y1;

            double D = 2 * x1 - 2 * x3;
            double E = 2 * y1 - 2 * y3;
            double F = x3 * x3 + y3 * y3 - x1 * x1 - y1 * y1;

            Equation.SimultaneousEquations(A, B, C, D, E, F, out x, out y);

            return true;
        }

        /// <summary>
        /// (xi,yi)をすべて含む最小の円の半径 len(x) = len(y) >= 2
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// 
        public static double InclusionCircleRange(double[] x, double[] y)
        {
            double ng = 0;
            double ok = 999999999999;

            for (int i = 0; i < 100; i++)
            {
                double med = (ng + ok) / 2;
                if (Check(x, y, med))
                {
                    ok = med;
                }
                else
                {
                    ng = med;
                }
            }

            return ok;
        }

        // 半径rの円がx,yの包含円か
        private static bool Check(double[] x, double[] y, double r)
        {
            for (int i = 0; i < x.Length; i++)
            {
                for (int j = i + 1; j < x.Length; j++)
                {
                    // 2点の中点

                    double dist = Dist(x[i], y[i], x[j], y[j]);
                    double d = dist / 2;
                    if (d > r)
                    {
                        return false;
                    }

                    // 2円の交点　中点からの距離
                    double h = Math.Sqrt(r * r - d * d);
                    double medX = (x[i] + x[j]) / 2;
                    double medY = (y[i] + y[j]) / 2;
                    double sinA = h / r;
                    double cosA = d / r;

                    double sinB = (y[j] - y[i]) / dist;
                    double cosB = (x[j] - x[i]) / dist;

                    double sinApB = sinA * cosB + cosA * sinB;
                    double cosApB = cosA * cosB - sinA * sinB;

                    double x1 = x[i] + cosApB * r;
                    double y1 = y[i] + sinApB * r;

                    if (Inclusion(x, y, x1, y1, r))
                    {
                        return true;
                    }

                    double x2 = 2 * medX - x1;
                    double y2 = 2 * medY - y1;

                    if (Inclusion(x, y, x2, y2, r))
                    {
                        return true;
                    }
                }
            }

            return false;
        }


        /// <summary>
        /// 中心 (cx,cy) 半径 cr (xi,yi)をすべて含むか
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="cx"></param>
        /// <param name="cy"></param>
        /// <param name="cr"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool Inclusion(double[] x, double[] y, double cx, double cy, double cr, int a = -1, int b = -1)
        {
            for (int i = 0; i < x.Length; i++)
            {
                if (i == a || i == b) continue;
                double dx = x[i] - cx;
                double dy = y[i] - cy;
                if (cr * cr < dx * dx + dy * dy)
                {
                    return false;
                }
            }

            return true;
        }
    }
}