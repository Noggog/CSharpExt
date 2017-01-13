using System;

namespace Noggog
{
    public interface IP3IntGet
    {
        int X { get; }
        int Y { get; }
        int Z { get; }
        P3Int Point { get; }
    }

    public struct P3Int : IP3IntGet, IEquatable<P3Int>
    {
        public static readonly P3Int Origin = new P3Int(0, 0, 0);
        public static readonly P3Int One = new P3Int(1, 1, 1);

        public int X;
        public int Y;
        public int Z;

        int IP3IntGet.X
        {
            get { return X; }
        }

        int IP3IntGet.Y
        {
            get { return Y; }
        }

        int IP3IntGet.Z
        {
            get { return Z; }
        }

        public P3Int Point
        {
            get { return this; }
        }

        public P3Int(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public P3Int(P3Double vec)
        {
            X = (int)Math.Round(vec.X);
            Y = (int)Math.Round(vec.Y);
            Z = (int)Math.Round(vec.Z);
        }

        public P3Int(double x, double y, double z)
        {
            X = (int)Math.Round(x);
            Y = (int)Math.Round(y);
            Z = (int)Math.Round(z);
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

            double x, y, z;
            if (!double.TryParse(split[0], out x)
                || !double.TryParse(split[1], out y)
                || !double.TryParse(split[2], out z))
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

        public override bool Equals(object obj)
        {
            if (!(obj is P3Int)) return false;
            return Equals((P3Int)obj);
        }

        public bool Equals(P3Int rhs)
        {
            return this.X == rhs.X
                && this.Y == rhs.Y
                && this.Z == rhs.Z;
        }

        public override int GetHashCode()
        {
            return HashHelper.GetHashCode(X, Y, Z);
        }

        public override string ToString()
        {
            return "(" + X + "," + Y + "," + Z + ")";
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

        public static bool operator ==(P3Int p1, P3Int p2)
        {
            return p1.X == p2.X && p1.Y == p2.Y && p1.Z == p2.Z;
        }

        public static bool operator !=(P3Int p1, P3Int p2)
        {
            return p1.X != p2.X || p1.Y != p2.Y || p1.Z != p2.Z;
        }

        public static explicit operator P3Double(P3Int point)
        {
            return new P3Double(point.X, point.Y, point.Z);
        }
    }
}
