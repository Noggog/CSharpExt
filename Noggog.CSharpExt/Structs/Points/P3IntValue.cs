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
        public readonly int X;
        public readonly int Y;
        public readonly int Z;
        public readonly T Value;
        int IP3IntGet.X => this.X;
        int IP3IntGet.Y => this.Y;
        int IP3IntGet.Z => this.Z;
        P3Int IP3IntGet.Point => new P3Int(this.X, this.Y, this.Z);

        public P3IntValue(int x, int y, int z, T val)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.Value = val;
        }

        public P3IntValue(P3Int rhs, T val)
        {
            this.X = rhs.X;
            this.Y = rhs.Y;
            this.Z = rhs.Z;
            this.Value = val;
        }

        public P3IntValue(P3IntValue<T> rhs)
        {
            this.X = rhs.X;
            this.Y = rhs.Y;
            this.Z = rhs.Z;
            this.Value = rhs.Value;
        }

        public override string ToString()
        {
            return $"({this.X},{this.Y},{this.Z},{this.Value})";
        }

        public override bool Equals(object obj)
        {
            if (!(obj is P3IntValue<T> rhs)) return false;
            return Equals(rhs);
        }

        public bool Equals(P3IntValue<T> rhs)
        {
            return this.X == rhs.X
                && this.Y == rhs.Y
                && this.Z == rhs.Z
                && object.Equals(Value, rhs.Value);
        }

        public override int GetHashCode()
        {
            var hash = new HashCode();
            hash.Add(X);
            hash.Add(Y);
            hash.Add(Z);
            hash.Add(Value);
            return hash.ToHashCode();
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
            return new P3Int(p.X, p.Y, p.Z);
        }
    }

}
