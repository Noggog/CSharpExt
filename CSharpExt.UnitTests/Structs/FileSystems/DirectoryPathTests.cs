using System.Collections;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Runtime.InteropServices;
using FluentAssertions;
using Noggog;
using NSubstitute;
using Xunit;

namespace CSharpExt.UnitTests.Structs.FileSystems;

public class DirectoryPathTests
{
    static bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
    static string absPrefix = isWindows ? "C:\\" : "/";

    class TestData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator() => Data().Select(x => new object[] { x }).GetEnumerator();

        private IEnumerable<string> Data()
        {
            yield return Path.Combine("Directory", "Test.txt"); // Relative might be file
            yield return Path.Combine("Directory", "Test"); // Relative
            yield return Path.Combine("..", "Test"); // Upwards navigation
            yield return Path.Combine(absPrefix, "Directory", "Test"); // Absolute
            yield return "Test"; // Just name
            yield return "Test.txt"; // Just name might be file
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    [Theory]
    [ClassData(typeof(TestData))]
    [InlineData("")]
    public void ConstructionDoesNotThrow(string path)
    {
        new DirectoryPath(path);
    }

    [Theory]
    [ClassData(typeof(TestData))]
    public void PathExposesAbsolutePath(string path)
    {
        new DirectoryPath(path)
            .Path.Should().Be(Path.GetFullPath(path));
    }

    [Fact]
    public void EmptyPathExposesEmptyPath()
    {
        new DirectoryPath()
            .Path.Should().Be(string.Empty);
    }

    [Theory]
    [ClassData(typeof(TestData))]
    [InlineData("")]
    public void RelativePathExposesGivenPath(string path)
    {
        new DirectoryPath(path)
            .RelativePath.Should().Be(path);
    }

    [Theory]
    [ClassData(typeof(TestData))]
    [InlineData("")]
    public void NameSameAsSystem(string path)
    {
        new DirectoryPath(path)
            .Name.Should().Be(new FileName(Path.GetFileName(path)));
    }

    [Theory]
    [ClassData(typeof(TestData))]
    public void DirectorySameAsSystem(string path)
    {
        new DirectoryPath(path)
            .Directory.Should().Be(
                new DirectoryPath(Path.GetDirectoryName(Path.GetFullPath(path))!));
    }

    [Theory]
    [ClassData(typeof(TestData))]
    public void GetFileSameAsSystem(string path)
    {
        var dir = new DirectoryPath(path);
        dir.GetFile("Text.txt")
            .Should().Be(
                new FilePath(Path.Combine(path, "Text.txt")));
    }

    [Theory]
    [InlineData("")]
    public void EmptyDirectoryNull(string path)
    {
        new DirectoryPath(path)
            .Directory.Should().BeNull();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void CheckExists(bool shouldExist)
    {
        var path = absPrefix + "SomeFolder";
        var fs = Substitute.For<IFileSystem>();
        fs.Directory.Exists(path).Returns(shouldExist);
        new DirectoryPath(path).CheckExists(fs).Should().Be(shouldExist);
    }

    [Fact]
    public void PathAdjustsForwardSlashes()
    {
        if (isWindows)
        {
            new DirectoryPath(absPrefix + "SomeDir")
                .Path.Should().Be("C:\\SomeDir");
        }
    }

    [Fact]
    public void RelativePathAdjustsForwardSlashes()
    {
        if (isWindows)
        {
            new DirectoryPath("SomeDir/SubDir")
                .RelativePath.Should().Be("SomeDir\\SubDir");
        }
    }

    [Fact]
    public void PathTrimsTrailingBackSlashes()
    {
        if (isWindows)
        {
            new DirectoryPath("C:\\SomeDir\\")
                .Path.Should().Be("C:\\SomeDir");
        }
    }

    [Fact]
    public void RelativePathTrimsTrailingBackSlashes()
    {
        if (isWindows)
        {
            new DirectoryPath("SomeDir\\SubDir\\")
                .RelativePath.Should().Be("SomeDir\\SubDir");
        }
    }

    [Fact]
    public void PathTrimsTrailingForwardSlashes()
    {
        new DirectoryPath(absPrefix + "SomeDir/")
            .Path.Should().Be(Path.Combine(absPrefix, "SomeDir"));
    }

    [Fact]
    public void RelativePathTrimsTrailingForwardSlashes()
    {
        new DirectoryPath("SomeDir/SubDir/")
            .RelativePath.Should().Be(Path.Combine("SomeDir", "SubDir"));
    }

    [Theory]
    [ClassData(typeof(TestData))]
    [InlineData("")]
    public void EqualsSelf(string path)
    {
        new DirectoryPath(path).Should().Be(new DirectoryPath(path));
    }

    [Theory]
    [ClassData(typeof(TestData))]
    [InlineData("")]
    public void EqualsDifferentCase(string path)
    {
        new DirectoryPath(path)
            .Should().Be(new DirectoryPath(path.ToUpper()));
    }

    [Fact]
    public void EmptyEqualsDefault()
    {
        new DirectoryPath().Should().Be(new DirectoryPath(string.Empty));
    }

    [Theory]
    [ClassData(typeof(TestData))]
    [InlineData("")]
    public void DoesNotEqualRawPath(string path)
    {
        new DirectoryPath(path).Should().NotBe(path);
    }

    [Theory]
    [ClassData(typeof(TestData))]
    [InlineData("")]
    public void HashEqualsSelf(string path)
    {
        var fp = new DirectoryPath(path);
        fp.GetHashCode().Should().Be(fp.GetHashCode());
    }

    [Theory]
    [ClassData(typeof(TestData))]
    [InlineData("")]
    public void HashEqualsDifferentCase(string path)
    {
        new DirectoryPath(path).GetHashCode()
            .Should().Be(new DirectoryPath(path.ToUpper()).GetHashCode());
    }

    [Fact]
    public void EmptyHashEqualsDefaultHash()
    {
        new DirectoryPath().GetHashCode()
            .Should().Be(new DirectoryPath(string.Empty).GetHashCode());
    }

    [Theory]
    [ClassData(typeof(TestData))]
    public void ToStringReturnsFullPath(string path)
    {
        new DirectoryPath(path).ToString().Should().Be(Path.GetFullPath(path));
    }

    [Fact]
    public void EmptyToStringReturnsEmpty()
    {
        new DirectoryPath(string.Empty).ToString().Should().Be(string.Empty);
    }

    [Fact]
    public void DefaultToStringReturnsEmpty()
    {
        new DirectoryPath().ToString().Should().Be(string.Empty);
    }

    [Theory]
    [ClassData(typeof(TestData))]
    [InlineData("")]
    public void ImplictOperatorEqualToCtor(string path)
    {
        DirectoryPath dir = path;
        dir.Should().Be(new DirectoryPath(path));
    }

    [Theory]
    [ClassData(typeof(TestData))]
    [InlineData("")]
    public void ImplicitOperatorFromEqualsRelative(string path)
    {
        var dir = new DirectoryPath(path);
        string str = dir;
        str.Should().Be(dir.RelativePath);
    }

    [Theory]
    [ClassData(typeof(TestData))]
    [InlineData("")]
    public void SpanImplicitOperatorFromEqualsRelative(string path)
    {
        var dir = new DirectoryPath(path);
        ReadOnlySpan<char> str = dir;
        Assert.True(str == dir.RelativePath);
    }

    [Theory]
    [InlineData("C:\\Directory\\Dir", "C:\\OtherDirectory", false)]
    [InlineData("C:\\Directory\\SubDir\\Dir", "C:\\Directory", true)]
    [InlineData("C:\\Directory\\SubDir\\Dir", "C:\\Directory\\SubDir", true)]
    [InlineData("C:\\Directory\\Dir", "D:\\Directory", false)]
    [InlineData("C:\\Directory\\Dir", "C:\\Directory", true)]
    [InlineData("C:\\DIRECTORY\\Dir", "C:\\Directory", true)]
    public void IsSubfolderOf(string from, string to, bool expected)
    {
        // TODO: Fix test to work on Linux
        if (isWindows)
        {
            new DirectoryPath(from)
                .IsSubfolderOf(new DirectoryPath(to))
                .Should().Be(expected);
        }
    }

    public MockFileSystem GetMockFileSystem()
    {
        return new MockFileSystem(new Dictionary<string, MockFileData>()
        {
            { absPrefix + "SomeDir/SubDir/SomeFile.txt", string.Empty },
            { absPrefix + "SomeDir/SubDir/SomeFile2.txt", string.Empty },
            { absPrefix + "SomeDir/SubDir/SubSubDir/SomeFile2.txt", string.Empty },
            { absPrefix + "SomeDir/SomeFile.txt", string.Empty },
            { absPrefix + "SomeDir/SomeFile2.txt", string.Empty },
            { absPrefix + "SomeFile.txt", string.Empty },
            { absPrefix + "SomeOtherDir/SomeFile2.txt", string.Empty },
        });
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void DeleteEntireFolder(bool locked)
    {
        var fs = GetMockFileSystem();

        if (locked)
        {
            foreach (var file in fs.AllFiles)
            {
                fs.File.SetAttributes(file, fs.File.GetAttributes(file).SetFlag(FileAttributes.ReadOnly, true));
            }
        }

        new DirectoryPath(absPrefix + "SomeDir")
            .DeleteEntireFolder(fileSystem: fs, disableReadOnly: locked);

        fs.File.Exists(absPrefix + "SomeDir/SubDir/SomeFile.txt").Should().BeFalse();
        fs.File.Exists(absPrefix + "SomeDir/SubDir/SomeFile2.txt").Should().BeFalse();
        fs.File.Exists(absPrefix + "SomeDir/SubDir/SubSubDir/SomeFile2.txt").Should().BeFalse();
        fs.File.Exists(absPrefix + "SomeDir/SomeFile.txt").Should().BeFalse();
        fs.File.Exists(absPrefix + "SomeDir/SomeFile2.txt").Should().BeFalse();
        fs.File.Exists(absPrefix + "SomeFile.txt").Should().BeTrue();
        fs.File.Exists(absPrefix + "SomeOtherDir/SomeFile2.txt").Should().BeTrue();

        fs.Directory.Exists(absPrefix + "SomeOtherDir").Should().BeTrue();
        fs.Directory.Exists(absPrefix + "SomeDir").Should().BeFalse();
    }

    [Fact]
    public void DeleteEntireFolderExceptSelf()
    {
        var fs = GetMockFileSystem();

        new DirectoryPath(absPrefix + "SomeDir")
            .DeleteEntireFolder(fileSystem: fs, deleteFolderItself: false);

        fs.File.Exists(absPrefix + "SomeDir/SubDir/SomeFile.txt").Should().BeFalse();
        fs.File.Exists(absPrefix + "SomeDir/SubDir/SomeFile2.txt").Should().BeFalse();
        fs.File.Exists(absPrefix + "SomeDir/SubDir/SubSubDir/SomeFile2.txt").Should().BeFalse();
        fs.File.Exists(absPrefix + "SomeDir/SomeFile.txt").Should().BeFalse();
        fs.File.Exists(absPrefix + "SomeDir/SomeFile2.txt").Should().BeFalse();
        fs.File.Exists(absPrefix + "SomeFile.txt").Should().BeTrue();
        fs.File.Exists(absPrefix + "SomeOtherDir/SomeFile2.txt").Should().BeTrue();

        fs.Directory.Exists(absPrefix + "SomeOtherDir").Should().BeTrue();
        fs.Directory.Exists(absPrefix + "SomeDir").Should().BeTrue();
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void TryDeleteEntireFolder(bool locked)
    {
        var fs = GetMockFileSystem();

        if (locked)
        {
            foreach (var file in fs.AllFiles)
            {
                fs.File.SetAttributes(file, fs.File.GetAttributes(file).SetFlag(FileAttributes.ReadOnly, true));
            }
        }

        new DirectoryPath(absPrefix + "SomeDir")
            .TryDeleteEntireFolder(fileSystem: fs, disableReadOnly: locked);

        fs.File.Exists(absPrefix + "SomeDir/SubDir/SomeFile.txt").Should().BeFalse();
        fs.File.Exists(absPrefix + "SomeDir/SubDir/SomeFile2.txt").Should().BeFalse();
        fs.File.Exists(absPrefix + "SomeDir/SubDir/SubSubDir/SomeFile2.txt").Should().BeFalse();
        fs.File.Exists(absPrefix + "SomeDir/SomeFile.txt").Should().BeFalse();
        fs.File.Exists(absPrefix + "SomeDir/SomeFile2.txt").Should().BeFalse();
        fs.File.Exists(absPrefix + "SomeFile.txt").Should().BeTrue();
        fs.File.Exists(absPrefix + "SomeOtherDir/SomeFile2.txt").Should().BeTrue();

        fs.Directory.Exists(absPrefix + "SomeOtherDir").Should().BeTrue();
        fs.Directory.Exists(absPrefix + "SomeDir").Should().BeFalse();
    }

    [Fact]
    public void TryDeleteEntireFolderExceptSelf()
    {
        var fs = GetMockFileSystem();

        new DirectoryPath(absPrefix + "SomeDir")
            .TryDeleteEntireFolder(fileSystem: fs, deleteFolderItself: false);

        fs.File.Exists(absPrefix + "SomeDir/SubDir/SomeFile.txt").Should().BeFalse();
        fs.File.Exists(absPrefix + "SomeDir/SubDir/SomeFile2.txt").Should().BeFalse();
        fs.File.Exists(absPrefix + "SomeDir/SubDir/SubSubDir/SomeFile2.txt").Should().BeFalse();
        fs.File.Exists(absPrefix + "SomeDir/SomeFile.txt").Should().BeFalse();
        fs.File.Exists(absPrefix + "SomeDir/SomeFile2.txt").Should().BeFalse();
        fs.File.Exists(absPrefix + "SomeFile.txt").Should().BeTrue();
        fs.File.Exists(absPrefix + "SomeOtherDir/SomeFile2.txt").Should().BeTrue();

        fs.Directory.Exists(absPrefix + "SomeOtherDir").Should().BeTrue();
        fs.Directory.Exists(absPrefix + "SomeDir").Should().BeTrue();
    }

    [Fact]
    public void DeleteEntireFolderLockedThrows()
    {
        var fs = GetMockFileSystem();
        foreach (var file in fs.AllFiles)
        {
            fs.File.SetAttributes(file, fs.File.GetAttributes(file).SetFlag(FileAttributes.ReadOnly, true));
        }

        Assert.Throws<UnauthorizedAccessException>(() =>
        {
            new DirectoryPath(absPrefix + "SomeDir")
                .DeleteEntireFolder(fileSystem: fs, disableReadOnly: false);
        });
    }

    [Fact]
    public void DeleteEntireFolderLockedDeletesWhatItCan()
    {
        var fs = GetMockFileSystem();
        foreach (var file in fs.AllFiles)
        {
            if (new FilePath(file).Equals(absPrefix + "SomeDir/SubDir/SomeFile2.txt")) continue;
            fs.File.SetAttributes(file, fs.File.GetAttributes(file).SetFlag(FileAttributes.ReadOnly, true));
        }

        Assert.Throws<UnauthorizedAccessException>(() =>
        {
            new DirectoryPath(absPrefix + "SomeDir")
                .DeleteEntireFolder(fileSystem: fs, disableReadOnly: false);
        });

        fs.File.Exists(absPrefix + "SomeDir/SubDir/SomeFile.txt").Should().BeTrue();
        fs.File.Exists(absPrefix + "SomeDir/SubDir/SomeFile2.txt").Should().BeFalse();
        fs.File.Exists(absPrefix + "SomeDir/SubDir/SubSubDir/SomeFile2.txt").Should().BeTrue();
        fs.File.Exists(absPrefix + "SomeDir/SomeFile.txt").Should().BeTrue();
        fs.File.Exists(absPrefix + "SomeDir/SomeFile2.txt").Should().BeTrue();
        fs.File.Exists(absPrefix + "SomeFile.txt").Should().BeTrue();
        fs.File.Exists(absPrefix + "SomeOtherDir/SomeFile2.txt").Should().BeTrue();

        fs.Directory.Exists(absPrefix + "SomeOtherDir").Should().BeTrue();
        fs.Directory.Exists(absPrefix + "SomeDir").Should().BeTrue();
    }

    [Fact]
    public void TryDeleteEntireFolderLockedDoesNotThrow()
    {
        var fs = GetMockFileSystem();
        foreach (var file in fs.AllFiles)
        {
            fs.File.SetAttributes(file, fs.File.GetAttributes(file).SetFlag(FileAttributes.ReadOnly, true));
        }

        new DirectoryPath(absPrefix + "SomeDir")
            .TryDeleteEntireFolder(fileSystem: fs, disableReadOnly: false);
    }

    [Fact]
    public void TryDeleteEntireFolderDeletesWhatItCan()
    {
        var fs = GetMockFileSystem();
        foreach (var file in fs.AllFiles)
        {
            if (new FilePath(file).Equals(absPrefix + "SomeDir/SubDir/SomeFile2.txt")) continue;
            fs.File.SetAttributes(file, fs.File.GetAttributes(file).SetFlag(FileAttributes.ReadOnly, true));
        }

        new DirectoryPath(absPrefix + "SomeDir")
            .TryDeleteEntireFolder(fileSystem: fs, disableReadOnly: false);

        fs.File.Exists(absPrefix + "SomeDir/SubDir/SomeFile.txt").Should().BeTrue();
        fs.File.Exists(absPrefix + "SomeDir/SubDir/SomeFile2.txt").Should().BeFalse();
        fs.File.Exists(absPrefix + "SomeDir/SubDir/SubSubDir/SomeFile2.txt").Should().BeTrue();
        fs.File.Exists(absPrefix + "SomeDir/SomeFile.txt").Should().BeTrue();
        fs.File.Exists(absPrefix + "SomeDir/SomeFile2.txt").Should().BeTrue();
        fs.File.Exists(absPrefix + "SomeFile.txt").Should().BeTrue();
        fs.File.Exists(absPrefix + "SomeOtherDir/SomeFile2.txt").Should().BeTrue();

        fs.Directory.Exists(absPrefix + "SomeOtherDir").Should().BeTrue();
        fs.Directory.Exists(absPrefix + "SomeDir").Should().BeTrue();
    }

    [Fact]
    public void CreateMakesFolder()
    {
        var fs = new MockFileSystem();
        var someDir = absPrefix + "SomeDir";
        new DirectoryPath(someDir)
            .Create(fs);
        fs.Directory.Exists(someDir).Should().BeTrue();
    }

    [Fact]
    public void DeleteDestroysFolder()
    {
        var fs = new MockFileSystem();
        var someDir = absPrefix + "SomeDir";
        fs.Directory.CreateDirectory(someDir);
        new DirectoryPath(someDir)
            .Delete(fs);
        fs.Directory.Exists(someDir).Should().BeFalse();
    }

    [Fact]
    public void CheckEmptyIsEmpty()
    {
        var fs = new MockFileSystem();
        var someDir = absPrefix + "SomeDir";
        fs.Directory.CreateDirectory(absPrefix + "SomeDir");
        new DirectoryPath(someDir)
            .CheckEmpty(fs)
            .Should().BeTrue();
    }

    [Fact]
    public void CheckEmptyWithFileReturnsFalse()
    {
        var fs = new MockFileSystem();
        var someDir = absPrefix + "SomeDir";
        fs.Directory.CreateDirectory(absPrefix + "SomeDir");
        fs.File.Create(Path.Combine(someDir, "SomeFile"));
        new DirectoryPath(someDir)
            .CheckEmpty(fs)
            .Should().BeFalse();
    }

    [Fact]
    public void CheckEmptyOnFileThrows()
    {
        var fs = new MockFileSystem();
        var someDir = absPrefix + "SomeDir";
        fs.Directory.CreateDirectory(absPrefix + "SomeDir");
        var filePath = Path.Combine(someDir, "SomeFile");
        fs.File.Create(filePath);
        Assert.Throws<IOException>(() =>
        {
            new DirectoryPath(filePath)
                .CheckEmpty(fs);
        });
    }

    [Fact]
    public void CheckEmptyWithDirectoryReturnsFalse()
    {
        var fs = new MockFileSystem();
        var someDir = absPrefix + "SomeDir";
        fs.Directory.CreateDirectory(absPrefix + "SomeDir");
        fs.Directory.CreateDirectory(Path.Combine(someDir, "SubDir"));
        new DirectoryPath(someDir)
            .CheckEmpty(fs)
            .Should().BeFalse();
    }

    [Theory]
    [InlineData("C:\\Directory\\Text.txt", "C:\\OtherDirectory", "..\\Directory\\Text.txt")]
    [InlineData("C:\\Directory\\Text.txt", "C:\\Directory", "Text.txt")]
    [InlineData("C:\\Directory\\Text.txt", "D:\\Directory", "C:\\Directory\\Text.txt")]
    public void GetRelativePathToDirectory(string from, string to, string expected)
    {
        // TODO: Fix test to work on Linux
        if (isWindows)
        {
            new DirectoryPath(from)
                .GetRelativePathTo(new DirectoryPath(to))
                .Should().Be(expected);
        }
    }

    [Theory]
    [InlineData("C:\\Directory\\Text.txt", "C:\\OtherDirectory\\SomeFile.txt", "..\\Directory\\Text.txt")]
    [InlineData("C:\\Directory\\Text.txt", "C:\\Directory\\SomeFile.txt", "Text.txt")]
    [InlineData("C:\\Directory\\Text.txt", "D:\\Directory\\SomeFile.txt", "C:\\Directory\\Text.txt")]
    public void GetRelativePathToFile(string from, string to, string expected)
    {
        // TODO: Fix test to work on Linux
        if (isWindows)
        {
            new DirectoryPath(from)
                .GetRelativePathTo(new FilePath(to))
                .Should().Be(expected);
        }
    }

    [Theory, BasicAutoData]
    public void EqualOperatorOverload(
        DirectoryPath dir)
    {
        DirectoryPath other = new DirectoryPath(dir.Path);
        Assert.True(dir == other);
    }

    [Theory, BasicAutoData]
    public void NotEqualOperatorOverload(
        DirectoryPath dir,
        DirectoryPath dir2)
    {
        Assert.True(dir != dir2);
    }

    [Theory, BasicAutoData]
    public void NotEqualOperatorToDefault(
        DirectoryPath dir)
    {
        Assert.True(dir != default);
    }
}