using System;

namespace System
{
    public class P2IntValueObj<T>
    {
        public P2IntValue<T> Value;

        public static implicit operator P2IntValue<T>(P2IntValueObj<T> obj)
        {
            return obj.Value;
        }

        public static implicit operator P2IntValueObj<T>(P2IntValue<T> obj)
        {
            return new P2IntValueObj<T>() { Value = obj };
        }
    }

    public struct P2IntValue<T> : IP2IntGet, IEquatable<P2IntValue<T>>
    {
        public T Value;
        public P2Int Point;
        public int X { get { return Point.X; } }
        public int Y { get { return Point.Y; } }
        P2Int IP2IntGet.Point { get { return Point; } }

        public P2IntValue(int x, int y, T val)
        {
            Value = val;
            Point = new P2Int(x, y);
        }

        public P2IntValue(double x, double y, T val)
        {
            Value = val;
            Point = new P2Double(x, y);
        }

        public P2IntValue(P2Double vect, T val)
        {
            Value = val;
            Point = vect;
        }

        public P2IntValue(P2Int rhs, T val)
        {
            Value = val;
            Point = new P2Int(rhs);
        }

        public P2IntValue(P2IntValue<T> rhs)
        {
            Value = rhs.Value;
            Point = rhs.Point;
        }

        public override string ToString()
        {
            return "PointValue (" + Point.X + "," + Point.Y + ", " + Value + ")";
        }

        public override bool Equals(object obj)
        {
            if (!(obj is P2IntValue<T>)) return false;
            return Equals((P2IntValue<T>)obj);
        }
        
        public bool Equals(P2IntValue<T> rhs)
        {
            return this.Point == rhs.Point
                && object.Equals(this.Value, rhs.Value);
        }

        public override int GetHashCode()
        {
            return HashHelper.GetHashCode(Point).CombineHashCode(Value);
        }

        public static bool operator ==(P2IntValue<T> left, P2IntValue<T> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(P2IntValue<T> left, P2IntValue<T> right)
        {
            return !Equals(left, right);
        }

        public static implicit operator P2Int(P2IntValue<T> p)
        {
            return p.Point;
        }

        public static implicit operator T(P2IntValue<T> p)
        {
            return p.Value;
        }
    }

}