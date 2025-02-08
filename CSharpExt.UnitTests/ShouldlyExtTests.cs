using Noggog;
using Noggog.Testing.AutoFixture;
using Noggog.Testing.Extensions;
using Shouldly;

namespace CSharpExt.UnitTests;

public class ShouldlyExtTests
{
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
        bs.ShouldEqual((int)b, (int)b2);
    }

    [Theory, DefaultAutoData]
    public void ShouldEqualEnumerable(
        IEnumerable<byte> bytes)
    {
        bytes.ShouldEqual(bytes.Select(x => (int)x));
    }

    [Theory, DefaultAutoData]
    public void ShouldEqualArray(
        byte[] bytes)
    {
        bytes.ShouldEqual(bytes.Select(x => (int)x));
    }

    [Theory, DefaultAutoData]
    public void ShouldEqualStringEnumerable(
        IEnumerable<string> str)
    {
        str.ShouldEqual(str);
    }

    [Theory, DefaultAutoData]
    public void ShouldEqualStringArray(
        string str)
    {
        new[] { str }.ShouldEqual(str);
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
        paths.Select(x => x.Path).ShouldEqual(paths);
    }

    [Theory, DefaultAutoData]
    public void ShouldEqualFilePathStringArray(
        IEnumerable<FilePath> paths)
    {
        paths.Select(x => x.Path).ShouldEqual(paths.ToArray());
    }
}