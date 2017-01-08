using System;

namespace System
{
    public struct P2Double : IEquatable<P2Double>
    {
        public double X;
        public double Y;

        public P2Double(double x, double y)
        {
            X = x;
            Y = y;
        }

        public override string ToString()
        {
            return $"P2Double ({X}, {Y})";
        }

        public P2Double Normalized
        {
            get
            {
                double length = Length;
                return new P2Double(X / length, Y / length);
            }
        }

        public P2Double Absolute
        {
            get
            {
                return new P2Double(
                    Math.Abs(this.X),
                    Math.Abs(this.Y));
            }
        }

        public P2Double Normalize()
        {
            var length = Length;
            return new P2Double(
                this.X / length,
                this.Y / length);
        }

        public double Length
        {
            get { return Math.Sqrt(X * X + Y * Y); }
        }

        public static double Dot(P2Double v1, P2Double v2)
        {
            return v1.X * v2.X + v1.Y * v2.Y;
        }

        public double Magnitude
        {
            get { return Length; }
        }

        public double sqrMagnitude
        {
            get { return (X * X + Y * Y); }
        }

        public double Distance(P2Double p2)
        {
            return (this - p2).Magnitude;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is P2Double)) return false;
            return Equals((P2Double)obj);
        }

        public bool Equals(P2Double rhs)
        {
            return this.X.EqualsWithin(rhs.X)
                && this.Y.EqualsWithin(rhs.Y);
        }

        public override int GetHashCode()
        {
            return HashHelper.GetHashCode(
                this.X,
                this.Y);
        }

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

        public static P2Double Max(P2Double p, double c)
        {
            return new P2Double(Math.Max(p.X, c), Math.Max(p.Y, c));
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
