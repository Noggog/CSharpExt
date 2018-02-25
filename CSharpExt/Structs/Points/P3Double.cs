using System;

namespace Noggog
{
    public struct P3Double : IEquatable<P3Double>
    {
        public static readonly P3Double Origin = Zero;
        public static readonly P3Double Zero = new P3Double(0, 0, 0);
        public static readonly P3Double One = new P3Double(1, 1, 1);
        public static readonly P3Double Up = new P3Double(0, 1, 0);
        public static readonly P3Double Down = new P3Double(0, -1, 0);
        public static readonly P3Double Back = new P3Double(0, 0, -1);
        public static readonly P3Double Forward = new P3Double(0, 0, 1);
        public static readonly P3Double Left = new P3Double(-1, 0, 0);
        public static readonly P3Double Right = new P3Double(1, 0, 0);

        public readonly double X;
        public readonly double Y;
        public readonly double Z;

        public P3Double XPoint => new P3Double(X, 0, 0);
        public P3Double YPoint => new P3Double(0, Y, 0);
        public P3Double ZPoint => new P3Double(0, 0, Z);

        public P2Double XY => new P2Double(X, Y);
        public P2Double XZ => new P2Double(X, Z);
        public P2Double YZ => new P2Double(Y, Z);
        public P2Double ZY => new P2Double(Z, Y);
        public P2Double ZX => new P2Double(Z, X);
        public P2Double YX => new P2Double(Y, X);

        public P2Double XX => new P2Double(X, X);
        public P2Double YY => new P2Double(Y, Y);
        public P2Double ZZ => new P2Double(Z, Z);

        public P3Double XXX => new P3Double(X, X, X);
        public P3Double XXY => new P3Double(X, X, Y);
        public P3Double XXZ => new P3Double(X, X, Z);
        public P3Double XYX => new P3Double(X, Y, X);
        public P3Double XYY => new P3Double(X, Y, Y);
        public P3Double XYZ => new P3Double(X, Y, Z);
        public P3Double XZX => new P3Double(X, Z, X);
        public P3Double XZY => new P3Double(X, Z, Y);
        public P3Double XZZ => new P3Double(X, Z, Z);

        public P3Double YXX => new P3Double(Y, X, X);
        public P3Double YXY => new P3Double(Y, X, Y);
        public P3Double YXZ => new P3Double(Y, X, Z);
        public P3Double YYX => new P3Double(Y, Y, X);
        public P3Double YYY => new P3Double(Y, Y, Y);
        public P3Double YYZ => new P3Double(Y, Y, Z);
        public P3Double YZX => new P3Double(Y, Z, X);
        public P3Double YZY => new P3Double(Y, Z, Y);
        public P3Double YZZ => new P3Double(Y, Z, Z);

        public P3Double ZXX => new P3Double(Z, X, X);
        public P3Double ZXY => new P3Double(Z, X, Y);
        public P3Double ZXZ => new P3Double(Z, X, Z);
        public P3Double ZYX => new P3Double(Z, Y, X);
        public P3Double ZYY => new P3Double(Z, Y, Y);
        public P3Double ZYZ => new P3Double(Z, Y, Z);
        public P3Double ZZX => new P3Double(Z, Z, X);
        public P3Double ZZY => new P3Double(Z, Z, Y);
        public P3Double ZZZ => new P3Double(Z, Z, Z);


        public P3Double Normalized
        {
            get
            {
                double length = Length;
                return new P3Double(X / length, Y / length, Z / length);
            }
        }

        public double Length => Math.Sqrt(X * X + Y * Y + Z * Z);

        public double Magnitude => Length;

        public double SqrMagnitude => (X * X + Y * Y + Z * Z);

        public P3Double(double x, double y, double z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public P3Double(P3Double p2)
        {
            this.X = p2.X;
            this.Y = p2.Y;
            this.Z = p2.Z;
        }
        
        public P3Double Shift(double x, double y, double z)
        {
            return new P3Double(X + x, Y + y, Z + z);
        }
        
        public P3Double Shift(float x, float y, float z)
        {
            return new P3Double(X + x, Y + y, Z + z);
        }
        
        public P3Double Shift(P3Double p)
        {
            return Shift(p.X, p.Y, p.Z);
        }

        public double Distance(P3Double p2)
        {
            return (this - p2).Length;
        }

        public static double Distance(P3Double p1, P3Double p2)
        {
            return (p1 - p2).Length;
        }
        
        public P3Double SetX(double x)
        {
            return new P3Double(x, Y, Z);
        }
        
        public P3Double SetY(double y)
        {
            return new P3Double(X, y, Z);
        }
        
        public P3Double SetZ(double z)
        {
            return new P3Double(X, Y, z);
        }
        
        public P3Double ModifyX(double x)
        {
            return new P3Double(X + x, Y, Z);
        }
        
        public P3Double ModifyY(double y)
        {
            return new P3Double(X, Y + y, Z);
        }
        
        public P3Double ModifyZ(double z)
        {
            return new P3Double(X, Y, Z + z);
        }

        public bool EqualsWithin(double value, double within = .000000001d)
        {
            return X.EqualsWithin(value, within) && Y.EqualsWithin(value, within) && Z.EqualsWithin(value, within);
        }

        public bool EqualsWithin(P3Double value, double within = .000000001d)
        {
            return X.EqualsWithin(value.X, within) && Y.EqualsWithin(value.Y, within) && Z.EqualsWithin(value.Z, within);
        }

        public P3Double Absolute => new P3Double(
            Math.Abs(X),
            Math.Abs(Y),
            Math.Abs(Z));

        public static P3Double ProjectOnPlane(P3Double v, P3Double planeNormal)
        {
            var distance = -Dot(planeNormal.Normalized, v);
            return v + planeNormal * distance;
        }

        public static double Dot(P3Double v1, P3Double v2)
        {
            return v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z;
        }

        public P3Double Process(Func<double, double> conv)
        {
            return new P3Double(
                conv(X),
                conv(Y),
                conv(Z));
        }

        public static P3Double Cross(P3Double v1, P3Double v2)
        {
            double x, y, z;
            x = v1.Y * v2.Z - v2.Y * v1.Z;
            y = (v1.X * v2.Z - v2.X * v1.Z) * -1;
            z = v1.X * v2.Y - v2.X * v1.Y;

            var rtnvector = new P3Double(x, y, z);
            return rtnvector;
        }

        public static P3Double Lerp(P3Double start, P3Double end, double percent)
        {
            percent.Clamp01();
            return start + percent * (end - start);
        }

        public P3Double Max(P3Double p2)
        {
            return new P3Double(Math.Max(X, p2.X), Math.Max(Y, p2.Y), Math.Max(Z, p2.Z));
        }

        public static P3Double Max(P3Double p1, P3Double p2)
        {
            return new P3Double(Math.Max(p1.X, p2.X), Math.Max(p1.Y, p2.Y), Math.Max(p1.Z, p2.Z));
        }

        public P3Double Min(P3Double p2)
        {
            return new P3Double(Math.Min(X, p2.X), Math.Min(Y, p2.Y), Math.Min(Z, p2.Z));
        }

        public static P3Double Min(P3Double p1, P3Double p2)
        {
            return new P3Double(Math.Min(p1.X, p2.X), Math.Min(p1.Y, p2.Y), Math.Min(p1.Z, p2.Z));
        }

        public static P3Double Abs(P3Double p1)
        {
            return p1.Absolute;
        }
        
        public P3Double SetTo(P3Double rhs, AxisDirection dir)
        {
            switch (dir)
            {
                case AxisDirection.X:
                case AxisDirection.XNeg:
                    return new P3Double(
                        rhs.X,
                        Y,
                        Z);
                case AxisDirection.Y:
                case AxisDirection.YNeg:
                    return new P3Double(
                        X,
                        rhs.Y,
                        Z);
                case AxisDirection.Z:
                case AxisDirection.ZNeg:
                    return new P3Double(
                        X,
                        Y,
                        rhs.Z);
                case AxisDirection.XY:
                case AxisDirection.XYNeg:
                case AxisDirection.XNegY:
                case AxisDirection.XNegYNeg:
                    return new P3Double(
                        rhs.X,
                        rhs.Y,
                        Z);
                case AxisDirection.YZ:
                case AxisDirection.YZNeg:
                case AxisDirection.YNegZ:
                case AxisDirection.YNegZNeg:
                    return new P3Double(
                        X,
                        rhs.Y,
                        rhs.Z);
                case AxisDirection.XZ:
                case AxisDirection.XZNeg:
                case AxisDirection.XNegZ:
                case AxisDirection.XNegZNeg:
                    return new P3Double(
                        rhs.X,
                        Y,
                        rhs.Z);
                default:
                    throw new NotImplementedException();
            }
        }
        
        public P3Double SetTo(double d, AxisDirection dir)
        {
            switch (dir)
            {
                case AxisDirection.X:
                case AxisDirection.XNeg:
                    return new P3Double(
                        d,
                        Y,
                        Z);
                case AxisDirection.Y:
                case AxisDirection.YNeg:
                    return new P3Double(
                        X,
                        d,
                        Z);
                case AxisDirection.Z:
                case AxisDirection.ZNeg:
                    return new P3Double(
                        X,
                        Y,
                        d);
                case AxisDirection.XY:
                case AxisDirection.XYNeg:
                case AxisDirection.XNegY:
                case AxisDirection.XNegYNeg:
                    return new P3Double(
                        d,
                        d,
                        Z);
                case AxisDirection.YZ:
                case AxisDirection.YZNeg:
                case AxisDirection.YNegZ:
                case AxisDirection.YNegZNeg:
                    return new P3Double(
                        X,
                        d,
                        d);
                case AxisDirection.XZ:
                case AxisDirection.XZNeg:
                case AxisDirection.XNegZ:
                case AxisDirection.XNegZNeg:
                    return new P3Double(
                        d,
                        Y,
                        d);
                default:
                    throw new NotImplementedException();
            }
        }
        
        public P3Double Modify(double d, AxisDirection dir)
        {
            switch (dir)
            {
                case AxisDirection.X:
                case AxisDirection.XNeg:
                    return new P3Double(
                        X + d,
                        Y,
                        Z);
                case AxisDirection.Y:
                case AxisDirection.YNeg:
                    return new P3Double(
                        X,
                        Y + d,
                        Z);
                case AxisDirection.Z:
                case AxisDirection.ZNeg:
                    return new P3Double(
                        X,
                        Y,
                        Z + d);
                case AxisDirection.XY:
                case AxisDirection.XYNeg:
                case AxisDirection.XNegY:
                case AxisDirection.XNegYNeg:
                    return new P3Double(
                        X + d,
                        Y + d,
                        Z);
                case AxisDirection.YZ:
                case AxisDirection.YZNeg:
                case AxisDirection.YNegZ:
                case AxisDirection.YNegZNeg:
                    return new P3Double(
                        X,
                        Y + d,
                        Z + d);
                case AxisDirection.XZ:
                case AxisDirection.XZNeg:
                case AxisDirection.XNegZ:
                case AxisDirection.XNegZNeg:
                    return new P3Double(
                        X + d,
                        Y,
                        Z + d);
                default:
                    throw new NotImplementedException();
            }
        }

        public static bool TryParse(string str, out P3Double p3)
        {
            string[] split = str.Split(',');
            if (split.Length != 3)
            {
                p3 = default(P3Double);
                return false;
            }

            if (!double.TryParse(split[0], out double x))
            {
                p3 = default(P3Double);
                return false;
            }
            if (!double.TryParse(split[1], out double y))
            {
                p3 = default(P3Double);
                return false;
            }
            if (!double.TryParse(split[2], out double z))
            {
                p3 = default(P3Double);
                return false;
            }
            p3 = new P3Double(x, y, z);
            return true;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is P3Double rhs)) return false;
            return Equals(rhs);
        }

        public bool Equals(P3Double rhs)
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
            return $"({X}, {Y}, {Z})";
        }

        public static P3Double operator +(P3Double p1, P3Double p2)
        {
            return p1.Shift(p2);
        }

        public static P3Double operator +(P3Double p1, double num)
        {
            return new P3Double(p1.X + num, p1.Y + num, p1.Z + num);
        }

        public static P3Double operator -(P3Double p1, P3Double p2)
        {
            return new P3Double(p1.X - p2.X, p1.Y - p2.Y, p1.Z - p2.Z);
        }

        public static P3Double operator -(P3Double p1, double num)
        {
            return new P3Double(p1.X - num, p1.Y - num, p1.Z - num);
        }

        public static P3Double operator -(P3Double p1)
        {
            return new P3Double(-p1.X, -p1.Y, -p1.Z);
        }

        public static P3Double operator *(P3Double p1, int num)
        {
            return new P3Double(p1.X * num, p1.Y * num, p1.Z * num);
        }

        public static P3Double operator *(P3Double p1, double num)
        {
            return new P3Double(p1.X * num, p1.Y * num, p1.Z * num);
        }

        public static P3Double operator *(double num, P3Double p1)
        {
            return new P3Double(p1.X * num, p1.Y * num, p1.Z * num);
        }

        public static P3Double operator /(P3Double p1, double num)
        {
            return new P3Double(p1.X / num, p1.Y / num, p1.Z / num);
        }

        public static P3Double operator *(P3Double p1, P3Double p2)
        {
            return new P3Double(p1.X * p2.X, p1.Y * p2.Y, p1.Z * p2.Z);
        }

        public static bool operator ==(P3Double p1, P3Double p2)
        {
            return p1.X == p2.X && p1.Y == p2.Y && p1.Z == p2.Z;
        }

        public static bool operator !=(P3Double p1, P3Double p2)
        {
            return p1.X != p2.X || p1.Y != p2.Y || p1.Z != p2.Z;
        }
        
        public static explicit operator P2Double(P3Double point)
        {
            return new P2Double(point.X, point.Y);
        }
    }
}