using System;
using System.Collections.Generic;

namespace Noggog
{
    public interface IP2Int16Get
    {
        short X { get; }
        short Y { get; }
        P2Int16 Point { get; }
    }

    public class P2Int16Obj
    {
        public P2Int Point;
    }

    public struct P2Int16 : IP2Int16Get, IEquatable<P2Int16>
    {
        private static readonly P2Int16[] _directions = new[] { new P2Int16(1, 0), new P2Int16(-1, 0), new P2Int16(0, 1), new P2Int16(0, -1) };
        public static IEnumerable<P2Int16> Directions => _directions;
        public static P2Int16 Down => _directions[3];
        public static P2Int16 Up => _directions[2];
        public static P2Int16 Left => _directions[0];
        public static P2Int16 Right => _directions[1];

        public readonly static P2Int16 Origin = new P2Int16(0, 0);
        public readonly static P2Int16 One = new P2Int16(1, 1);
        public bool IsZero => X == 0 && Y == 0;

        P2Int16 IP2Int16Get.Point => this;
        public readonly short X;
        short IP2Int16Get.X => this.X;
        public readonly short Y;
        short IP2Int16Get.Y => this.Y;

        #region Ctors
        public P2Int16(short x, short y)
        {
            this.X = x;
            this.Y = y;
        }

        public P2Int16(IP2Int16Get rhs)
            : this(rhs.X, rhs.Y)
        {
        }
        #endregion Ctors

        #region Shifts
        public P2Int16 Shift(short x, short y)
        {
            return new P2Int16((short)(this.X + x), (short)(this.Y + y));
        }

        public P2Int16 Shift(double x, double y)
        {
            return Shift((short)x, (short)y);
        }

        public P2Int16 Shift(P2Double vect)
        {
            return Shift(vect.X, vect.Y);
        }

        public P2Int16 Shift(P2Int16 p)
        {
            return Shift(p.X, p.Y);
        }

        public P2Int16 ShiftToPositive()
        {
            return Shift(
                this.X < 0 ? -this.X : 0,
                this.Y < 0 ? -this.Y : 0);
        }
        #endregion Shifts

        public P2Int16 UnitDir()
        {
            short max = (short)Math.Max(Math.Abs(X), Math.Abs(Y));
            if (max != 0)
            {
                return new P2Int16(
                    (short)Math.Round(((decimal)X) / max),
                    (short)Math.Round(((decimal)Y) / max));
            }
            else
            {
                return new P2Int16();
            }
        }

        public short MidPoint()
        {
            return (short)((Y - X) / 2);
        }

        public double Distance(P2Int16 rhs)
        {
            return Distance(rhs.X, rhs.Y);
        }

        public double Distance(short x, short y)
        {
            return Math.Sqrt(Math.Pow(x - this.X, 2) + Math.Pow(y - this.Y, 2));
        }

        public P2Int16 Invert()
        {
            return new P2Int16((short)-X, (short)-Y);
        }
        
        public override string ToString()
        {
            return $"({X},{Y})";
        }

        public static void Rotate(P2Int16 p, out P2Int16 outP, ClockRotation rotation)
        {
            switch (rotation)
            {
                case ClockRotation.ClockWise:
                    outP = new P2Int16(p.Y, (short)-p.X);
                    break;
                case ClockRotation.CounterClockWise:
                    outP = new P2Int16((short)-p.Y, p.X);
                    break;
                case ClockRotation.OneEighty:
                    outP = new P2Int16(
                        (short)-p.X,
                        (short)-p.Y);
                    break;
                case ClockRotation.None:
                    outP = p;
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        public P2Int16 Rotate(ClockRotation rotation)
        {
            switch (rotation)
            {
                case ClockRotation.ClockWise:
                    return new P2Int16(Y, (short)-X);
                case ClockRotation.CounterClockWise:
                    return new P2Int16((short)-Y, X);
                case ClockRotation.OneEighty:
                    return new P2Int16((short)-Y, (short)-X);
                case ClockRotation.None:
                    return this;
                default:
                    throw new NotImplementedException();
            }
        }

        public override bool Equals(object obj)
        {
            if (!(obj is P2Int16)) return false;
            return Equals((P2Int16)obj);
        }

        public bool Equals(P2Int16 rhs)
        {
            return this.X == rhs.X
                && this.Y == rhs.Y;
        }

        public static bool TryParse(string str, out P2Int16 ret)
        {
            if (str == null)
            {
                ret = default(P2Int16);
                return false;
            }

            string[] split = str.Split(',');
            if (split.Length != 2)
            {
                ret = default(P2Int16);
                return false;
            }

            if (!short.TryParse(split[0], out var x)
                || !short.TryParse(split[1], out var y))
            {
                ret = default(P2Int16);
                return false;
            }

            ret = new P2Int16(x, y);
            return true;
        }

        public override int GetHashCode()
        {
            return HashHelper.GetHashCode(X, Y);
        }

        public bool NextTo(P2Int16 p)
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

        public static bool operator ==(P2Int16 obj1, P2Int16 obj2)
        {
            return obj1.Equals(obj2);
        }

        public static bool operator !=(P2Int16 obj1, P2Int16 obj2)
        {
            return !obj1.Equals(obj2);
        }

        public static P2Int16 operator +(P2Int16 p1, P2Int16 p2)
        {
            return p1.Shift(p2);
        }

        public static P2Int16 operator -(P2Int16 p1, P2Int16 p2)
        {
            return new P2Int16((short)(p1.X - p2.X), (short)(p1.Y - p2.Y));
        }

        public static P2Int16 operator -(P2Int16 p1)
        {
            return new P2Int16((short)-p1.X, (short)-p1.Y);
        }

        public static P2Int16 operator *(P2Int16 p1, short num)
        {
            return new P2Int16((short)(p1.X * num), (short)(p1.Y * num));
        }
    }
}
