using System;

namespace Noggog
{
    public struct RangeDouble : IEquatable<RangeDouble>
    {
        public readonly double Min;
        public float FMin => (float)Min;

        public readonly double Max;
        public float FMax => (float)Max;
        public double Average => ((Max - Min) / 2f) + Min;

        public RangeDouble(double val1, double val2)
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

        public RangeDouble(double? min, double? max)
            : this(min ?? double.MinValue, max ?? double.MaxValue)
        {
        }

        public RangeDouble(double val)
            : this(val, val)
        {
        }

        public static RangeDouble Parse(string str)
        {
            if (!TryParse(str, out RangeDouble rd))
            {
                return default(RangeDouble);
            }
            return rd;
        }

        public static bool TryParse(string str, out RangeDouble rd)
        {
            if (str == null)
            {
                rd = default(RangeDouble);
                return false;
            }
            string[] split = str.Split('-');
            if (split.Length != 2)
            {
                rd = default(RangeDouble);
                return false;
            }
            rd = new RangeDouble(
                double.Parse(split[0]),
                double.Parse(split[1]));
            return true;
        }

        public bool IsInRange(double f)
        {
            if (f > this.Max) return false;
            if (f < this.Min) return false;
            return true;
        }

        public double PutInRange(double f, bool throwException = true)
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

        public bool IsInRange(RangeDouble r)
        {
            if (r.Max > this.Max) return false;
            if (r.Min < this.Min) return false;
            return true;
        }

        public RangeDouble PutInRange(RangeDouble r, bool throwException = true)
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
                double min = r.Min < this.Min ? this.Min : r.Min;
                double max = r.Max < this.Max ? this.Max : r.Max;
                return new RangeDouble(min, max);
            }
        }

        public override bool Equals(object obj)
        {
            if (!(obj is RangeDouble rhs)) return false;
            return Equals(rhs);
        }

        public bool Equals(RangeDouble other)
        {
            return this.Min.EqualsWithin(other.Min)
                && this.Max.EqualsWithin(other.Max);
        }

        public override int GetHashCode() => HashCode.Combine(Min, Max);

        public override string ToString()
        {
            return Min.EqualsWithin(Max) ? $"({Min.ToString()})" : $"({Min} - {Max})";
        }

        public string ToString(string format)
        {
            return Min == Max ? $"({Min.ToString(format)})" : $"({Min.ToString(format)} - {Max.ToString(format)})";
        }

        public static RangeDouble operator -(RangeDouble r1, RangeDouble r2)
        {
            return new RangeDouble(r1.Min - r2.Min, r1.Max - r2.Max);
        }

        public static RangeDouble operator +(RangeDouble r1, RangeDouble r2)
        {
            return new RangeDouble(r1.Min + r2.Min, r1.Max + r2.Max);
        }

        public static bool operator ==(RangeDouble c1, RangeDouble c2)
        {
            return c1.Equals(c2);
        }

        public static bool operator !=(RangeDouble c1, RangeDouble c2)
        {
            return !c1.Equals(c2);
        }
    }
}
