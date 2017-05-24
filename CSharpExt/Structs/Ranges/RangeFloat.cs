using System;

namespace Noggog
{
    public struct RangeFloat : IEquatable<RangeFloat>
    {
        public readonly float Min;
        public readonly float Max;
        public float Average => ((Max - Min) / 2f) + Min;

        public RangeFloat(float val1, float val2)
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
        
        public bool IsInRange(float f)
        {
            if (f > this.Max) return false;
            if (f < this.Min) return false;
            return true;
        }

        public float PutInRange(float f, bool throwException = true)
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

        public bool IsInRange(RangeFloat r)
        {
            if (r.Max > this.Max) return false;
            if (r.Min < this.Min) return false;
            return true;
        }

        public RangeFloat PutInRange(RangeFloat r, bool throwException = true)
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
                float min = r.Min < this.Min ? this.Min : r.Min;
                float max = r.Max < this.Max ? this.Max : r.Max;
                return new RangeFloat(min, max);
            }
        }

        public override int GetHashCode()
        {
            return HashHelper.GetHashCode(Min, Max);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is RangeFloat rhs)) return false;
            return Equals(rhs);
        }

        public bool Equals(RangeFloat other)
        {
            return this.Min.EqualsWithin(other.Min)
                && this.Max.EqualsWithin(other.Max);
        }

        public override string ToString()
        {
            return this.Min.EqualsWithin(this.Max) ? $"({Min.ToString()})" : $"({Min} - {Max})";
        }

        public static bool operator ==(RangeFloat c1, RangeFloat c2)
        {
            return c1.Equals(c2);
        }

        public static bool operator !=(RangeFloat c1, RangeFloat c2)
        {
            return !c1.Equals(c2);
        }
    }
}
