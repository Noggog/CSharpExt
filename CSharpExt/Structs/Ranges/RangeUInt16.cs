using System;

namespace Noggog
{
    public struct RangeUInt16 : IEquatable<RangeUInt16>
    {
        public readonly ushort Min;
        public readonly ushort Max;
        public float Average => ((Max - Min) / 2f) + Min;
        public ushort Difference => (ushort)(this.Max - this.Min);

        public RangeUInt16(ushort val1, ushort val2)
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

        public RangeUInt16(ushort? min, ushort? max)
            : this(min ?? ushort.MinValue, max ?? ushort.MaxValue)
        {
        }

        public static RangeUInt16 Parse(string str)
        {
            if (!TryParse(str, out RangeUInt16 rd))
            {
                return default(RangeUInt16);
            }
            return rd;
        }

        public static bool TryParse(string str, out RangeUInt16 rd)
        {
            if (str == null)
            {
                rd = default(RangeUInt16);
                return false;
            }
            string[] split = str.Split('-');
            if (split.Length != 2)
            {
                rd = default(RangeUInt16);
                return false;
            }
            rd = new RangeUInt16(
                ushort.Parse(split[0]),
                ushort.Parse(split[1]));
            return true;
        }

        public bool IsInRange(ushort i)
        {
            if (i > this.Max) return false;
            if (i < this.Min) return false;
            return true;
        }

        public ushort PutInRange(ushort f, bool throwException = true)
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

        public bool IsInRange(RangeUInt16 r)
        {
            if (r.Max > this.Max) return false;
            if (r.Min < this.Min) return false;
            return true;
        }

        public RangeUInt16 PutInRange(RangeUInt16 r, bool throwException = true)
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
                ushort min = r.Min < this.Min ? this.Min : r.Min;
                ushort max = r.Max < this.Max ? this.Max : r.Max;
                return new RangeUInt16(min, max);
            }
        }
        
        public override bool Equals(object obj)
        {
            if (!(obj is RangeUInt16 rhs)) return false;
            return Equals(rhs);
        }

        public bool Equals(RangeUInt16 other)
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
    }
}
