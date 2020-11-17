using System;

namespace Noggog
{
    public interface IP3Int16Get
    {
        short X { get; }
        short Y { get; }
        short Z { get; }
        P3Int16 Point { get; }
    }

    public struct P3Int16 : IP3Int16Get, IEquatable<P3Int16>
    {
        public static readonly P3Int16 Origin = new P3Int16(0, 0, 0);
        public static readonly P3Int16 One = new P3Int16(1, 1, 1);

        public readonly short X;
        public readonly short Y;
        public readonly short Z;

        short IP3Int16Get.X => this.X;
        short IP3Int16Get.Y => this.Y;
        short IP3Int16Get.Z => this.Z;
        public P3Int16 Point => this;

        public P3Int16(short x, short y, short z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public static bool TryParse(string str, out P3Int16 ret)
        {
            if (str == null)
            {
                ret = default(P3Int16);
                return false;
            }

            string[] split = str.Split(',');
            if (split.Length != 3)
            {
                ret = default(P3Int16);
                return false;
            }

            if (!short.TryParse(split[0], out short x)
                || !short.TryParse(split[1], out short y)
                || !short.TryParse(split[2], out short z))
            {
                ret = default(P3Int16);
                return false;
            }

            ret = new P3Int16(x, y, z);
            return true;
        }

        public P3Int16 Shift(short x, short y, short z)
        {
            return new P3Int16((short)(this.X + x), (short)(this.Y + y), (short)(this.Z + z));
        }

        public P3Int16 Shift(P3Int16 p)
        {
            return Shift(p.X, p.Y, p.Z);
        }

        public override bool Equals(object? obj)
        {
            if (obj is not P3Int16 rhs) return false;
            return Equals(rhs);
        }

        public bool Equals(P3Int16 rhs)
        {
            return this.X == rhs.X
                && this.Y == rhs.Y
                && this.Z == rhs.Z;
        }

        public override int GetHashCode() => HashCode.Combine(X, Y, Z);

        public override string ToString()
        {
            return $"({X},{Y},{Z}";
        }

        public static bool operator ==(P3Int16 obj1, P3Int16 obj2)
        {
            return obj1.Equals(obj2);
        }

        public static bool operator !=(P3Int16 obj1, P3Int16 obj2)
        {
            return !obj1.Equals(obj2);
        }

        public static P3Int16 operator +(P3Int16 p1, P3Int16 p2)
        {
            return p1.Shift(p2);
        }

        public static P3Int16 operator +(P3Int16 p1, int p)
        {
            return new P3Int16((short)(p1.X + p), (short)(p1.Y + p), (short)(p1.Z + p));
        }

        public static P3Int16 operator -(P3Int16 p1, P3Int16 p2)
        {
            return new P3Int16((short)(p1.X - p2.X), (short)(p1.Y - p2.Y), (short)(p1.Z - p2.Z));
        }

        public static P3Int16 operator -(P3Int16 p1, short p)
        {
            return new P3Int16((short)(p1.X - p), (short)(p1.Y - p), (short)(p1.Z - p));
        }

        public static P3Int16 operator -(P3Int16 p1)
        {
            return new P3Int16((short)(-p1.X), (short)(-p1.Y), (short)(-p1.Z));
        }

        public static P3Int16 operator *(P3Int16 p1, short num)
        {
            return new P3Int16((short)(p1.X * num), (short)(p1.Y * num), (short)(p1.Z * num));
        }

        public static P3Int16 operator *(P3Int16 p1, P3Int16 p2)
        {
            return new P3Int16((short)(p1.X * p2.X), (short)(p1.Y * p2.Y), (short)(p1.Z * p2.Z));
        }

        public static P3Int16 operator /(P3Int16 p1, short num)
        {
            return new P3Int16((short)(p1.X / num), (short)(p1.Y / num), (short)(p1.Z / num));
        }

        public static explicit operator P3Double(P3Int16 point)
        {
            return new P3Double(point.X, point.Y, point.Z);
        }
    }
}
