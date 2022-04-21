using System;

namespace Noggog
{
    public struct RangeUDouble : IEquatable<RangeUDouble>
    {
        private UDouble _min;
        public UDouble Min
        {
            get => _min;
            set => _min = value;
        }

        private UDouble _max;
        public UDouble Max
        {
            get => _max;
            set => _max = value;
        }
        
        public float FMin => (float)Min;
        public float FMax => (float)Max;
        public UDouble Average => ((Max - Min) / 2f) + Min;

        public RangeUDouble(UDouble val1, UDouble val2)
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

        public RangeUDouble(UDouble? min, UDouble? max)
            : this(min ?? UDouble.MinValue, max ?? UDouble.MaxValue)
        {
        }

        public RangeUDouble(UDouble val)
            : this(val, val)
        {
        }

        public static RangeUDouble Parse(string str)
        {
            if (!TryParse(str, out RangeUDouble rd))
            {
                return default(RangeUDouble);
            }
            return rd;
        }

        public static bool TryParse(string str, out RangeUDouble rd)
        {
            string[] split = str.Split('-');
            if (split.Length != 2)
            {
                rd = default(RangeUDouble);
                return false;
            }
            rd = new RangeUDouble(
                UDouble.Parse(split[0]),
                UDouble.Parse(split[1]));
            return true;
        }

        public bool IsInRange(UDouble f)
        {
            if (f > _max) return false;
            if (f < _min) return false;
            return true;
        }

        public UDouble PutInRange(UDouble f, bool throwException = true)
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

        public bool IsInRange(RangeUDouble r)
        {
            if (r._max > _max) return false;
            if (r._min < _min) return false;
            return true;
        }

        public RangeUDouble PutInRange(RangeUDouble r, bool throwException = true)
        {
            if (throwException)
            {
                if (r._min < _min)
                {
                    throw new ArgumentException($"Min is out of range: {r._min} < {_min}");
                }
                if (r._max > _max)
                {
                    throw new ArgumentException($"Max is out of range: {r._max} < {_max}");
                }
                return r;
            }
            else
            {
                UDouble min = r._min < _min ? _min : r._min;
                UDouble max = r._max < _max ? _max : r._max;
                return new RangeUDouble(min, max);
            }
        }

        public override bool Equals(object? obj)
        {
            if (obj is not RangeUDouble rhs) return false;
            return Equals(rhs);
        }

        public bool Equals(RangeUDouble other)
        {
            return _min.EqualsWithin(other._min)
                && _max.EqualsWithin(other._max);
        }

        public override int GetHashCode() => HashCode.Combine(_min, _max);

        public override string ToString()
        {
            return _min.EqualsWithin(_max) ? $"({_min})" : $"({_min} - {_max})";
        }

        public string ToString(string format)
        {
            return _min.EqualsWithin(_max) ? $"({_min.ToString(format)})" : $"({_min.ToString(format)} - {_max.ToString(format)})";
        }

        public static RangeUDouble operator -(RangeUDouble r1, RangeUDouble r2)
        {
            return new RangeUDouble(r1._min - r2._min, r1._max - r2._max);
        }

        public static RangeUDouble operator +(RangeUDouble r1, RangeUDouble r2)
        {
            return new RangeUDouble(r1._min + r2._min, r1._max + r2._max);
        }

        public static bool operator ==(RangeUDouble c1, RangeUDouble c2)
        {
            return c1.Equals(c2);
        }

        public static bool operator !=(RangeUDouble c1, RangeUDouble c2)
        {
            return !c1.Equals(c2);
        }
    }
}
