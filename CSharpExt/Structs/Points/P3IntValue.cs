using System;

namespace Noggog
{
    public class P3IntValueObj<T>
    {
        public P3IntValue<T> Value;

        public static implicit operator P3IntValue<T>(P3IntValueObj<T> obj)
        {
            return obj.Value;
        }

        public static implicit operator P3IntValueObj<T>(P3IntValue<T> obj)
        {
            return new P3IntValueObj<T>() { Value = obj };
        }
    }

    public struct P3IntValue<T> : IP3IntGet, IEquatable<P3IntValue<T>>
    {
        public T Value;
        public P3Int Point;
        public int X { get { return Point.X; } }
        public int Y { get { return Point.Y; } }
        public int Z { get { return Point.Z; } }
        P3Int IP3IntGet.Point { get { return this.Point; } }

        public P3IntValue(int x, int y, int z, T val)
        {
            this.Value = val;
            Point = new P3Int(x, y, z);
        }

        public P3IntValue(float x, float y, float z, T val)
        {
            this.Value = val;
            Point = new P3Int(x, y, z);
        }

        public P3IntValue(P3Double vect, T val)
        {
            this.Value = val;
            Point = new P3Int(vect);
        }

        public P3IntValue(P3Int rhs, T val)
        {
            this.Value = val;
            Point = rhs;
        }

        public P3IntValue(P3IntValue<T> rhs)
        {
            this.Value = rhs.Value;
            this.Point = rhs.Point;
        }

        public override string ToString()
        {
            return "PointValue (" + Point.X + "," + Point.Y + ", " + Value + ")";
        }

        public override bool Equals(object obj)
        {
            if (!(obj is P3IntValue<T>)) return false;
            return Equals((P3IntValue<T>)obj);
        }

        public bool Equals(P3IntValue<T> rhs)
        {
            return this.Point == rhs.Point
                && object.Equals(Value, rhs.Value);
        }

        public override int GetHashCode()
        {
            return HashHelper.GetHashCode(Point).CombineHashCode(Value);
        }

        public static bool operator ==(P3IntValue<T> left, P3IntValue<T> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(P3IntValue<T> left, P3IntValue<T> right)
        {
            return !Equals(left, right);
        }

        public static implicit operator P3Int(P3IntValue<T> p)
        {
            return p.Point;
        }
    }

}