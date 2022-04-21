using System;
using System.Collections;
using System.Collections.Generic;

namespace Noggog
{
    public struct RangeInt16 : IEquatable<RangeInt16>, IEnumerable<short>
    {
        private short _min;
        public short Min
        {
            get => _min;
            set => _min = value;
        }

        private short _max;
        public short Max
        {
            get => _max;
            set => _max = value;
        }
        
        public float Average => ((_max - _min) / 2f) + _min;
        public short Difference => (short)(_max - _min);
        public ushort Width => (ushort)(_max - _min + 1);

        public RangeInt16(short val1, short val2)
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

        public RangeInt16(short? min, short? max)
            : this(min ?? short.MinValue, max ?? short.MaxValue)
        {
        }

        public RangeInt16(short val)
            : this(val, val)
        {
        }

        public static RangeInt32 FromLength(short loc, short length)
        {
            return new RangeInt32(
                loc,
                loc + length - 1);
        }

        public static RangeInt16 Parse(string str)
        {
            if (!TryParse(str, out RangeInt16 rd))
            {
                return default(RangeInt16);
            }
            return rd;
        }

        public static bool TryParse(string str, out RangeInt16 rd)
        {
            string[] split = str.Split('-');
            if (split.Length != 2)
            {
                rd = default(RangeInt16);
                return false;
            }
            rd = new RangeInt16(
                short.Parse(split[0]),
                short.Parse(split[1]));
            return true;
        }

        public bool IsInRange(short i)
        {
            if (i > _max) return false;
            if (i < _min) return false;
            return true;
        }

        public short PutInRange(short f, bool throwException = true)
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

        public bool IsInRange(RangeInt16 r)
        {
            if (r._max > _max) return false;
            if (r._min < _min) return false;
            return true;
        }

        public RangeInt16 PutInRange(RangeInt16 r, bool throwException = true)
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
                short min = r._min < _min ? _min : r._min;
                short max = r._max < _max ? _max : r._max;
                return new RangeInt16(min, max);
            }
        }

        public override bool Equals(object? obj)
        {
            if (obj is not RangeInt16 rhs) return false;
            return Equals(rhs);
        }

        public bool Equals(RangeInt16 other)
        {
            return _min == other._min
                && _max == other._max;
        }

        public override int GetHashCode() => HashCode.Combine(_min, _max);

        public override string ToString()
        {
            return _min == _max ? $"({_min})" : $"({_min} - {_max})";
        }

        public string ToString(string format)
        {
            string prefix;
            if (format.Contains("X"))
            {
                prefix = "0x";
            }
            else
            {
                prefix = string.Empty;
            }
            return _min == _max ? $"({prefix}{_min.ToString(format)})" : $"({prefix}{_min.ToString(format)} - {prefix}{_max.ToString(format)})";
        }

        public static bool operator ==(RangeInt16 c1, RangeInt16 c2)
        {
            return c1.Equals(c2);
        }

        public static bool operator !=(RangeInt16 c1, RangeInt16 c2)
        {
            return !c1.Equals(c2);
        }

        public IEnumerator<short> GetEnumerator()
        {
            for (short i = _min; i <= _max; i++)
            {
                yield return i;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
