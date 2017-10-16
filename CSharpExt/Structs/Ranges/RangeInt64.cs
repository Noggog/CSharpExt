using System;
using System.Collections.Generic;

namespace Noggog
{
    public struct RangeInt64 : IEquatable<RangeInt64>
    {
        public readonly long Min;
        public readonly long Max;
        public float Average => ((Max - Min) / 2f) + Min;
        public long Difference => this.Max - this.Min;

        public RangeInt64(long val1, long val2)
        {
            if (val1 > val2)
            {
                Max = val1;
                Min = val2;
            }
            else
            {
                Min = val1;
                Max = val2;
            }
        }

        public RangeInt64(long? min, long? max)
            : this(min ?? long.MinValue, max ?? long.MaxValue)
        {
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
            if (str == null)
            {
                rd = default(RangeInt64);
                return false;
            }
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
            if (i > this.Max) return false;
            if (i < this.Min) return false;
            return true;
        }

        public long PutInRange(long f, bool throwException = true)
        {
            if (throwException)
            {
                if (f < this.Min)
                {
                    throw new ArgumentException($"Min is out of range: {f} < {this.Min}");
                }
                if (f > this.Max)
                {
                    throw new ArgumentException($"Min is out of range: {f} < {this.Max}");
                }
            }
            else
            {
                if (f > this.Max) return this.Max;
                if (f < this.Min) return this.Min;
            }
            return f;
        }

        public bool IsInRange(RangeInt64 r)
        {
            if (r.Max > this.Max) return false;
            if (r.Min < this.Min) return false;
            return true;
        }

        public RangeInt64 PutInRange(RangeInt64 r, bool throwException = true)
        {
            if (throwException)
            {
                if (r.Min < this.Min)
                {
                    throw new ArgumentException($"Min is out of range: {r.Min} < {this.Min}");
                }
                if (r.Max > this.Max)
                {
                    throw new ArgumentException($"Min is out of range: {r.Max} < {this.Max}");
                }
                return r;
            }
            else
            {
                long min = r.Min < this.Min ? this.Min : r.Min;
                long max = r.Max < this.Max ? this.Max : r.Max;
                return new RangeInt64(min, max);
            }
        }
        
        public override bool Equals(object obj)
        {
            if (!(obj is RangeInt64 rhs)) return false;
            return Equals(rhs);
        }

        public bool Equals(RangeInt64 other)
        {
            return this.Min == other.Min
                && this.Max == other.Max;
        }

        public override int GetHashCode()
        {
            return HashHelper.GetHashCode(Min, Max);
        }

        public override string ToString()
        {
            return Min == Max ? $"({Min.ToString()})" : $"({Min} - {Max})";
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
    }
}
