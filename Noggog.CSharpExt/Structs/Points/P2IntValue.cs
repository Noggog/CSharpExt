using System;
using System.Runtime.Serialization;

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
        [DataMember]
        private int _x;
        public int X
        {
            get => _x;
            set => _x = value;
        }
        
        [DataMember]
        private int _y;
        public int Y
        {
            get => _y;
            set => _y = value;
        }
        
        [DataMember]
        private T _value;
        public T Value
        {
            get => _value;
            set => _value = value;
        }
        
        [IgnoreDataMember]
        public P2Int Point => new P2Int(_x, _y);

        public P2IntValue(int x, int y, T val)
        {
            _x = x;
            _y = y;
            _value = val;
        }

        public P2IntValue(P2Int rhs, T val)
        {
            _x = rhs.X;
            _y = rhs.Y;
            _value = val;
        }

        public P2IntValue(P2IntValue<T> rhs)
        {
            _x = rhs._x;
            _y = rhs._y;
            _value = rhs.Value;
        }

        public override string ToString()
        {
            return $"({_x}, {_y}, {Value})";
        }

        public override bool Equals(object? obj)
        {
            if (obj is not P2IntValue<T> rhs) return false;
            return Equals(rhs);
        }
        
        public bool Equals(P2IntValue<T> rhs)
        {
            return _x == rhs._x
                && _y == rhs._y
                && object.Equals(Value, rhs.Value);
        }

        public override int GetHashCode()
        {
            var hash = new HashCode();
            hash.Add(_x);
            hash.Add(_y);
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
            return new P2Int(p._x, p._y);
        }

        public static implicit operator T(P2IntValue<T> p)
        {
            return p.Value;
        }
    }

}
