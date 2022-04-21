using System;
using System.Collections;
using System.Collections.Generic;

namespace Noggog
{
    public struct RangeInt64 : IEquatable<RangeInt64>, IEnumerable<long>
    {
        private long _min;
        public long Min
        {
            get => _min;
            set => _min = value;
        }

        private long _max;
        public long Max
        {
            get => _max;
            set => _max = value;
        }
        
        public float Average => ((_max - _min) / 2f) + _min;
        public long Difference => _max - _min;
        public long Width => Difference + 1;

        public RangeInt64(long val1, long val2)
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

        public RangeInt64(long? min, long? max)
            : this(min ?? long.MinValue, max ?? long.MaxValue)
        {
        }

        public RangeInt64(long val)
            : this(val, val)
        {
        }

        public static RangeInt64 FromLength(long loc, long length)
        {
            return new RangeInt64(
                loc,
                loc + length - 1);
        }

        public static RangeInt64 Parse(string str)
        {
            if (!TryParse(str, out RangeInt64 rd))
            {
                return default(RangeInt64);
            }
            return rd;
        }

        public static bool TryParse(string str, out RangeInt64 rd)
        {
            string[] split = str.Split('-');
            if (split.Length != 2)
            {
                rd = default(RangeInt64);
                return false;
            }
            rd = new RangeInt64(
                long.Parse(split[0]),
                long.Parse(split[1]));
            return true;
        }

        public bool IsInRange(long i)
        {
            if (i > _max) return false;
            if (i < _min) return false;
            return true;
        }

        // ToDo
        // Make more generic to be usable for all ranges
        public bool Collides(RangeInt64 rhs)
        {
            if (_min < rhs._min)
            {
                if (_max < rhs._min) return false;
                return true;
            }
            else
            {
                if (_min > rhs._max) return false;
                return true;
            }
        }

        public long PutInRange(long f, bool throwException = true)
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

        public bool IsInRange(RangeInt64 r)
        {
            if (r._max > _max) return false;
            if (r._min < _min) return false;
            return true;
        }

        public RangeInt64 PutInRange(RangeInt64 r, bool throwException = true)
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
                long min = r._min < _min ? _min : r._min;
                long max = r._max < _max ? _max : r._max;
                return new RangeInt64(min, max);
            }
        }

        public override bool Equals(object? obj)
        {
            if (obj is not RangeInt64 rhs) return false;
            return Equals(rhs);
        }

        public bool Equals(RangeInt64 other)
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

        public static bool operator ==(RangeInt64 c1, RangeInt64 c2)
        {
            return c1.Equals(c2);
        }

        public static bool operator !=(RangeInt64 c1, RangeInt64 c2)
        {
            return !c1.Equals(c2);
        }

        public static IEnumerable<RangeInt64> ConstructRanges<T>(
            IEnumerable<KeyValuePair<long, T>> items,
            Func<T, bool> eval)
        {
            bool inRange = false;
            long startRange = 0;
            foreach (var item in items)
            {
                if (eval(item.Value))
                {
                    if (!inRange)
                    {
                        startRange = item.Key;
                        inRange = true;
                    }
                }
                else
                {
                    if (inRange)
                    {
                        yield return new RangeInt64(startRange, item.Key - 1);
                        inRange = false;
                    }
                }
            }
        }

        public IEnumerator<long> GetEnumerator()
        {
            for (long i = _min; i <= _max; i++)
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
