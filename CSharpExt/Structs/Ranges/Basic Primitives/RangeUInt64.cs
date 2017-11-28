using System;

namespace Noggog
{
    public struct RangeUInt64 : IEquatable<RangeUInt64>
    {
        public readonly ulong Min;
        public readonly ulong Max;
        public float Average => ((Max - Min) / 2f) + Min;
        public ulong Difference => (ulong)(this.Max - this.Min);
        public ulong Width => (ulong)(this.Max - this.Min + 1);

        public RangeUInt64(ulong val1, ulong val2)
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

        public RangeUInt64(ulong? min, ulong? max)
            : this(min ?? ulong.MinValue, max ?? ulong.MaxValue)
        {
        }

        public RangeUInt64(ulong val)
            : this(val, val)
        {
        }

        public static RangeUInt64 Parse(string str)
        {
            if (!TryParse(str, out RangeUInt64 rd))
            {
                return default(RangeUInt64);
            }
            return rd;
        }

        public static bool TryParse(string str, out RangeUInt64 rd)
        {
            if (str == null)
            {
                rd = default(RangeUInt64);
                return false;
            }
            string[] split = str.Split('-');
            if (split.Length != 2)
            {
                rd = default(RangeUInt64);
                return false;
            }
            rd = new RangeUInt64(
                ulong.Parse(split[0]),
                ulong.Parse(split[1]));
            return true;
        }

        public bool IsInRange(ulong i)
        {
            if (i > this.Max) return false;
            if (i < this.Min) return false;
            return true;
        }

        public ulong PutInRange(ulong f, bool throwException = true)
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

        public bool IsInRange(RangeUInt64 r)
        {
            if (r.Max > this.Max) return false;
            if (r.Min < this.Min) return false;
            return true;
        }

        public RangeUInt64 PutInRange(RangeUInt64 r, bool throwException = true)
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
                ulong min = r.Min < this.Min ? this.Min : r.Min;
                ulong max = r.Max < this.Max ? this.Max : r.Max;
                return new RangeUInt64(min, max);
            }
        }
        
        public override bool Equals(object obj)
        {
            if (!(obj is RangeUInt64 rhs)) return false;
            return Equals(rhs);
        }

        public bool Equals(RangeUInt64 other)
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
            return Min == Max ? $"({prefix}{Min.ToString(format)})" : $"({prefix}{Min.ToString(format)} - {prefix}{Max.ToString(format)})";
        }

        public static bool operator ==(RangeUInt64 c1, RangeUInt64 c2)
        {
            return c1.Equals(c2);
        }

        public static bool operator !=(RangeUInt64 c1, RangeUInt64 c2)
        {
            return !c1.Equals(c2);
        }
    }
}
