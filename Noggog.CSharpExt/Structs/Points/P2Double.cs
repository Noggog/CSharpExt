using System;
using System.Runtime.Serialization;

namespace Noggog
{
    public struct P2Double : IEquatable<P2Double>
    {
        private double _x;
        [DataMember]
        public double X
        {
            get => _x;
            set => _x = value;
        }
        
        private double _y;
        [DataMember]
        public double Y
        {
            get => _y;
            set => _y = value;
        }

        [IgnoreDataMember]
        public double Length => Math.Sqrt(_x * _x + _y * _y);
        [IgnoreDataMember]
        public double Magnitude => Length;
        [IgnoreDataMember]
        public double SqrMagnitude => (_x * _x + _y * _y);

        [IgnoreDataMember]
        public P2Double Normalized
        {
            get
            {
                double length = Length;
                return new P2Double(_x / length, _y / length);
            }
        }

        [IgnoreDataMember]
        public P2Double Absolute => new P2Double(
            Math.Abs(this._x),
            Math.Abs(this._y));

        public P2Double(double x, double y)
        {
            this._x = x;
            this._y = y;
        }

        public override string ToString()
        {
            return $"({_x}, {_y})";
        }

        public P2Double Normalize()
        {
            var length = Length;
            return new P2Double(
                this._x / length,
                this._y / length);
        }

        public static double Dot(P2Double v1, P2Double v2) => v1._x * v2._x + v1._y * v2._y;

        public double Distance(P2Double p2) => (this - p2).Magnitude;

        public static bool TryParse(string str, out P2Double p2)
        {
            string[] split = str.Split(',');
            if (split.Length != 2)
            {
                p2 = default(P2Double);
                return false;
            }

            if (!double.TryParse(split[0], out double x))
            {
                p2 = default(P2Double);
                return false;
            }
            if (!double.TryParse(split[1], out double y))
            {
                p2 = default(P2Double);
                return false;
            }
            p2 = new P2Double(x, y);
            return true;
        }

        public override bool Equals(object? obj)
        {
            if (obj is not P2Double rhs) return false;
            return Equals(rhs);
        }

        public bool Equals(P2Double rhs)
        {
            return this._x.EqualsWithin(rhs._x)
                && this._y.EqualsWithin(rhs._y);
        }

        public override int GetHashCode() => HashCode.Combine(_x, _y);

        public static P2Double Max(double p1, double p2, double c1, double c2)
        {
            return new P2Double(Math.Max(p1, c1), Math.Max(p2, c2));
        }

        public static P2Double Max(P2Double p, P2Double c)
        {
            return new P2Double(Math.Max(p._x, c._x), Math.Max(p._y, c._y));
        }

        public P2Double Max(double c)
        {
            return new P2Double(Math.Max(_x, c), Math.Max(_y, c));
        }

        public static bool operator ==(P2Double obj1, P2Double obj2)
        {
            return obj1.Equals(obj2);
        }

        public static bool operator !=(P2Double obj1, P2Double obj2)
        {
            return !obj1.Equals(obj2);
        }

        public static P2Double operator -(P2Double c1)
        {
            return new P2Double(-c1._x, -c1._y);
        }

        public static P2Double operator +(P2Double c1, P2Double c2)
        {
            return new P2Double(c1._x + c2._x, c1._y + c2._y);
        }

        public static P2Double operator +(P2Double c1, double f)
        {
            return new P2Double(c1._x + f, c1._y + f);
        }

        public static P2Double operator -(P2Double c1, P2Double c2)
        {
            return new P2Double(c1._x - c2._x, c1._y - c2._y);
        }

        public static P2Double operator -(P2Double c1, double f)
        {
            return new P2Double(c1._x - f, c1._y - f);
        }

        public static P2Double operator *(P2Double c1, P2Double c2)
        {
            return new P2Double(c1._x * c2._x, c1._y * c2._y);
        }

        public static P2Double operator *(P2Double c1, double f)
        {
            return new P2Double(c1._x * f, c1._y * f);
        }

        public static P2Double operator /(P2Double c1, P2Double c2)
        {
            return new P2Double(c1._x / c2._x, c1._y / c2._y);
        }

        public static P2Double operator /(P2Double c1, double f)
        {
            return new P2Double(c1._x / f, c1._y / f);
        }

        public static implicit operator P2Double(P2Int point)
        {
            return new P2Double(point.X, point.Y);
        }
    }
}
