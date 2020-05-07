using System;

namespace Noggog
{
    public static class UInt16Ext
    {
        public static bool IsInRange(this ushort d, ushort min, ushort max)
        {
            if (d < min) return false;
            if (d > max) return false;
            return true;
        }

        public static ushort InRange(this ushort d, ushort min, ushort max)
        {
            if (d < min) throw new ArgumentException($"{d} was lower than the minimum {min}.");
            if (d > max) throw new ArgumentException($"{d} was greater than the maximum {max}.");
            return d;
        }

        public static ushort PutInRange(this ushort d, ushort min, ushort max)
        {
            if (d < min) return min;
            if (d > max) return max;
            return d;
        }
    }
}
