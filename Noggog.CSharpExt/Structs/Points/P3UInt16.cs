using System;

namespace Noggog
{
    public interface IP3UInt16Get
    {
        ushort X { get; }
        ushort Y { get; }
        ushort Z { get; }
        P3UInt16 Point { get; }
    }

    public struct P3UInt16 : IP3UInt16Get, IEquatable<P3UInt16>
    {
        public static readonly P3UInt16 Origin = new P3UInt16(0, 0, 0);
        public static readonly P3UInt16 One = new P3UInt16(1, 1, 1);

        public readonly ushort X;
        public readonly ushort Y;
        public readonly ushort Z;

        ushort IP3UInt16Get.X => this.X;
        ushort IP3UInt16Get.Y => this.Y;
        ushort IP3UInt16Get.Z => this.Z;
        public P3UInt16 Point => this;

        public P3UInt16(ushort x, ushort y, ushort z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public static bool TryParse(string str, out P3UInt16 ret)
        {
            if (str == null)
            {
                ret = default(P3UInt16);
                return false;
            }

            string[] split = str.Split(',');
            if (split.Length != 3)
            {
                ret = default(P3UInt16);
                return false;
            }

            if (!ushort.TryParse(split[0], out ushort x)
                || !ushort.TryParse(split[1], out ushort y)
                || !ushort.TryParse(split[2], out ushort z))
            {
                ret = default(P3UInt16);
                return false;
            }

            ret = new P3UInt16(x, y, z);
            return true;
        }

        public P3UInt16 Shift(ushort x, ushort y, ushort z)
        {
            return new P3UInt16((ushort)(this.X + x), (ushort)(this.Y + y), (ushort)(this.Z + z));
        }

        public P3UInt16 Shift(P3UInt16 p)
        {
            return Shift(p.X, p.Y, p.Z);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is P3UInt16 rhs)) return false;
            return Equals(rhs);
        }

        public bool Equals(P3UInt16 rhs)
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

        public static bool operator ==(P3UInt16 obj1, P3UInt16 obj2)
        {
            return obj1.Equals(obj2);
        }

        public static bool operator !=(P3UInt16 obj1, P3UInt16 obj2)
        {
            return !obj1.Equals(obj2);
        }

        public static P3UInt16 operator +(P3UInt16 p1, P3UInt16 p2)
        {
            return p1.Shift(p2);
        }

        public static P3UInt16 operator +(P3UInt16 p1, int p)
        {
            return new P3UInt16((ushort)(p1.X + p), (ushort)(p1.Y + p), (ushort)(p1.Z + p));
        }

        public static P3UInt16 operator -(P3UInt16 p1, P3UInt16 p2)
        {
            return new P3UInt16((ushort)(p1.X - p2.X), (ushort)(p1.Y - p2.Y), (ushort)(p1.Z - p2.Z));
        }

        public static P3UInt16 operator -(P3UInt16 p1, ushort p)
        {
            return new P3UInt16((ushort)(p1.X - p), (ushort)(p1.Y - p), (ushort)(p1.Z - p));
        }

        public static P3UInt16 operator -(P3UInt16 p1)
        {
            return new P3UInt16((ushort)(-p1.X), (ushort)(-p1.Y), (ushort)(-p1.Z));
        }

        public static P3UInt16 operator *(P3UInt16 p1, ushort num)
        {
            return new P3UInt16((ushort)(p1.X * num), (ushort)(p1.Y * num), (ushort)(p1.Z * num));
        }

        public static P3UInt16 operator *(P3UInt16 p1, P3UInt16 p2)
        {
            return new P3UInt16((ushort)(p1.X * p2.X), (ushort)(p1.Y * p2.Y), (ushort)(p1.Z * p2.Z));
        }

        public static P3UInt16 operator /(P3UInt16 p1, ushort num)
        {
            return new P3UInt16((ushort)(p1.X / num), (ushort)(p1.Y / num), (ushort)(p1.Z / num));
        }

        public static explicit operator P3Double(P3UInt16 point)
        {
            return new P3Double(point.X, point.Y, point.Z);
        }
    }
}
