using System;

namespace Noggog
{
    public struct P3Float : IEquatable<P3Float>
    {
        public readonly float X;
        public readonly float Y;
        public readonly float Z;
        public float Length => (float)Math.Sqrt(X * X + Y * Y + Z * Z);
        public float Magnitude => Length;
        public float SqrMagnitude => (X * X + Y * Y);

        public P3Float Normalized
        {
            get
            {
                float length = Length;
                return new P3Float(X / length, Y / length, Z / length);
            }
        }

        public P3Float Absolute => new P3Float(
            Math.Abs(this.X),
            Math.Abs(this.Y),
            Math.Abs(this.Z));

        public P3Float(float x, float y, float z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public override string ToString()
        {
            return $"({X}, {Y}, {Z})";
        }

        public P3Float Normalize()
        {
            var length = Length;
            return new P3Float(
                this.X / length,
                this.Y / length,
                this.Z / length);
        }

        public static float Dot(P3Float v1, P3Float v2) => v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z;
        public float Distance(P3Float p2) => (this - p2).Magnitude;

        public static bool TryParse(string str, out P3Float p2)
        {
            string[] split = str.Split(',');
            if (split.Length != 3)
            {
                p2 = default(P3Float);
                return false;
            }

            if (!float.TryParse(split[0], out float x))
            {
                p2 = default(P3Float);
                return false;
            }
            if (!float.TryParse(split[1], out float y))
            {
                p2 = default(P3Float);
                return false;
            }
            if (!float.TryParse(split[2], out float z))
            {
                p2 = default(P3Float);
                return false;
            }
            p2 = new P3Float(x, y, z);
            return true;
        }

        public override bool Equals(object? obj)
        {
            if (obj is not P3Float rhs) return false;
            return Equals(rhs);
        }

        public bool Equals(P3Float rhs)
        {
            return this.X.EqualsWithin(rhs.X)
                && this.Y.EqualsWithin(rhs.Y)
                && this.Z.EqualsWithin(rhs.Z);
        }

        public override int GetHashCode() => HashCode.Combine(X, Y, Z);

        public static P3Float Max(P3Float p, P3Float c)
        {
            return new P3Float(Math.Max(p.X, c.X), Math.Max(p.Y, c.Y), Math.Max(p.Z, c.Z));
        }

        public P3Float Max(float c)
        {
            return new P3Float(Math.Max(X, c), Math.Max(Y, c), Math.Max(Z, c));
        }

        public static bool operator ==(P3Float obj1, P3Float obj2)
        {
            return obj1.Equals(obj2);
        }

        public static bool operator !=(P3Float obj1, P3Float obj2)
        {
            return !obj1.Equals(obj2);
        }

        public static P3Float operator -(P3Float c1)
        {
            return new P3Float(-c1.X, -c1.Y, -c1.Z);
        }

        public static P3Float operator +(P3Float c1, P3Float c2)
        {
            return new P3Float(c1.X + c2.X, c1.Y + c2.Y, c1.Z + c2.Z);
        }

        public static P3Float operator +(P3Float c1, float f)
        {
            return new P3Float(c1.X + f, c1.Y + f, c1.Y + f);
        }

        public static P3Float operator -(P3Float c1, P3Float c2)
        {
            return new P3Float(c1.X - c2.X, c1.Y - c2.Y, c1.Z - c2.Z);
        }

        public static P3Float operator -(P3Float c1, float f)
        {
            return new P3Float(c1.X - f, c1.Y - f, c1.Z - f);
        }

        public static P3Float operator *(P3Float c1, P3Float c2)
        {
            return new P3Float(c1.X * c2.X, c1.Y * c2.Y, c1.Z * c2.Z);
        }

        public static P3Float operator *(P3Float c1, float f)
        {
            return new P3Float(c1.X * f, c1.Y * f, c1.Z * f);
        }

        public static P3Float operator /(P3Float c1, P3Float c2)
        {
            return new P3Float(c1.X / c2.X, c1.Y / c2.Y, c1.Z / c2.Z);
        }

        public static P3Float operator /(P3Float c1, float f)
        {
            return new P3Float(c1.X / f, c1.Y / f, c1.Z / f);
        }

        public static implicit operator P3Float(P3Int point)
        {
            return new P3Float(point.X, point.Y, point.Z);
        }
    }
}
