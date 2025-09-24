using System.Diagnostics.Contracts;

namespace Noggog;

public static class UInt64Ext
{
    [Pure]
    public static bool IsInRange(this ulong d, ulong min, ulong max)
    {
        if (d < min) return false;
        if (d > max) return false;
        return true;
    }

    public static ulong InRange(this ulong d, ulong min, ulong max)
    {
        if (d < min) throw new ArgumentException($"{d} was lower than the minimum {min}.");
        if (d > max) throw new ArgumentException($"{d} was greater than the maximum {max}.");
        return d;
    }

    [Pure]
    public static ulong PutInRange(this ulong d, ulong min, ulong max)
    {
        if (d < min) return min;
        if (d > max) return max;
        return d;
    }
}