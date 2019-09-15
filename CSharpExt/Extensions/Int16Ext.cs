using System;

namespace Noggog
{
    public static class Int16Ext
    {
        public static bool IsInRange(this short d, short min, short max)
        {
            if (d < min) return false;
            if (d > max) return false;
            return true;
        }

        public static short InRange(this short d, short min, short max)
        {
            if (d < min) throw new ArgumentException($"{d} was lower than the minimum {min}.");
            if (d > max) throw new ArgumentException($"{d} was greater than the maximum {max}.");
            return d;
        }

        public static short PutInRange(this short d, short min, short max)
        {
            if (d < min) return min;
            if (d > max) return max;
            return d;
        }
    }
}
