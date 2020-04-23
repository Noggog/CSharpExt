using System;

namespace Noggog
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

        public static bool EqualsWithin(this double? a, double? b, double within = 0.000000001d)
        {
            if (a == null) return b == null;
            if (b == null) return false;
            return Math.Abs(a.Value - b.Value) < within;
        }

        public static T Clamp<T>(this T val, T min, T max)
            where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0) return min;
            if (val.CompareTo(max) > 0) return max;
            return val;
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

        public static bool IsInRange(this double d, double min, double max)
        {
            if (d < min) return false;
            if (d > max) return false;
            return true;
        }

        public static double InRange(this double d, double min, double max)
        {
            if (d < min) throw new ArgumentException($"{d} was lower than the minimum {min}.");
            if (d > max) throw new ArgumentException($"{d} was greater than the maximum {max}.");
            return d;
        }

        public static double PutInRange(this double d, double min, double max)
        {
            if (d < min) return min;
            if (d > max) return max;
            return d;
        }
    }
}
