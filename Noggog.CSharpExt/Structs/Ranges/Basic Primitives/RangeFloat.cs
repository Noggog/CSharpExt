namespace Noggog;

public struct RangeFloat : IEquatable<RangeFloat>
{
    private float _min;
    public float Min
    {
        get => _min;
        set => _min = value;
    }

    private float _max;
    public float Max
    {
        get => _max;
        set => _max = value;
    }
        
    public float Average => ((_max - _min) / 2f) + _min;

    public RangeFloat(float val1, float val2)
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

    public RangeFloat(float val)
        : this(val, val)
    {
    }

    public bool IsInRange(float f)
    {
        if (f > _max) return false;
        if (f < _min) return false;
        return true;
    }

    public float PutInRange(float f, bool throwException = true)
    {
        if (throwException)
        {
            if (f < _min)
            {
                throw new ArgumentException($"Min is out of range: {f} < {_min}");
            }
            if (f > _max)
            {
                throw new ArgumentException($"Max is out of range: {f} < {_max}");
            }
        }
        else
        {
            if (f > _max) return _max;
            if (f < _min) return _min;
        }
        return f;
    }

    public bool IsInRange(RangeFloat r)
    {
        if (r._max > _max) return false;
        if (r._min < _min) return false;
        return true;
    }

    public RangeFloat PutInRange(RangeFloat r, bool throwException = true)
    {
        if (throwException)
        {
            if (r._min < _min)
            {
                throw new ArgumentException($"Min is out of range: {r._min} < {_min}");
            }
            if (r.Max > Max)
            {
                throw new ArgumentException($"Max is out of range: {r.Max} < {Max}");
            }
            return r;
        }
        else
        {
            float min = r._min < _min ? _min : r._min;
            float max = r._max < _max ? _max : r._max;
            return new RangeFloat(min, max);
        }
    }

    public override int GetHashCode() => HashCode.Combine(_min, _max);

    public override bool Equals(object? obj)
    {
        if (obj is not RangeFloat rhs) return false;
        return Equals(rhs);
    }

    public bool Equals(RangeFloat other)
    {
        return _min.EqualsWithin(other._min)
               && _max.EqualsWithin(other._max);
    }

    public override string ToString()
    {
        return _min.EqualsWithin(_max) ? $"({_min})" : $"({_min} - {_max})";
    }

    public string ToString(string format)
    {
        return _min.EqualsWithin(_max) ? $"({_min.ToString(format)})" : $"({_min.ToString(format)} - {_max.ToString(format)})";
    }

    public static bool operator ==(RangeFloat c1, RangeFloat c2)
    {
        return c1.Equals(c2);
    }

    public static bool operator !=(RangeFloat c1, RangeFloat c2)
    {
        return !c1.Equals(c2);
    }
}