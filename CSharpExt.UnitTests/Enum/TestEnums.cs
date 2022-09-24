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