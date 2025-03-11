using System.Collections;
using System.IO.Abstractions;
using System.Runtime.InteropServices;
using Noggog;
using Noggog.Testing.AutoFixture;
using NSubstitute;
using Shouldly;

namespace CSharpExt.UnitTests.Structs.FileSystems;

public class FilePathTests
{
    static bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
    static string absPrefix = isWindows ? "C:\\" : "/";

    class TestData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator() => Data().Select(x => new object[] { x }).GetEnumerator();

        private IEnumerable<string> Data()
        {
            yield return Path.Combine(absPrefix, "Directory", "Test.txt"); // Absolute path
            yield return Path.Combine("Directory", "Test.txt"); // Relative path
            yield return "Test.txt"; // Just file name
            yield return Path.Combine("Directory", "Test"); // Relative file with no extension
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    class RelPathDirData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator() => Data().GetEnumerator();

        private IEnumerable<string[]> Data()
        {
            string from = Path.Combine(absPrefix, "Directory", "Text.txt");

            yield return new string[] {from, Path.Combine(absPrefix, "OtherDirectory"),
                Path.Combine("..", "Directory", "Text.txt")};
            yield return new string[] { from, Path.Combine(absPrefix, "Directory"), "Text.txt" };
            if (isWindows)
            {
                yield return new string[] { from, "D:\\Directory", "C:\\Directory\\Text.txt" };
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    class RelPathFileData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator() => Data().GetEnumerator();

        private IEnumerable<object[]> Data()
        {
            string from = Path.Combine(absPrefix, "Directory", "Text.txt");
            yield return new object[] {from ,
                Path.Combine(absPrefix, "OtherDirectory", "SomeFile.txt"),
                Path.Combine("..", "Directory", "Text.txt")};
            yield return new object[] { from, Path.Combine(absPrefix, "Directory", "SomeFile.txt"), "Text.txt" };
            if (isWindows)
            {
                yield return new object[] { from, "D:\\Directory\\SomeFile.txt", "C:\\Directory\\Text.txt" };
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    class UnderneathData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator() => Data().GetEnumerator();

        private IEnumerable<object[]> Data()
        {
            string from = Path.Combine(absPrefix, "Directory", "Text.txt");
            string from2 = Path.Combine(absPrefix, "Directory", "SubDir", "Text.txt");
            yield return new object[] { from, Path.Combine(absPrefix, "OtherDirectory"), false };
            yield return new object[] { from2, Path.Combine(absPrefix, "Directory"), true };
            yield return new object[] { from2, Path.Combine(absPrefix, "Directory", "SubDir"), true };
            yield return new object[] { from, Path.Combine(absPrefix, "Directory"), true };
            if (isWindows)
            {
                yield return new object[] { from, "D:\\Directory", false };
                yield return new object[] { "C:\\DIRECTORY\\Text.txt", "C:\\Directory", true };
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }


    [Theory]
    [ClassData(typeof(TestData))]
    [InlineData("")]
    public void ConstructionDoesNotThrow(string path)
    {
        new FilePath(path);
    }

    [Theory]
    [ClassData(typeof(TestData))]
    public void PathExposesAbsolutePath(string path)
    {
        new FilePath(path)
            .Path.ShouldBe(Path.GetFullPath(path));
    }

    [Fact]
    public void EmptyPathExposesEmptyPath()
    {
        new FilePath()
            .Path.ShouldBe(string.Empty);
    }

    [Theory]
    [ClassData(typeof(TestData))]
    [InlineData("")]
    public void RelativePathExposesGivenPath(string path)
    {
        new FilePath(path)
            .RelativePath.ShouldBe(path);
    }

    [Theory]
    [ClassData(typeof(TestData))]
    [InlineData("")]
    public void NameSameAsSystem(string path)
    {
        new FilePath(path)
            .Name.ShouldBe(new FileName(Path.GetFileName(path)));
    }

    [Theory]
    [ClassData(typeof(TestData))]
    [InlineData("")]
    public void ExtensionSameAsSystem(string path)
    {
        new FilePath(path)
            .Extension.ShouldBe(Path.GetExtension(path));
    }

    [Theory]
    [ClassData(typeof(TestData))]
    [InlineData("")]
    public void NameWithoutExtensionSameAsSystem(string path)
    {
        new FilePath(path)
            .NameWithoutExtension.ShouldBe(Path.GetFileNameWithoutExtension(path));
    }

    [Theory]
    [ClassData(typeof(TestData))]
    public void DirectorySameAsSystem(string path)
    {
        new FilePath(path)
            .Directory.ShouldBe(
                new DirectoryPath(Path.GetDirectoryName(Path.GetFullPath(path))!));
    }

    [Theory]
    [InlineData("")]
    public void EmptyDirectoryNull(string path)
    {
        new FilePath(path)
            .Directory.ShouldBeNull();
    }

    [Fact]
    public void DefaultFilePathSameAsEmpty()
    {
        new FilePath().ShouldBe(new FilePath(string.Empty));
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void CheckExists(bool shouldExist)
    {
        var path = Path.Combine(absPrefix, "SomeFile");
        var fs = Substitute.For<IFileSystem>();
        fs.File.Exists(path).Returns(shouldExist);
        new FilePath(path).CheckExists(fs).ShouldBe(shouldExist);
    }

    [Fact]
    public void PathAdjustsForwardSlashes()
    {
        if (isWindows)
        {
            new FilePath("C:/SomeFile")
                .Path.ShouldBe("C:\\SomeFile");
        }
    }

    [Fact]
    public void RelativePathAdjustsForwardSlashes()
    {
        if (isWindows)
        {
            new FilePath("SomeDir/SomeFile")
                .RelativePath.ShouldBe("SomeDir\\SomeFile");
        }
    }

    [Theory]
    [ClassData(typeof(TestData))]
    [InlineData("")]
    public void EqualsSelf(string path)
    {
        new FilePath(path).ShouldBe(new FilePath(path));
    }

    [Theory]
    [ClassData(typeof(TestData))]
    [InlineData("")]
    public void EqualsDifferentCase(string path)
    {
        new FilePath(path)
            .ShouldBe(new FilePath(path.ToUpper()));
    }

    [Fact]
    public void EmptyEqualsDefault()
    {
        new FilePath().ShouldBe(new FilePath(string.Empty));
    }

    [Theory]
    [ClassData(typeof(TestData))]
    [InlineData("")]
    public void DoesNotEqualRawPath(string path)
    {
        new FilePath(path).Equals((object)path).ShouldBeFalse();
    }

    [Theory]
    [ClassData(typeof(TestData))]
    [InlineData("")]
    public void HashEqualsSelf(string path)
    {
        var fp = new FilePath(path);
        fp.GetHashCode().ShouldBe(fp.GetHashCode());
    }

    [Theory]
    [ClassData(typeof(TestData))]
    [InlineData("")]
    public void HashEqualsDifferentCase(string path)
    {
        new FilePath(path).GetHashCode()
            .ShouldBe(new FilePath(path.ToUpper()).GetHashCode());
    }

    [Fact]
    public void EmptyHashEqualsDefaultHash()
    {
        new FilePath().GetHashCode()
            .ShouldBe(new FilePath(string.Empty).GetHashCode());
    }

    [Theory]
    [ClassData(typeof(RelPathDirData))]
    public void GetRelativePathToDirectory(string from, string to, string expected)
    {
        new FilePath(from)
            .GetRelativePathTo(new DirectoryPath(to))
            .ShouldBe(expected);
    }

    [Theory]
    [ClassData(typeof(RelPathFileData))]
    public void GetRelativePathToFile(string from, string to, string expected)
    {
        new FilePath(from)
            .GetRelativePathTo(new FilePath(to))
            .ShouldBe(expected);
    }

    [Theory]
    [ClassData(typeof(UnderneathData))]
    public void IsUnderneath(string from, string to, bool expected)
    {
        new FilePath(from)
            .IsUnderneath(new DirectoryPath(to))
            .ShouldBe(expected);
    }

    [Theory]
    [ClassData(typeof(TestData))]
    public void ToStringReturnsFullPath(string path)
    {
        new FilePath(path).ToString().ShouldBe(Path.GetFullPath(path));
    }

    [Fact]
    public void EmptyToStringReturnsEmpty()
    {
        new FilePath(string.Empty).ToString().ShouldBe(string.Empty);
    }

    [Fact]
    public void DefaultToStringReturnsEmpty()
    {
        new FilePath().ToString().ShouldBe(string.Empty);
    }

    [Theory]
    [ClassData(typeof(TestData))]
    [InlineData("")]
    public void ImplictOperatorEqualToCtor(string path)
    {
        FilePath fp = path;
        fp.Equals(new FilePath(path));
    }

    [Theory]
    [ClassData(typeof(TestData))]
    [InlineData("")]
    public void ImplicitOperatorFromEqualsRelative(string path)
    {
        var fp = new FilePath(path);
        string str = fp;
        str.ShouldBe(fp.RelativePath);
    }

    [Theory, DefaultAutoData]
    public void EqualOperatorOverload(
        FilePath file)
    {
        FilePath other = new FilePath(file.Path);
        Assert.True(file == other);
    }

    [Theory, DefaultAutoData]
    public void NotEqualOperatorOverload(
        FilePath file,
        FilePath file2)
    {
        Assert.True(file != file2);
    }

    [Theory, DefaultAutoData]
    public void NotEqualOperatorToDefault(
        FilePath file)
    {
        Assert.True(file != default);
    }
}