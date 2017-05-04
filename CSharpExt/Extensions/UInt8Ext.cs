using System;

namespace System
{
    public static class UInt8Ext
    {
        public static bool IsInRange(this byte d, byte min, byte max)
        {
            if (d < min) return false;
            if (d > max) return false;
            return true;
        }

        public static byte InRange(this byte d, byte min, byte max)
        {
            if (d < min) throw new ArgumentException($"{d} was lower than the minimum {min}.");
            if (d > max) throw new ArgumentException($"{d} was greater than the maximum {max}.");
            return d;
        }

        public static byte PutInRange(this byte d, byte min, byte max)
        {
            if (d < min) return min;
            if (d > max) return max;
            return d;
        }
    }
}
