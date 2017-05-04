using System;

namespace System
{
    public static class Int64Ext
    {
        public static bool IsInRange(this long d, long min, long max)
        {
            if (d < min) return false;
            if (d > max) return false;
            return true;
        }

        public static long InRange(this long d, long min, long max)
        {
            if (d < min) throw new ArgumentException($"{d} was lower than the minimum {min}.");
            if (d > max) throw new ArgumentException($"{d} was greater than the maximum {max}.");
            return d;
        }

        public static long PutInRange(this long d, long min, long max)
        {
            if (d < min) return min;
            if (d > max) return max;
            return d;
        }
    }
}
