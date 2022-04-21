namespace Noggog;

public struct RangeDouble : IEquatable<RangeDouble>
{
    private double _min;
    public double Min
    {
        get => _min;
        set => _min = value;
    }

    private double _max;
    public double Max
    {
        get => _max;
        set => _max = value;
    }
        
    public float FMin => (float)Min;
    public float FMax => (float)Max;
    public double Average => ((Max - Min) / 2f) + Min;

    public RangeDouble(double val1, double val2)
    {
        if (val1 > val2)
        {
            _max = val1;
            _min = val2;
        }
        else
        {
            _min = val1;
            _max = val2;
        }
    }

    public RangeDouble(double? min, double? max)
        : this(min ?? double.MinValue, max ?? double.MaxValue)
    {
    }

    public RangeDouble(double val)
        : this(val, val)
    {
    }

    public static RangeDouble Parse(string str)
    {
        if (!TryParse(str, out RangeDouble rd))
        {
            return default(RangeDouble);
        }
        return rd;
    }

    public static bool TryParse(string str, out RangeDouble rd)
    {
        string[] split = str.Split('-');
        if (split.Length != 2)
        {
            rd = default(RangeDouble);
            return false;
        }
        rd = new RangeDouble(
            double.Parse(split[0]),
            double.Parse(split[1]));
        return true;
    }

    public bool IsInRange(double f)
    {
        if (f > Max) return false;
        if (f < Min) return false;
        return true;
    }

    public double PutInRange(double f, bool throwException = true)
    {
        if (throwException)
        {
            if (f < Min)
            {
                throw new ArgumentException($"Min is out of range: {f} < {Min}");
            }
            if (f > Max)
            {
                throw new ArgumentException($"Max is out of range: {f} < {Max}");
            }
        }
        else
        {
            if (f > Max) return Max;
            if (f < Min) return Min;
        }
        return f;
    }

    public bool IsInRange(RangeDouble r)
    {
        if (r.Max > Max) return false;
        if (r.Min < Min) return false;
        return true;
    }

    public RangeDouble PutInRange(RangeDouble r, bool throwException = true)
    {
        if (throwException)
        {
            if (r.Min < Min)
            {
                throw new ArgumentException($"Min is out of range: {r.Min} < {Min}");
            }
            if (r.Max > Max)
            {
                throw new ArgumentException($"Max is out of range: {r.Max} < {Max}");
            }
            return r;
        }
        else
        {
            double min = r.Min < Min ? Min : r.Min;
            double max = r.Max < Max ? Max : r.Max;
            return new RangeDouble(min, max);
        }
    }

    public override bool Equals(object? obj)
    {
        if (obj is not RangeDouble rhs) return false;
        return Equals(rhs);
    }

    public bool Equals(RangeDouble other)
    {
        return Min.EqualsWithin(other.Min)
               && Max.EqualsWithin(other.Max);
    }

    public override int GetHashCode() => HashCode.Combine(Min, Max);

    public override string ToString()
    {
        return Min.EqualsWithin(Max) ? $"({Min})" : $"({Min} - {Max})";
    }

    public string ToString(string format)
    {
        return Min.EqualsWithin(Max) ? $"({Min.ToString(format)})" : $"({Min.ToString(format)} - {Max.ToString(format)})";
    }

    public static RangeDouble operator -(RangeDouble r1, RangeDouble r2)
    {
        return new RangeDouble(r1.Min - r2.Min, r1.Max - r2.Max);
    }

    public static RangeDouble operator +(RangeDouble r1, RangeDouble r2)
    {
        return new RangeDouble(r1.Min + r2.Min, r1.Max + r2.Max);
    }

    public static bool operator ==(RangeDouble c1, RangeDouble c2)
    {
        return c1.Equals(c2);
    }

    public static bool operator !=(RangeDouble c1, RangeDouble c2)
    {
        return !c1.Equals(c2);
    }
}