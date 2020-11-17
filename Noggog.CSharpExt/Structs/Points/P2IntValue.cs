using System;

namespace Noggog
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
        public readonly int X;
        public readonly int Y;
        public readonly T Value;
        int IP2IntGet.X => this.X;
        int IP2IntGet.Y => this.Y;
        public P2Int Point => new P2Int(this.X, this.Y);

        public P2IntValue(int x, int y, T val)
        {
            this.X = x;
            this.Y = y;
            this.Value = val;
        }

        public P2IntValue(P2Int rhs, T val)
        {
            this.X = rhs.X;
            this.Y = rhs.Y;
            this.Value = val;
        }

        public P2IntValue(P2IntValue<T> rhs)
        {
            this.X = rhs.X;
            this.Y = rhs.Y;
            Value = rhs.Value;
        }

        public override string ToString()
        {
            return $"({this.X}, {this.Y}, {this.Value})";
        }

        public override bool Equals(object? obj)
        {
            if (obj is not P2IntValue<T> rhs) return false;
            return Equals(rhs);
        }
        
        public bool Equals(P2IntValue<T> rhs)
        {
            return this.X == rhs.X
                && this.Y == rhs.Y
                && object.Equals(this.Value, rhs.Value);
        }

        public override int GetHashCode()
        {
            var hash = new HashCode();
            hash.Add(X);
            hash.Add(Y);
            hash.Add(Value);
            return hash.ToHashCode();
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
            return new P2Int(p.X, p.Y);
        }

        public static implicit operator T(P2IntValue<T> p)
        {
            return p.Value;
        }
    }

}
