using System;

namespace Noggog
{
    public struct P2Double : IEquatable<P2Double>
    {
        public readonly double X;
        public readonly double Y;

        public double Length => Math.Sqrt(X * X + Y * Y);
        public double Magnitude => Length;
        public double SqrMagnitude => (X * X + Y * Y);

        public P2Double Normalized
        {
            get
            {
                double length = Length;
                return new P2Double(X / length, Y / length);
            }
        }

        public P2Double Absolute => new P2Double(
            Math.Abs(this.X),
            Math.Abs(this.Y));

        public P2Double(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }

        public override string ToString()
        {
            return $"({X}, {Y})";
        }

        public P2Double Normalize()
        {
            var length = Length;
            return new P2Double(
                this.X / length,
                this.Y / length);
        }

        public static double Dot(P2Double v1, P2Double v2) => v1.X * v2.X + v1.Y * v2.Y;

        public double Distance(P2Double p2) => (this - p2).Magnitude;

        public static bool TryParse(string str, out P2Double p2)
        {
            string[] split = str.Split(',');
            if (split.Length != 2)
            {
                p2 = default(P2Double);
                return false;
            }

            if (!double.TryParse(split[0], out double x))
            {
                p2 = default(P2Double);
                return false;
            }
            if (!double.TryParse(split[1], out double y))
            {
                p2 = default(P2Double);
                return false;
            }
            p2 = new P2Double(x, y);
            return true;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is P2Double rhs)) return false;
            return Equals(rhs);
        }

        public bool Equals(P2Double rhs)
        {
            return this.X.EqualsWithin(rhs.X)
                && this.Y.EqualsWithin(rhs.Y);
        }

        public override int GetHashCode() => HashCode.Combine(X, Y);

        public static P2Double Max(double p1, double p2, double c1, double c2)
        {
            return new P2Double(Math.Max(p1, c1), Math.Max(p2, c2));
        }

        public static P2Double Max(P2Double p, P2Double c)
        {
            return new P2Double(Math.Max(p.X, c.X), Math.Max(p.Y, c.Y));
        }

        public P2Double Max(double c)
        {
            return new P2Double(Math.Max(X, c), Math.Max(Y, c));
        }

        public static bool operator ==(P2Double obj1, P2Double obj2)
        {
            return obj1.Equals(obj2);
        }

        public static bool operator !=(P2Double obj1, P2Double obj2)
        {
            return !obj1.Equals(obj2);
        }

        public static P2Double operator -(P2Double c1)
        {
            return new P2Double(-c1.X, -c1.Y);
        }

        public static P2Double operator +(P2Double c1, P2Double c2)
        {
            return new P2Double(c1.X + c2.X, c1.Y + c2.Y);
        }

        public static P2Double operator +(P2Double c1, double f)
        {
            return new P2Double(c1.X + f, c1.Y + f);
        }

        public static P2Double operator -(P2Double c1, P2Double c2)
        {
            return new P2Double(c1.X - c2.X, c1.Y - c2.Y);
        }

        public static P2Double operator -(P2Double c1, double f)
        {
            return new P2Double(c1.X - f, c1.Y - f);
        }

        public static P2Double operator *(P2Double c1, P2Double c2)
        {
            return new P2Double(c1.X * c2.X, c1.Y * c2.Y);
        }

        public static P2Double operator *(P2Double c1, double f)
        {
            return new P2Double(c1.X * f, c1.Y * f);
        }

        public static P2Double operator /(P2Double c1, P2Double c2)
        {
            return new P2Double(c1.X / c2.X, c1.Y / c2.Y);
        }

        public static P2Double operator /(P2Double c1, double f)
        {
            return new P2Double(c1.X / f, c1.Y / f);
        }

        public static implicit operator P2Double(P2Int point)
        {
            return new P2Double(point.X, point.Y);
        }
    }
}
