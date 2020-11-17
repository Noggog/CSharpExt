using System;
using System.Collections;
using System.Collections.Generic;

namespace Noggog
{
    public struct RangeUInt32 : IEquatable<RangeUInt32>, IEnumerable<uint>
    {
        public readonly uint Min;
        public readonly uint Max;
        public float Average => ((Max - Min) / 2f) + Min;
        public uint Difference => (uint)(this.Max - this.Min);
        public ulong Width => (ulong)(this.Max - this.Min + 1);

        public RangeUInt32(uint val1, uint val2)
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

        public RangeUInt32(uint? min, uint? max)
            : this(min ?? uint.MinValue, max ?? uint.MaxValue)
        {
        }

        public RangeUInt32(uint val)
            : this(val, val)
        {
        }

        public static RangeUInt32 FactoryFromLength(uint loc, uint length)
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
            if (str == null)
            {
                rd = default(RangeUInt32);
                return false;
            }
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
            if (i > this.Max) return false;
            if (i < this.Min) return false;
            return true;
        }

        public uint PutInRange(uint f, bool throwException = true)
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

        public bool IsInRange(RangeUInt32 r)
        {
            if (r.Max > this.Max) return false;
            if (r.Min < this.Min) return false;
            return true;
        }

        public RangeUInt32 PutInRange(RangeUInt32 r, bool throwException = true)
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
                uint min = r.Min < this.Min ? this.Min : r.Min;
                uint max = r.Max < this.Max ? this.Max : r.Max;
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
            for (uint i = this.Min; i <= this.Max; i++)
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
