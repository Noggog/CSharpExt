using System.Diagnostics.Contracts;

namespace Noggog;

public static class UInt32Ext
{
    [Pure]
    public static bool IsInRange(this uint d, uint min, uint max)
    {
        if (d < min) return false;
        if (d > max) return false;
        return true;
    }

    public static uint InRange(this uint d, uint min, uint max)
    {
        if (d < min) throw new ArgumentException($"{d} was lower than the minimum {min}.");
        if (d > max) throw new ArgumentException($"{d} was greater than the maximum {max}.");
        return d;
    }

    [Pure]
    public static uint PutInRange(this uint d, uint min, uint max)
    {
        if (d < min) return min;
        if (d > max) return max;
        return d;
    }
}