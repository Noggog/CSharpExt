using System;

namespace Noggog
{
    public static class Int8Ext
    {
        public static bool IsInRange(this sbyte d, sbyte min, sbyte max)
        {
            if (d < min) return false;
            if (d > max) return false;
            return true;
        }

        public static sbyte InRange(this sbyte d, sbyte min, sbyte max)
        {
            if (d < min) throw new ArgumentException($"{d} was lower than the minimum {min}.");
            if (d > max) throw new ArgumentException($"{d} was greater than the maximum {max}.");
            return d;
        }

        public static sbyte PutInRange(this sbyte d, sbyte min, sbyte max)
        {
            if (d < min) return min;
            if (d > max) return max;
            return d;
        }
    }
}
