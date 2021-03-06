using System;
using System.Runtime.Serialization;

namespace Noggog
{
    public interface IP3IntGet
    {
        [DataMember]
        int X { get; }
        [DataMember]
        int Y { get; }
        [DataMember]
        int Z { get; }
        [IgnoreDataMember]
        P3Int Point { get; }
    }

    public struct P3Int : IP3IntGet, IEquatable<P3Int>
    {
        public static readonly P3Int Origin = new P3Int(0, 0, 0);
        public static readonly P3Int One = new P3Int(1, 1, 1);

        [DataMember]
        public readonly int X;
        [DataMember]
        public readonly int Y;
        [DataMember]
        public readonly int Z;

        [IgnoreDataMember]
        int IP3IntGet.X => this.X;
        [IgnoreDataMember]
        int IP3IntGet.Y => this.Y;
        [IgnoreDataMember]
        int IP3IntGet.Z => this.Z;
        [IgnoreDataMember]
        public P3Int Point => this;

        public P3Int(int x, int y, int z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public static bool TryParse(string str, out P3Int ret)
        {
            if (str == null)
            {
                ret = default(P3Int);
                return false;
            }

            string[] split = str.Split(',');
            if (split.Length != 3)
            {
                ret = default(P3Int);
                return false;
            }

            if (!int.TryParse(split[0], out int x)
                || !int.TryParse(split[1], out int y)
                || !int.TryParse(split[2], out int z))
            {
                ret = default(P3Int);
                return false;
            }

            ret = new P3Int(x, y, z);
            return true;
        }

        public P3Int Shift(int x, int y, int z)
        {
            return new P3Int(this.X + x, this.Y + y, this.Z + z);
        }

        public P3Int Shift(P3Int p)
        {
            return Shift(p.X, p.Y, p.Z);
        }

        public override bool Equals(object? obj)
        {
            if (obj is not P3Int rhs) return false;
            return Equals(rhs);
        }

        public bool Equals(P3Int rhs)
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

        public static bool operator ==(P3Int obj1, P3Int obj2)
        {
            return obj1.Equals(obj2);
        }

        public static bool operator !=(P3Int obj1, P3Int obj2)
        {
            return !obj1.Equals(obj2);
        }

        public static P3Int operator +(P3Int p1, P3Int p2)
        {
            return p1.Shift(p2);
        }

        public static P3Int operator +(P3Int p1, int p)
        {
            return new P3Int(p1.X + p, p1.Y + p, p1.Z + p);
        }

        public static P3Int operator -(P3Int p1, P3Int p2)
        {
            return new P3Int(p1.X - p2.X, p1.Y - p2.Y, p1.Z - p2.Z);
        }

        public static P3Int operator -(P3Int p1, int p)
        {
            return new P3Int(p1.X - p, p1.Y - p, p1.Z - p);
        }

        public static P3Int operator -(P3Int p1)
        {
            return new P3Int(-p1.X, -p1.Y, -p1.Z);
        }

        public static P3Int operator *(P3Int p1, int num)
        {
            return new P3Int(p1.X * num, p1.Y * num, p1.Z * num);
        }

        public static P3Int operator *(P3Int p1, P3Int p2)
        {
            return new P3Int(p1.X * p2.X, p1.Y * p2.Y, p1.Z * p2.Z);
        }

        public static P3Int operator /(P3Int p1, int num)
        {
            return new P3Int(p1.X / num, p1.Y / num, p1.Z / num);
        }

        public static explicit operator P3Double(P3Int point)
        {
            return new P3Double(point.X, point.Y, point.Z);
        }
    }
}
