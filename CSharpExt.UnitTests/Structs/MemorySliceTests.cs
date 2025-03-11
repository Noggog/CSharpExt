﻿using Noggog;
using Shouldly;

namespace CSharpExt.UnitTests.Structs;

public class MemorySliceTests
{
    [Fact]
    public void DefaultToArray()
    {
        var sut = default(MemorySlice<byte>);
        sut.ToArray().ShouldBeEmpty();
    }
    
    [Fact]
    public void ReadonlyDefaultToArray()
    {
        var sut = default(ReadOnlyMemorySlice<byte>);
        sut.ToArray().ShouldBeEmpty();
    }
    
    [Fact]
    public void DefaultZeroLengthToArray()
    {
        var sut = new MemorySlice<byte>(new byte[6], 4, 0);
        sut.ToArray().ShouldBeEmpty();
    }
    
    [Fact]
    public void ReadonlyZeroLengthDefaultToArray()
    {
        var sut = new ReadOnlyMemorySlice<byte>(new byte[6], 4, 0);
        sut.ToArray().ShouldBeEmpty();
    }
}