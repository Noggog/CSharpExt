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
        public static readonly P3UInt8 Origin = new(0, 0, 0);
        public static readonly P3UInt8 One = new(1, 1, 1);

        private byte _x;
        [DataMember]
        public byte X
        {
            get => _x;
            set => _x = value;
        }
        
        private byte _y;
        [DataMember]
        public byte Y
        {
            get => _y;
            set => _y = value;
        }
        
        private byte _z;
        [DataMember]
        public byte Z
        {
            get => _z;
            set => _z = value;
        }
        
        [IgnoreDataMember]
        public P3UInt8 Point => this;

        public P3UInt8(byte x, byte y, byte z)
        {
            _x = x;
            _y = y;
            _z = z;
        }

#if NETSTANDARD2_0
        public static bool TryParse(string str, out P3UInt8 ret)
        {
            // ToDo
            // Improve parsing to reduce allocation
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
#else 
        public static bool TryParse(ReadOnlySpan<char> str, out P3UInt8 ret)
        {
            // ToDo
            // Improve parsing to reduce allocation
            string[] split = str.ToString().Split(',');
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
#endif

        public override bool Equals(object? obj)
        {
            if (obj is not P3UInt8 rhs) return false;
            return Equals(rhs);
        }

        public bool Equals(P3UInt8 rhs)
        {
            return _x == rhs._x
                && _y == rhs._y
                && _z == rhs._z;
        }

        public override int GetHashCode() => HashCode.Combine(_x, _y, _z);

        public override string ToString()
        {
            return $"{_x}, {_y}, {_z}";
        }

        public string ToString(IFormatProvider? provider)
        {
            return $"{_x.ToString(provider)}, {_y.ToString(provider)}, {_z.ToString(provider)}";
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
            return new P3Double(point._x, point._y, point._z);
        }
    }