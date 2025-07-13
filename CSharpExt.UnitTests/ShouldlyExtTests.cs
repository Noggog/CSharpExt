using Noggog;
using Noggog.Testing.AutoFixture;
using Noggog.Testing.Extensions;
using Shouldly;

namespace CSharpExt.UnitTests;

public class ShouldlyExtTests
{
    [Theory, DefaultAutoData]
    public void RoughlyEqual(
        byte b)
    {
        ShouldlyExt.RoughlyEqual(b, (int)b).ShouldBeTrue();
    }

    [Theory, DefaultAutoData]
    public void RoughlyEqualFalse(
        byte b)
    {
        ShouldlyExt.RoughlyEqual(b, (int)(b + 1)).ShouldBeFalse();
    }

    [Theory, DefaultAutoData]
    public void ShouldEqual(
        byte b)
    {
        b.ShouldEqual((int)b);
    }
    
    [Theory, DefaultAutoData]
    public void ShouldEqualParams(
        byte b,
        byte b2)
    {
        byte[] bs = [b, b2];
        bs.ShouldEqualEnumerable((int)b, (int)b2);
    }

    [Theory, DefaultAutoData]
    public void ShouldEqualEnumerable(
        IEnumerable<byte> bytes)
    {
        bytes.ShouldEqualEnumerable(bytes.Select(x => (int)x));
    }

    [Theory, DefaultAutoData]
    public void ShouldEqualArray(
        byte[] bytes)
    {
        bytes.ShouldEqualEnumerable(bytes.Select(x => (int)x));
    }

    [Theory, DefaultAutoData]
    public void ShouldEqualStringEnumerable(
        IEnumerable<string> str)
    {
        str.ShouldEqualEnumerable(str);
    }

    [Theory, DefaultAutoData]
    public void ShouldEqualStringArray(
        string str)
    {
        new[] { str }.ShouldEqualEnumerable(str);
    }

    [Theory, DefaultAutoData]
    public void ShouldEqualFilePathString(
        FilePath path)
    {
        path.ShouldEqual(path.Path);
        path.Path.ShouldEqual(path);
    }

    [Theory, DefaultAutoData]
    public void ShouldEqualFilePathStringEnumerable(
        IEnumerable<FilePath> paths)
    {
        paths.Select(x => x.Path).ShouldEqualEnumerable(paths);
    }

    [Theory, DefaultAutoData]
    public void ShouldEqualFilePathStringArray(
        IEnumerable<FilePath> paths)
    {
        paths.Select(x => x.Path).ShouldEqualEnumerable(paths.ToArray());
    }

    [Fact]
    public void ShouldEqualEnumerableMemorySlice()
    {
        ReadOnlyMemorySlice<byte> b1 = new byte[] { 1, 2, 3 };
        ReadOnlyMemorySlice<byte> b2 = new byte[] { 1, 2, 3 };
        b1.ShouldEqualEnumerable(b2);
    }

    [Fact]
    public void ShouldEqualEnumerableMemorySliceNullable()
    {
        ReadOnlyMemorySlice<byte> b1 = new byte[] { 1, 2, 3 };
        ReadOnlyMemorySlice<byte>? b2 = new byte[] { 1, 2, 3 };
        b1.ShouldEqualEnumerable(b2);
    }
}