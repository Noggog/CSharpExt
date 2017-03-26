using System;

namespace System
{
    public static class FloatExt
    {
        public static int ToInt(this float a)
        {
            return Convert.ToInt32(a);
        }

        public static bool EqualsWithin(this float a, float b, float within = 0.000000001f)
        {
            return Math.Abs(a - b) < within;
        }

        public static int Round(this float a)
        {
            return (int)Math.Round(a);
        }

        public static bool IsReal(this float f)
        {
            return !float.IsInfinity(f)
                && !float.IsNaN(f);
        }

        public static float Average(this float a, float b)
        {
            float less, more;
            if (a < b)
            {
                less = a;
                more = b;
            }
            else
            {
                more = a;
                less = b;
            }
            var diff = more - less;
            diff /= 2;
            return diff + less;
        }
    }
}
