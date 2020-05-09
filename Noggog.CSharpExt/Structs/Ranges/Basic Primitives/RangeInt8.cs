using System;
using System.Collections;
using System.Collections.Generic;

namespace Noggog
{
    public struct RangeInt8 : IEquatable<RangeInt8>, IEnumerable<sbyte>
    {
        public readonly sbyte Min;
        public readonly sbyte Max;
        public float Average => ((Max - Min) / 2f) + Min;
        public sbyte Difference => (sbyte)(this.Max - this.Min);
        public ushort Width => (ushort)(this.Max - this.Min + 1);

        public RangeInt8(sbyte val1, sbyte val2)
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

        public RangeInt8(sbyte? min, sbyte? max)
            : this(min ?? sbyte.MinValue, max ?? sbyte.MaxValue)
        {
        }

        public RangeInt8(sbyte val)
            : this(val, val)
        {
        }

        public static RangeInt8 FactoryFromLength(sbyte loc, sbyte length)
        {
            return new RangeInt8(
                loc,
                (sbyte)(loc + length - 1));
        }

        public static RangeInt8 Parse(string str)
        {
            if (!TryParse(str, out RangeInt8 rd))
            {
                return default(RangeInt8);
            }
            return rd;
        }

        public static bool TryParse(string str, out RangeInt8 rd)
        {
            if (str == null)
            {
                rd = default(RangeInt8);
                return false;
            }
            string[] split = str.Split('-');
            if (split.Length != 2)
            {
                rd = default(RangeInt8);
                return false;
            }
            rd = new RangeInt8(
                sbyte.Parse(split[0]),
                sbyte.Parse(split[1]));
            return true;
        }

        public bool IsInRange(sbyte i)
        {
            if (i > this.Max) return false;
            if (i < this.Min) return false;
            return true;
        }

        public sbyte PutInRange(sbyte f, bool throwException = true)
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

        public bool IsInRange(RangeInt8 r)
        {
            if (r.Max > this.Max) return false;
            if (r.Min < this.Min) return false;
            return true;
        }

        public RangeInt8 PutInRange(RangeInt8 r, bool throwException = true)
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
                sbyte min = r.Min < this.Min ? this.Min : r.Min;
                sbyte max = r.Max < this.Max ? this.Max : r.Max;
                return new RangeInt8(min, max);
            }
        }
        
        public override bool Equals(object obj)
        {
            if (!(obj is RangeInt8 rhs)) return false;
            return Equals(rhs);
        }

        public bool Equals(RangeInt8 other)
        {
            return this.Min == other.Min
                && this.Max == other.Max;
        }

        public override int GetHashCode() => HashCode.Combine(Min, Max);

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

        public static bool operator ==(RangeInt8 c1, RangeInt8 c2)
        {
            return c1.Equals(c2);
        }

        public static bool operator !=(RangeInt8 c1, RangeInt8 c2)
        {
            return !c1.Equals(c2);
        }

        public IEnumerator<sbyte> GetEnumerator()
        {
            for (sbyte i = this.Min; i <= this.Max; i++)
            {
                yield return i;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
