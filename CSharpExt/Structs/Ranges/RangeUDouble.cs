using System;

namespace Noggog
{
    public struct RangeUDouble : IEquatable<RangeUDouble>
    {
        public readonly UDouble Min;
        public float FMin => (float)Min;

        public readonly UDouble Max;
        public float FMax => (float)Max;
        public UDouble Average => ((Max - Min) / 2f) + Min;

        public RangeUDouble(UDouble val1, UDouble val2)
        {
            if (val1 > val2)
            {
                this.Max = val1;
                this.Min = val2;
            }
            else
            {
                this.Min = val1;
                this.Max = val2;
            }
        }

        public RangeUDouble(UDouble? min, UDouble? max)
            : this(min ?? UDouble.MinValue, max ?? UDouble.MaxValue)
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
            if (str == null)
            {
                rd = default(RangeUDouble);
                return false;
            }
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
            if (f > this.Max) return false;
            if (f < this.Min) return false;
            return true;
        }

        public UDouble PutInRange(UDouble f, bool throwException = true)
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

        public bool IsInRange(RangeUDouble r)
        {
            if (r.Max > this.Max) return false;
            if (r.Min < this.Min) return false;
            return true;
        }

        public RangeUDouble PutInRange(RangeUDouble r, bool throwException = true)
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
                UDouble min = r.Min < this.Min ? this.Min : r.Min;
                UDouble max = r.Max < this.Max ? this.Max : r.Max;
                return new RangeUDouble(min, max);
            }
        }

        public override bool Equals(object obj)
        {
            if (!(obj is RangeUDouble rhs)) return false;
            return Equals(rhs);
        }

        public bool Equals(RangeUDouble other)
        {
            return this.Min.EqualsWithin(other.Min)
                && this.Max.EqualsWithin(other.Max);
        }

        public override int GetHashCode()
        {
            return HashHelper.GetHashCode(Min, Max);
        }

        public override string ToString()
        {
            return Min == Max ? $"({Min.ToString()})" : $"({Min} - {Max})";
        }

        public static RangeUDouble operator -(RangeUDouble r1, RangeUDouble r2)
        {
            return new RangeUDouble(r1.Min - r2.Min, r1.Max - r2.Max);
        }

        public static RangeUDouble operator +(RangeUDouble r1, RangeUDouble r2)
        {
            return new RangeUDouble(r1.Min + r2.Min, r1.Max + r2.Max);
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
