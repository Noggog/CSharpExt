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