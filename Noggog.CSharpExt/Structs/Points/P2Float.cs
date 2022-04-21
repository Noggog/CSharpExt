using System;
using System.Runtime.Serialization;

namespace Noggog
{
    public struct P2Float : IEquatable<P2Float>
    {
        private float _x;
        [DataMember]
        public float X
        {
            get => _x;
            set => _x = value;
        }
        
        private float _y;
        [DataMember]
        public float Y
        {
            get => _y;
            set => _y = value;
        }

        [IgnoreDataMember]
        public float Length => (float)Math.Sqrt(_x * _x + _y * _y);
        [IgnoreDataMember]
        public float Magnitude => Length;
        [IgnoreDataMember]
        public float SqrMagnitude => (_x * _x + _y * _y);

        [IgnoreDataMember]
        public P2Float Normalized
        {
            get
            {
                float length = Length;
                return new P2Float(_x / length, _y / length);
            }
        }

        [IgnoreDataMember]
        public P2Float Absolute => new P2Float(
            Math.Abs(this._x),
            Math.Abs(this._y));

        public P2Float(float x, float y)
        {
            this._x = x;
            this._y = y;
        }

        public override string ToString()
        {
            return $"({_x}, {_y})";
        }

        public P2Float Normalize()
        {
            var length = Length;
            return new P2Float(
                this._x / length,
                this._y / length);
        }

        public static float Dot(P2Float v1, P2Float v2) => v1._x * v2._x + v1._y * v2._y;
        public float Distance(P2Float p2) => (this - p2).Magnitude;

        public static bool TryParse(string str, out P2Float p2)
        {
            string[] split = str.Split(',');
            if (split.Length != 2)
            {
                p2 = default(P2Float);
                return false;
            }

            if (!float.TryParse(split[0], out float x))
            {
                p2 = default(P2Float);
                return false;
            }
            if (!float.TryParse(split[1], out float y))
            {
                p2 = default(P2Float);
                return false;
            }
            p2 = new P2Float(x, y);
            return true;
        }

        public override bool Equals(object? obj)
        {
            if (obj is not P2Float rhs) return false;
            return Equals(rhs);
        }

        public bool Equals(P2Float rhs)
        {
            return this._x.EqualsWithin(rhs._x)
                && this._y.EqualsWithin(rhs._y);
        }

        public override int GetHashCode() => HashCode.Combine(_x, _y);

        public static P2Float Max(P2Float p, P2Float c)
        {
            return new P2Float(Math.Max(p._x, c._x), Math.Max(p._y, c._y));
        }

        public P2Float Max(float c)
        {
            return new P2Float(Math.Max(_x, c), Math.Max(_y, c));
        }

        public static bool operator ==(P2Float obj1, P2Float obj2)
        {
            return obj1.Equals(obj2);
        }

        public static bool operator !=(P2Float obj1, P2Float obj2)
        {
            return !obj1.Equals(obj2);
        }

        public static P2Float operator -(P2Float c1)
        {
            return new P2Float(-c1._x, -c1._y);
        }

        public static P2Float operator +(P2Float c1, P2Float c2)
        {
            return new P2Float(c1._x + c2._x, c1._y + c2._y);
        }

        public static P2Float operator +(P2Float c1, float f)
        {
            return new P2Float(c1._x + f, c1._y + f);
        }

        public static P2Float operator -(P2Float c1, P2Float c2)
        {
            return new P2Float(c1._x - c2._x, c1._y - c2._y);
        }

        public static P2Float operator -(P2Float c1, float f)
        {
            return new P2Float(c1._x - f, c1._y - f);
        }

        public static P2Float operator *(P2Float c1, P2Float c2)
        {
            return new P2Float(c1._x * c2._x, c1._y * c2._y);
        }

        public static P2Float operator *(P2Float c1, float f)
        {
            return new P2Float(c1._x * f, c1._y * f);
        }

        public static P2Float operator /(P2Float c1, P2Float c2)
        {
            return new P2Float(c1._x / c2._x, c1._y / c2._y);
        }

        public static P2Float operator /(P2Float c1, float f)
        {
            return new P2Float(c1._x / f, c1._y / f);
        }

        public static implicit operator P2Float(P2Int point)
        {
            return new P2Float(point.X, point.Y);
        }
    }
}
