﻿using Noggog;
using Noggog.Testing.Extensions;
using Shouldly;

namespace CSharpExt.UnitTests.Enum;

public class ValuesTests
{
    [Fact]
    public void Typical()
    {
        Enums<TestEnum>.Values.ShouldBe(
            TestEnum.First,
            TestEnum.Second,
            TestEnum.Third);
    }

    [Fact]
    public void Empty()
    {
        Enums<EmptyTestEnum>.Values.ShouldBeEmpty();
    }

    [Fact]
    public void TypicalFlags()
    {
        Enums<FlagsTestEnum>.Values.ShouldBe(
            FlagsTestEnum.One,
            FlagsTestEnum.Two,
            FlagsTestEnum.Four);
    }

    [Fact]
    public void EmptyFlags()
    {
        Enums<EmptyFlagsTestEnum>.Values.ShouldBeEmpty();
    }
}