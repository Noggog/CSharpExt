using System;
using System.Collections;
using System.Collections.Generic;

namespace Noggog
{
    public struct RangeInt32 : IEquatable<RangeInt32>, IEnumerable<int>
    {
        public readonly int Min;
        public readonly int Max;
        public float Average => ((Max - Min) / 2f) + Min;
        public int Difference => this.Max - this.Min;
        public uint Width => (uint)(this.Max - this.Min + 1);

        public RangeInt32(int val1, int val2)
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

        public RangeInt32(int? min, int? max)
            : this(min ?? int.MinValue, max ?? int.MaxValue)
        {
        }

        public RangeInt32(int val)
            : this(val, val)
        {
        }

        public static RangeInt32 Parse(string str)
        {
            if (!TryParse(str, out RangeInt32 rd))
            {
                return default(RangeInt32);
            }
            return rd;
        }

        public static bool TryParse(string str, out RangeInt32 rd)
        {
            if (str == null)
            {
                rd = default(RangeInt32);
                return false;
            }
            string[] split = str.Split('-');
            if (split.Length != 2)
            {
                rd = default(RangeInt32);
                return false;
            }
            rd = new RangeInt32(
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

        public bool IsInRange(RangeInt32 r)
        {
            if (r.Max > this.Max) return false;
            if (r.Min < this.Min) return false;
            return true;
        }

        public RangeInt32 PutInRange(RangeInt32 r, bool throwException = true)
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
                return new RangeInt32(min, max);
            }
        }
        
        public override bool Equals(object obj)
        {
            if (!(obj is RangeInt32 rhs)) return false;
            return Equals(rhs);
        }

        public bool Equals(RangeInt32 other)
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

        public IEnumerator<int> GetEnumerator()
        {
            for (int i = this.Min; i <= this.Max; i++)
            {
                yield return i;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public static bool operator ==(RangeInt32 c1, RangeInt32 c2)
        {
            return c1.Equals(c2);
        }

        public static bool operator !=(RangeInt32 c1, RangeInt32 c2)
        {
            return !c1.Equals(c2);
        }
    }
}
