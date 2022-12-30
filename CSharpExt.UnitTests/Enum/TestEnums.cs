namespace CSharpExt.UnitTests.Enum;

public enum EmptyTestEnum
{
}

[Flags]
public enum EmptyFlagsTestEnum
{
}

public enum TestEnum
{
    First = 1,
    Second = 2,
    Third = 150,
}

[Flags]
public enum FlagsTestEnum
{
    One = 0x01,
    Two = 0x02,
    Four = 0x04,
}

[Flags]
public enum UlongFlagsTestEnum : ulong
{
    One = 0x01,
    Two = 0x02,
    Four = 0x04,
    Max = 0x8000000000000000,
}

[Flags]
public enum LongFlagsTestEnum : long
{
    One = 0x01,
    Two = 0x02,
    Four = 0x04,
    Max = 0x4000000000000000
}

public enum LongEnum : long
{
    First = 1,
    Second = 2,
    Third = 150,
    Last = long.MaxValue,
    Around = long.MinValue,
}

public enum ULongEnum : ulong
{
    First = 1,
    Second = 2,
    Third = 150,
    Last = ulong.MaxValue,
}

public enum ByteEnum : byte
{
    First = 1,
    Second = 2,
    Third = 150,
}