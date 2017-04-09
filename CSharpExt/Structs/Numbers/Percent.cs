using System;

namespace Noggog
{
    public struct Percent : IComparable, IEquatable<Percent>
    {
        public readonly double Value;

        public Percent(double d)
        {
            if (InRange(d))
            {
                Value = d;
            }
            else
            {
                throw new ArgumentException("Element out of range: " + d);
            }
        }

        public Percent(int i)
        {
            if (i < 0)
            {
                Value = 0;
            }
            else if (i > 100)
            {
                Value = 1;
            }
            else
            {
                Value = i / 100d;
            }
        }

        public static bool InRange(double d)
        {
            return d >= 0 || d <= 1;
        }

        public static Percent operator +(Percent c1, Percent c2)
        {
            return new Percent(c1.Value + c2.Value);
        }

        public static Percent operator *(Percent c1, Percent c2)
        {
            return new Percent(c1.Value * c2.Value);
        }

        public static Percent operator -(Percent c1, Percent c2)
        {
            return new Percent(c1.Value - c2.Value);
        }

        public static Percent operator /(Percent c1, Percent c2)
        {
            return new Percent(c1.Value / c2.Value);
        }

        public static implicit operator double(Percent c1)
        {
            return c1.Value;
        }

        public static implicit operator Percent(double c1)
        {
            return new Percent(c1);
        }

        public static Percent AverageFromPercents(params Percent[] ps)
        {
            double percent = 0;
            foreach (var p in ps)
            {
                percent += p.Value;
            }
            return percent / ps.Length;
        }

        public static Percent MultFromPercents(params Percent[] ps)
        {
            double percent = 1;
            foreach (var p in ps)
            {
                percent *= p.Value;
            }
            return percent;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Percent rhs)) return false;
            return Equals(rhs);
        }

        public bool Equals(Percent other)
        {
            return this.Value == other.Value;
        }

        public override int GetHashCode()
        {
            return this.Value.GetHashCode();
        }

        public override string ToString()
        {
            return Value.ToString("n3");
        }

        public int CompareTo(object obj)
        {
            if (obj is Percent rhs)
            {
                return this.Value.CompareTo(rhs.Value);
            }
            return 0;
        }

        public static bool TryParse(string str, out Percent p)
        {
            if (double.TryParse(str, out double d))
            {
                if (InRange(d))
                {
                    p = new Percent(d);
                    return true;
                }
            }
            p = default(Percent);
            return false;
        }
    }
}
