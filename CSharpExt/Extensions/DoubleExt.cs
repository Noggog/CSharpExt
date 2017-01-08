using System;

namespace System
{
    public static class DoubleExt
    {
        public static double Modulo(this double a, double b)
        {
            return a - Math.Floor(a / b) * b;
        }

        public static bool EqualsWithin(this double a, double b, double within = 0.000000001d)
        {
            return Math.Abs(a - b) < within;
        }

        public static T Clamp<T>(this T val, T min, T max)
            where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0) return min;
            else if (val.CompareTo(max) > 0) return max;
            else return val;
        }

        public static double Clamp(this double a, double min, double max)
        {
            return Math.Min(Math.Max(a, min), max);
        }

        public static double Clamp01(this double a)
        {
            return Math.Min(Math.Max(a, 0d), 1d);
        }

        public static int Round(this double a)
        {
            return (int)Math.Round(a);
        }
    }
}
