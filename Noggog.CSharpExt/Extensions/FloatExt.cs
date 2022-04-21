namespace Noggog;

public static class FloatExt
{
    public static int ToInt(this float a)
    {
        return Convert.ToInt32(a);
    }

    public static bool EqualsWithin(this float a, float b, float within = 0.000000001f)
    {
        if (Math.Abs(a - b) < within) return true;
        if (float.IsInfinity(a) && float.IsInfinity(b)) return true;
        if (float.IsNaN(a) && float.IsNaN(b)) return true;
        return false;
    }

    public static bool EqualsWithin(this float? a, float? b, float within = 0.000000001f)
    {
        if (a.HasValue && b.HasValue)
        {
            if (Math.Abs(a.Value - b.Value) < within) return true;
            if (float.IsInfinity(a.Value) && float.IsInfinity(b.Value)) return true;
            if (float.IsNaN(a.Value) && float.IsNaN(b.Value)) return true;
            return false;
        }
        else
        {
            return !a.HasValue && !b.HasValue;
        }
    }

    public static int Round(this float a)
    {
        return (int)Math.Round(a);
    }

    public static bool IsReal(this float f)
    {
        return !float.IsInfinity(f)
               && !float.IsNaN(f);
    }

    public static float Average(this float a, float b)
    {
        float less, more;
        if (a < b)
        {
            less = a;
            more = b;
        }
        else
        {
            more = a;
            less = b;
        }
        var diff = more - less;
        diff /= 2;
        return diff + less;
    }

    public static bool IsInRange(this float d, float min, float max)
    {
        if (d < min) return false;
        if (d > max) return false;
        return true;
    }

    public static float InRange(this float d, float min, float max)
    {
        if (d < min) throw new ArgumentException($"{d} was lower than the minimum {min}.");
        if (d > max) throw new ArgumentException($"{d} was greater than the maximum {max}.");
        return d;
    }

    public static float PutInRange(this float d, float min, float max)
    {
        if (d < min) return min;
        if (d > max) return max;
        return d;
    }
}