using System;
using System.Collections;
using System.Collections.Generic;

namespace Noggog
{
    public struct RangeUInt32 : IEquatable<RangeUInt32>, IEnumerable<uint>
    {
        private uint _min;
        public uint Min
        {
            get => _min;
            set => _min = value;
        }

        private uint _max;
        public uint Max
        {
            get => _max;
            set => _max = value;
        }
        
        public float Average => ((_max - _min) / 2f) + _min;
        public uint Difference => (uint)(_max - _min);
        public ulong Width => (ulong)(_max - _min + 1);

        public RangeUInt32(uint val1, uint val2)
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

        public RangeUInt32(uint? min, uint? max)
            : this(min ?? uint.MinValue, max ?? uint.MaxValue)
        {
        }

        public RangeUInt32(uint val)
            : this(val, val)
        {
        }

        public static RangeUInt32 FromLength(uint loc, uint length)
        {
            return new RangeUInt32(
                loc,
                loc + length - 1);
        }

        public static RangeUInt32 Parse(string str)
        {
            if (!TryParse(str, out RangeUInt32 rd))
            {
                return default(RangeUInt32);
            }
            return rd;
        }

        public static bool TryParse(string str, out RangeUInt32 rd)
        {
            string[] split = str.Split('-');
            if (split.Length != 2)
            {
                rd = default(RangeUInt32);
                return false;
            }
            rd = new RangeUInt32(
                uint.Parse(split[0]),
                uint.Parse(split[1]));
            return true;
        }

        public bool IsInRange(uint i)
        {
            if (i > _max) return false;
            if (i < _min) return false;
            return true;
        }

        public uint PutInRange(uint f, bool throwException = true)
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

        public bool IsInRange(RangeUInt32 r)
        {
            if (r._max > _max) return false;
            if (r._min < _min) return false;
            return true;
        }

        public RangeUInt32 PutInRange(RangeUInt32 r, bool throwException = true)
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
                uint min = r._min < _min ? _min : r._min;
                uint max = r._max < _max ? _max : r._max;
                return new RangeUInt32(min, max);
            }
        }
        
        public override bool Equals(object? obj)
        {
            if (obj is not RangeUInt32 rhs) return false;
            return Equals(rhs);
        }

        public bool Equals(RangeUInt32 other)
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

        public static bool operator ==(RangeUInt32 c1, RangeUInt32 c2)
        {
            return c1.Equals(c2);
        }

        public static bool operator !=(RangeUInt32 c1, RangeUInt32 c2)
        {
            return !c1.Equals(c2);
        }

        public IEnumerator<uint> GetEnumerator()
        {
            for (uint i = _min; i <= _max; i++)
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
