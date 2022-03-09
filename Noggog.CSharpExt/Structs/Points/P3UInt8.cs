using System;
using System.Runtime.Serialization;

namespace Noggog;

    public interface IP3UInt8Get
    {
        [DataMember]
        byte X { get; }
        [DataMember]
        byte Y { get; }
        [DataMember]
        byte Z { get; }
        [IgnoreDataMember]
        P3UInt8 Point { get; }
    }

    public struct P3UInt8 : IP3UInt8Get, IEquatable<P3UInt8>
    {
        public static readonly P3UInt8 Origin = new P3UInt8(0, 0, 0);
        public static readonly P3UInt8 One = new P3UInt8(1, 1, 1);

        [DataMember]
        public readonly byte X;
        [DataMember]
        public readonly byte Y;
        [DataMember]
        public readonly byte Z;

        [IgnoreDataMember]
        byte IP3UInt8Get.X => this.X;
        [IgnoreDataMember]
        byte IP3UInt8Get.Y => this.Y;
        [IgnoreDataMember]
        byte IP3UInt8Get.Z => this.Z;
        [IgnoreDataMember]
        public P3UInt8 Point => this;

        public P3UInt8(byte x, byte y, byte z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public static bool TryParse(string str, out P3UInt8 ret)
        {
            string[] split = str.Split(',');
            if (split.Length != 3)
            {
                ret = default(P3UInt8);
                return false;
            }

            if (!byte.TryParse(split[0], out var x)
                || !byte.TryParse(split[1], out var y)
                || !byte.TryParse(split[2], out var z))
            {
                ret = default(P3UInt8);
                return false;
            }

            ret = new P3UInt8(x, y, z);
            return true;
        }

        public override bool Equals(object? obj)
        {
            if (obj is not P3UInt8 rhs) return false;
            return Equals(rhs);
        }

        public bool Equals(P3UInt8 rhs)
        {
            return this.X == rhs.X
                && this.Y == rhs.Y
                && this.Z == rhs.Z;
        }

        public override int GetHashCode() => HashCode.Combine(X, Y, Z);

        public override string ToString()
        {
            return $"({X},{Y},{Z}";
        }

        public static bool operator ==(P3UInt8 obj1, P3UInt8 obj2)
        {
            return obj1.Equals(obj2);
        }

        public static bool operator !=(P3UInt8 obj1, P3UInt8 obj2)
        {
            return !obj1.Equals(obj2);
        }

        public static explicit operator P3Double(P3UInt8 point)
        {
            return new P3Double(point.X, point.Y, point.Z);
        }
    }