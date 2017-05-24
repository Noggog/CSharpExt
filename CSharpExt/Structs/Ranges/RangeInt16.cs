using System;

namespace Noggog
{
    public struct RangeInt16 : IEquatable<RangeInt16>
    {
        public readonly short Min;
        public readonly short Max;
        public float Average => ((Max - Min) / 2f) + Min;
        public short Difference => (short)(this.Max - this.Min);

        public RangeInt16(short val1, short val2)
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

        public RangeInt16(short? min, short? max)
            : this(min ?? short.MinValue, max ?? short.MaxValue)
        {
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
            if (str == null)
            {
                rd = default(RangeInt16);
                return false;
            }
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
            if (i > this.Max) return false;
            if (i < this.Min) return false;
            return true;
        }

        public short PutInRange(short f, bool throwException = true)
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

        public bool IsInRange(RangeInt16 r)
        {
            if (r.Max > this.Max) return false;
            if (r.Min < this.Min) return false;
            return true;
        }

        public RangeInt16 PutInRange(RangeInt16 r, bool throwException = true)
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
                short min = r.Min < this.Min ? this.Min : r.Min;
                short max = r.Max < this.Max ? this.Max : r.Max;
                return new RangeInt16(min, max);
            }
        }

        public override bool Equals(object obj)
        {
            if (!(obj is RangeInt16 rhs)) return false;
            return Equals(rhs);
        }

        public bool Equals(RangeInt16 other)
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

        public static bool operator ==(RangeInt16 c1, RangeInt16 c2)
        {
            return c1.Equals(c2);
        }

        public static bool operator !=(RangeInt16 c1, RangeInt16 c2)
        {
            return !c1.Equals(c2);
        }
    }
}
