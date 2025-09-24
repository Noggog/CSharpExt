using System.Diagnostics.Contracts;

namespace Noggog;

public static class Int32Ext
{
    [Pure]
    public static int Clamp(this int a, int min, int max)
    {
        return Math.Min(Math.Max(a, min), max);
    }

    [Pure]
    public static bool IsInRange(this int d, int min, int max)
    {
        if (d < min) return false;
        if (d > max) return false;
        return true;
    }

    public static int InRange(this int d, int min, int max)
    {
        if (d < min) throw new ArgumentException($"{d} was lower than the minimum {min}.");
        if (d > max) throw new ArgumentException($"{d} was greater than the maximum {max}.");
        return d;
    }

    [Pure]
    public static int PutInRange(this int d, int min, int max)
    {
        if (d < min) return min;
        if (d > max) return max;
        return d;
    }
}