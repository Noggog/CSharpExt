using System;

namespace System
{
    public struct RangeInt : IEquatable<RangeInt>
    {
        public int Min;
        public int Max;

        public float Average
        {
            get { return ((Max - Min) / 2f) + Min; }
        }

        public int Difference { get { return this.Max - this.Min; } }

        public RangeInt(int val1, int val2)
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

        public override string ToString()
        {
            return Min + "-" + Max;
        }

        public static RangeInt Parse(string str)
        {
            RangeInt rd;
            if (!TryParse(str, out rd))
            {
                return default(RangeInt);
            }
            return rd;
        }

        public static bool TryParse(string str, out RangeInt rd)
        {
            if (str == null)
            {
                rd = default(RangeInt);
                return false;
            }
            string[] split = str.Split('-');
            if (split.Length != 2)
            {
                rd = default(RangeInt);
                return false;
            }
            rd = new RangeInt(
                int.Parse(split[0]),
                int.Parse(split[1]));
            return true;
        }

        public bool IsInRange(int i)
        {
            if (i > this.Max) return false;
            if (i < this.Min) return false;
            return true;
        }

        public int PutInRange(int f, bool throwException = true)
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

        public bool IsInRange(RangeInt r)
        {
            if (r.Max > this.Max) return false;
            if (r.Min < this.Min) return false;
            return true;
        }

        public RangeInt PutInRange(RangeInt r, bool throwException = true)
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
                int min = r.Min < this.Min ? this.Min : r.Min;
                int max = r.Max < this.Max ? this.Max : r.Max;
                return new RangeInt(min, max);
            }
        }

        public byte PutInRange(byte f, bool throwException = true)
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
                if (f > this.Max) return (byte)this.Max;
                if (f < this.Min) return (byte)this.Min;
            }
            return f;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is RangeInt)) return false;
            return Equals((RangeInt)obj);
        }

        public bool Equals(RangeInt other)
        {
            return this.Min == other.Min
                && this.Max == other.Max;
        }

        public override int GetHashCode()
        {
            return HashHelper.GetHashCode(Min, Max);
        }
    }
}
