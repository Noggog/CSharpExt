using System;
using System.Runtime.Serialization;

namespace Noggog
{
    public struct P2Float : IEquatable<P2Float>
    {
        [DataMember]
        public readonly float X;
        [DataMember]
        public readonly float Y;

        [IgnoreDataMember]
        public float Length => (float)Math.Sqrt(X * X + Y * Y);
        [IgnoreDataMember]
        public float Magnitude => Length;
        [IgnoreDataMember]
        public float SqrMagnitude => (X * X + Y * Y);

        [IgnoreDataMember]
        public P2Float Normalized
        {
            get
            {
                float length = Length;
                return new P2Float(X / length, Y / length);
            }
        }

        [IgnoreDataMember]
        public P2Float Absolute => new P2Float(
            Math.Abs(this.X),
            Math.Abs(this.Y));

        public P2Float(float x, float y)
        {
            this.X = x;
            this.Y = y;
        }

        public override string ToString()
        {
            return $"({X}, {Y})";
        }

        public P2Float Normalize()
        {
            var length = Length;
            return new P2Float(
                this.X / length,
                this.Y / length);
        }

        public static float Dot(P2Float v1, P2Float v2) => v1.X * v2.X + v1.Y * v2.Y;
        public float Distance(P2Float p2) => (this - p2).Magnitude;

        public static bool TryParse(string str, out P2Float p2)
        {
            string[] split = str.Split(',');
            if (split.Length != 2)
            {
                p2 = default(P2Float);
                return false;
            }

            if (!float.TryParse(split[0], out float x))
            {
                p2 = default(P2Float);
                return false;
            }
            if (!float.TryParse(split[1], out float y))
            {
                p2 = default(P2Float);
                return false;
            }
            p2 = new P2Float(x, y);
            return true;
        }

        public override bool Equals(object? obj)
        {
            if (obj is not P2Float rhs) return false;
            return Equals(rhs);
        }

        public bool Equals(P2Float rhs)
        {
            return this.X.EqualsWithin(rhs.X)
                && this.Y.EqualsWithin(rhs.Y);
        }

        public override int GetHashCode() => HashCode.Combine(X, Y);

        public static P2Float Max(P2Float p, P2Float c)
        {
            return new P2Float(Math.Max(p.X, c.X), Math.Max(p.Y, c.Y));
        }

        public P2Float Max(float c)
        {
            return new P2Float(Math.Max(X, c), Math.Max(Y, c));
        }

        public static bool operator ==(P2Float obj1, P2Float obj2)
        {
            return obj1.Equals(obj2);
        }

        public static bool operator !=(P2Float obj1, P2Float obj2)
        {
            return !obj1.Equals(obj2);
        }

        public static P2Float operator -(P2Float c1)
        {
            return new P2Float(-c1.X, -c1.Y);
        }

        public static P2Float operator +(P2Float c1, P2Float c2)
        {
            return new P2Float(c1.X + c2.X, c1.Y + c2.Y);
        }

        public static P2Float operator +(P2Float c1, float f)
        {
            return new P2Float(c1.X + f, c1.Y + f);
        }

        public static P2Float operator -(P2Float c1, P2Float c2)
        {
            return new P2Float(c1.X - c2.X, c1.Y - c2.Y);
        }

        public static P2Float operator -(P2Float c1, float f)
        {
            return new P2Float(c1.X - f, c1.Y - f);
        }

        public static P2Float operator *(P2Float c1, P2Float c2)
        {
            return new P2Float(c1.X * c2.X, c1.Y * c2.Y);
        }

        public static P2Float operator *(P2Float c1, float f)
        {
            return new P2Float(c1.X * f, c1.Y * f);
        }

        public static P2Float operator /(P2Float c1, P2Float c2)
        {
            return new P2Float(c1.X / c2.X, c1.Y / c2.Y);
        }

        public static P2Float operator /(P2Float c1, float f)
        {
            return new P2Float(c1.X / f, c1.Y / f);
        }

        public static implicit operator P2Float(P2Int point)
        {
            return new P2Float(point.X, point.Y);
        }
    }
}
