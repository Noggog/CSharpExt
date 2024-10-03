using System.Globalization;
using System.Runtime.Serialization;

namespace Noggog;

    public interface IP2UInt8Get
    {
        [DataMember]
        byte X { get; }
        [DataMember]
        byte Y { get; }
        [IgnoreDataMember]
        P2UInt8 Point { get; }
    }

    public struct P2UInt8 : IP2UInt8Get, IEquatable<P2UInt8>
    {
        public static readonly P2UInt8 Origin = new(0, 0);
        public static readonly P2UInt8 One = new(1, 1);

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
        
        [IgnoreDataMember]
        public P2UInt8 Point => this;

        public P2UInt8(byte x, byte y)
        {
            _x = x;
            _y = y;
        }

#if NETSTANDARD2_0
        public static bool TryParse(String str, out P2UInt8 ret, IFormatProvider? provider = null)
        {
            // ToDo
            // Improve parsing to reduce allocation
            string[] split = str.Split(',');
            if (split.Length != 2)
            {
                ret = default(P2UInt8);
                return false;
            }

            if (!byte.TryParse(split[0], NumberStyles.Any, provider, out var x)
                || !byte.TryParse(split[1], NumberStyles.Any, provider, out var y))
            {
                ret = default(P2UInt8);
                return false;
            }

            ret = new P2UInt8(x, y);
            return true;
        }
#else 
        public static bool TryParse(ReadOnlySpan<char> str, out P2UInt8 ret, IFormatProvider? provider = null)
        {
            // ToDo
            // Improve parsing to reduce allocation
            string[] split = str.ToString().Split(',');
            if (split.Length != 2)
            {
                ret = default(P2UInt8);
                return false;
            }

            if (!byte.TryParse(split[0], NumberStyles.Any, provider, out var x)
                || !byte.TryParse(split[1], NumberStyles.Any, provider, out var y))
            {
                ret = default(P2UInt8);
                return false;
            }

            ret = new P2UInt8(x, y);
            return true;
        }
#endif

        public override bool Equals(object? obj)
        {
            if (obj is not P2UInt8 rhs) return false;
            return Equals(rhs);
        }

        public bool Equals(P2UInt8 rhs)
        {
            return _x == rhs._x
                && _y == rhs._y;
        }

        public override int GetHashCode() => HashCode.Combine(_x, _y);

        public override string ToString()
        {
            return $"{_x}, {_y}";
        }

        public string ToString(IFormatProvider? provider)
        {
            return $"{_x.ToString(provider)}, {_y.ToString(provider)}";
        }

        public static bool operator ==(P2UInt8 obj1, P2UInt8 obj2)
        {
            return obj1.Equals(obj2);
        }

        public static bool operator !=(P2UInt8 obj1, P2UInt8 obj2)
        {
            return !obj1.Equals(obj2);
        }

        public static explicit operator P2Double(P2UInt8 point)
        {
            return new P2Double(point._x, point._y);
        }
    }