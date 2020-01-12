namespace CompLib.Mathematics
{
    public static partial class Equation
    {
        /// <summary>
        /// ax + b = 0
        /// xを求める
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static double LinearEquation(double a, double b)
        {
            return -b / a;
        }

        /// <summary>
        /// ax + by + c = 0
        /// dx + ey + f = 0
        /// x,yを求める
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="d"></param>
        /// <param name="e"></param>
        /// <param name="f"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static void SimultaneousEquations(double a, double b, double c, double d, double e, double f,
            out double x, out double y)
        {
            /*
             * ax + by + c = 0
             * dx + ey + f = 0
             *
             * adx + bdy + cd = 0
             * adx + aey + af = 0
             *
             * (bd - ae)y + (cd - af) = 0 
             */

            if (a == 0)
            {
                LinearEquation(b, c);
            }

            y = LinearEquation(b * d - a * e, d * c - a * f);
            x = a == 0 ? LinearEquation(d, e * y + f) : LinearEquation(a, b * y + c);
        }
    }
}