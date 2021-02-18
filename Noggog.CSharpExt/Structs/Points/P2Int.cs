using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Noggog
{
    public interface IP2IntGet
    {
        [DataMember]
        int X { get; }
        [DataMember]
        int Y { get; }
        [IgnoreDataMember]
        P2Int Point { get; }
    }

    public class P2IntObj
    {
        [IgnoreDataMember]
        public P2Int Point;
    }

    public struct P2Int : IP2IntGet, IEquatable<P2Int>
    {
        private static readonly P2Int[] _directions = new[] { new P2Int(1, 0), new P2Int(-1, 0), new P2Int(0, 1), new P2Int(0, -1) };
        public static IEnumerable<P2Int> Directions => _directions;
        public static P2Int Down => _directions[3];
        public static P2Int Up => _directions[2];
        public static P2Int Left => _directions[0];
        public static P2Int Right => _directions[1];

        public readonly static P2Int Origin = new P2Int(0, 0);
        public readonly static P2Int One = new P2Int(1, 1);

        [IgnoreDataMember]
        public bool IsZero => X == 0 && Y == 0;

        [IgnoreDataMember]
        P2Int IP2IntGet.Point => this;

        [DataMember]
        public readonly int X;
        [IgnoreDataMember]
        int IP2IntGet.X => this.X;
        [DataMember]
        public readonly int Y;
        [IgnoreDataMember]
        int IP2IntGet.Y => this.Y;

        #region Ctors
        public P2Int(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public P2Int(IP2IntGet rhs)
            : this(rhs.X, rhs.Y)
        {
        }
        #endregion Ctors

        #region Shifts
        public P2Int Shift(int x, int y)
        {
            return new P2Int(this.X + x, this.Y + y);
        }

        public P2Int Shift(double x, double y)
        {
            return Shift((int)x, (int)y);
        }

        public P2Int Shift(P2Double vect)
        {
            return Shift(vect.X, vect.Y);
        }

        public P2Int Shift(P2Int p)
        {
            return Shift(p.X, p.Y);
        }

        public P2Int ShiftToPositive()
        {
            return Shift(
                this.X < 0 ? -this.X : 0,
                this.Y < 0 ? -this.Y : 0);
        }
        #endregion Shifts

        public P2Int UnitDir()
        {
            int max = Math.Max(Math.Abs(X), Math.Abs(Y));
            if (max != 0)
            {
                return new P2Int(
                    (int)Math.Round(((decimal)X) / max),
                    (int)Math.Round(((decimal)Y) / max));
            }
            else
            {
                return new P2Int();
            }
        }

        public int MidPoint()
        {
            return (Y - X) / 2;
        }

        public double Distance(P2Int rhs)
        {
            return Distance(rhs.X, rhs.Y);
        }

        public double Distance(int x, int y)
        {
            return Math.Sqrt(Math.Pow(x - this.X, 2) + Math.Pow(y - this.Y, 2));
        }

        public P2Int Invert()
        {
            return new P2Int(-X, -Y);
        }
        
        public override string ToString()
        {
            return $"({X},{Y})";
        }

        public override bool Equals(object? obj)
        {
            if (obj is not P2Int p) return false;
            return Equals(p);
        }

        public bool Equals(P2Int rhs)
        {
            return this.X == rhs.X
                && this.Y == rhs.Y;
        }

        public static bool TryParse(string str, out P2Int ret)
        {
            if (str == null)
            {
                ret = default(P2Int);
                return false;
            }

            string[] split = str.Split(',');
            if (split.Length != 2)
            {
                ret = default(P2Int);
                return false;
            }

            if (!int.TryParse(split[0], out int x)
                || !int.TryParse(split[1], out int y))
            {
                ret = default(P2Int);
                return false;
            }

            ret = new P2Int(x, y);
            return true;
        }

        public override int GetHashCode() => HashCode.Combine(X, Y);

        public bool NextTo(P2Int p)
        {
            if (p.X == this.X)
            {
                return p.Y == this.Y + 1 || p.Y == this.Y - 1;
            }
            if (p.Y == this.Y)
            {
                return p.X == this.X + 1 || p.X == this.X - 1;
            }
            return false;
        }

        public static bool operator ==(P2Int obj1, P2Int obj2)
        {
            return obj1.Equals(obj2);
        }

        public static bool operator !=(P2Int obj1, P2Int obj2)
        {
            return !obj1.Equals(obj2);
        }

        public static P2Int operator +(P2Int p1, P2Int p2)
        {
            return p1.Shift(p2);
        }

        public static P2Int operator -(P2Int p1, P2Int p2)
        {
            return new P2Int(p1.X - p2.X, p1.Y - p2.Y);
        }

        public static P2Int operator -(P2Int p1)
        {
            return new P2Int(-p1.X, -p1.Y);
        }

        public static P2Int operator *(P2Int p1, int num)
        {
            return new P2Int(p1.X * num, p1.Y * num);
        }
    }
}
