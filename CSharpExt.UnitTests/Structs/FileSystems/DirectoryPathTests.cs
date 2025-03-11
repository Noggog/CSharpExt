using System.Collections;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Runtime.InteropServices;
using Noggog;
using Noggog.Testing.AutoFixture;
using NSubstitute;
using Shouldly;

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
            .Path.ShouldBe(Path.GetFullPath(path));
    }

    [Fact]
    public void EmptyPathExposesEmptyPath()
    {
        new DirectoryPath()
            .Path.ShouldBe(string.Empty);
    }

    [Theory]
    [ClassData(typeof(TestData))]
    [InlineData("")]
    public void RelativePathExposesGivenPath(string path)
    {
        new DirectoryPath(path)
            .RelativePath.ShouldBe(path);
    }

    [Theory]
    [ClassData(typeof(TestData))]
    [InlineData("")]
    public void NameSameAsSystem(string path)
    {
        new DirectoryPath(path)
            .Name.ShouldBe(new FileName(Path.GetFileName(path)));
    }

    [Theory]
    [ClassData(typeof(TestData))]
    public void DirectorySameAsSystem(string path)
    {
        new DirectoryPath(path)
            .Directory.ShouldBe(
                new DirectoryPath(Path.GetDirectoryName(Path.GetFullPath(path))!));
    }

    [Theory]
    [ClassData(typeof(TestData))]
    public void GetFileSameAsSystem(string path)
    {
        var dir = new DirectoryPath(path);
        dir.GetFile("Text.txt")
            .ShouldBe(
                new FilePath(Path.Combine(path, "Text.txt")));
    }

    [Theory]
    [InlineData("")]
    public void EmptyDirectoryNull(string path)
    {
        new DirectoryPath(path)
            .Directory.ShouldBeNull();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void CheckExists(bool shouldExist)
    {
        var path = absPrefix + "SomeFolder";
        var fs = Substitute.For<IFileSystem>();
        fs.Directory.Exists(path).Returns(shouldExist);
        new DirectoryPath(path).CheckExists(fs).ShouldBe(shouldExist);
    }

    [Fact]
    public void PathAdjustsForwardSlashes()
    {
        if (isWindows)
        {
            new DirectoryPath(absPrefix + "SomeDir")
                .Path.ShouldBe("C:\\SomeDir");
        }
    }

    [Fact]
    public void RelativePathAdjustsForwardSlashes()
    {
        if (isWindows)
        {
            new DirectoryPath("SomeDir/SubDir")
                .RelativePath.ShouldBe("SomeDir\\SubDir");
        }
    }

    [Fact]
    public void PathTrimsTrailingBackSlashes()
    {
        if (isWindows)
        {
            new DirectoryPath("C:\\SomeDir\\")
                .Path.ShouldBe("C:\\SomeDir");
        }
    }

    [Fact]
    public void RelativePathTrimsTrailingBackSlashes()
    {
        if (isWindows)
        {
            new DirectoryPath("SomeDir\\SubDir\\")
                .RelativePath.ShouldBe("SomeDir\\SubDir");
        }
    }

    [Fact]
    public void PathTrimsTrailingForwardSlashes()
    {
        new DirectoryPath(absPrefix + "SomeDir/")
            .Path.ShouldBe(Path.Combine(absPrefix, "SomeDir"));
    }

    [Fact]
    public void RelativePathTrimsTrailingForwardSlashes()
    {
        new DirectoryPath("SomeDir/SubDir/")
            .RelativePath.ShouldBe(Path.Combine("SomeDir", "SubDir"));
    }

    [Theory]
    [ClassData(typeof(TestData))]
    [InlineData("")]
    public void EqualsSelf(string path)
    {
        new DirectoryPath(path).ShouldBe(new DirectoryPath(path));
    }

    [Theory]
    [ClassData(typeof(TestData))]
    [InlineData("")]
    public void EqualsDifferentCase(string path)
    {
        new DirectoryPath(path)
            .ShouldBe(new DirectoryPath(path.ToUpper()));
    }

    [Fact]
    public void EmptyEqualsDefault()
    {
        new DirectoryPath().ShouldBe(new DirectoryPath(string.Empty));
    }

    [Theory]
    [ClassData(typeof(TestData))]
    [InlineData("")]
    public void DoesNotEqualRawPath(string path)
    {
        new DirectoryPath(path).Equals((object)path).ShouldBeFalse();
    }

    [Theory]
    [ClassData(typeof(TestData))]
    [InlineData("")]
    public void HashEqualsSelf(string path)
    {
        var fp = new DirectoryPath(path);
        fp.GetHashCode().ShouldBe(fp.GetHashCode());
    }

    [Theory]
    [ClassData(typeof(TestData))]
    [InlineData("")]
    public void HashEqualsDifferentCase(string path)
    {
        new DirectoryPath(path).GetHashCode()
            .ShouldBe(new DirectoryPath(path.ToUpper()).GetHashCode());
    }

    [Fact]
    public void EmptyHashEqualsDefaultHash()
    {
        new DirectoryPath().GetHashCode()
            .ShouldBe(new DirectoryPath(string.Empty).GetHashCode());
    }

    [Theory]
    [ClassData(typeof(TestData))]
    public void ToStringReturnsFullPath(string path)
    {
        new DirectoryPath(path).ToString().ShouldBe(Path.GetFullPath(path));
    }

    [Fact]
    public void EmptyToStringReturnsEmpty()
    {
        new DirectoryPath(string.Empty).ToString().ShouldBe(string.Empty);
    }

    [Fact]
    public void DefaultToStringReturnsEmpty()
    {
        new DirectoryPath().ToString().ShouldBe(string.Empty);
    }

    [Theory]
    [ClassData(typeof(TestData))]
    [InlineData("")]
    public void ImplictOperatorEqualToCtor(string path)
    {
        DirectoryPath dir = path;
        dir.ShouldBe(new DirectoryPath(path));
    }

    [Theory]
    [ClassData(typeof(TestData))]
    [InlineData("")]
    public void ImplicitOperatorFromEqualsRelative(string path)
    {
        var dir = new DirectoryPath(path);
        string str = dir;
        str.ShouldBe(dir.RelativePath);
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
                .ShouldBe(expected);
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

        fs.File.Exists(absPrefix + "SomeDir/SubDir/SomeFile.txt").ShouldBeFalse();
        fs.File.Exists(absPrefix + "SomeDir/SubDir/SomeFile2.txt").ShouldBeFalse();
        fs.File.Exists(absPrefix + "SomeDir/SubDir/SubSubDir/SomeFile2.txt").ShouldBeFalse();
        fs.File.Exists(absPrefix + "SomeDir/SomeFile.txt").ShouldBeFalse();
        fs.File.Exists(absPrefix + "SomeDir/SomeFile2.txt").ShouldBeFalse();
        fs.File.Exists(absPrefix + "SomeFile.txt").ShouldBeTrue();
        fs.File.Exists(absPrefix + "SomeOtherDir/SomeFile2.txt").ShouldBeTrue();

        fs.Directory.Exists(absPrefix + "SomeOtherDir").ShouldBeTrue();
        fs.Directory.Exists(absPrefix + "SomeDir").ShouldBeFalse();
    }

    [Fact]
    public void DeleteEntireFolderExceptSelf()
    {
        var fs = GetMockFileSystem();

        new DirectoryPath(absPrefix + "SomeDir")
            .DeleteEntireFolder(fileSystem: fs, deleteFolderItself: false);

        fs.File.Exists(absPrefix + "SomeDir/SubDir/SomeFile.txt").ShouldBeFalse();
        fs.File.Exists(absPrefix + "SomeDir/SubDir/SomeFile2.txt").ShouldBeFalse();
        fs.File.Exists(absPrefix + "SomeDir/SubDir/SubSubDir/SomeFile2.txt").ShouldBeFalse();
        fs.File.Exists(absPrefix + "SomeDir/SomeFile.txt").ShouldBeFalse();
        fs.File.Exists(absPrefix + "SomeDir/SomeFile2.txt").ShouldBeFalse();
        fs.File.Exists(absPrefix + "SomeFile.txt").ShouldBeTrue();
        fs.File.Exists(absPrefix + "SomeOtherDir/SomeFile2.txt").ShouldBeTrue();

        fs.Directory.Exists(absPrefix + "SomeOtherDir").ShouldBeTrue();
        fs.Directory.Exists(absPrefix + "SomeDir").ShouldBeTrue();
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

        fs.File.Exists(absPrefix + "SomeDir/SubDir/SomeFile.txt").ShouldBeFalse();
        fs.File.Exists(absPrefix + "SomeDir/SubDir/SomeFile2.txt").ShouldBeFalse();
        fs.File.Exists(absPrefix + "SomeDir/SubDir/SubSubDir/SomeFile2.txt").ShouldBeFalse();
        fs.File.Exists(absPrefix + "SomeDir/SomeFile.txt").ShouldBeFalse();
        fs.File.Exists(absPrefix + "SomeDir/SomeFile2.txt").ShouldBeFalse();
        fs.File.Exists(absPrefix + "SomeFile.txt").ShouldBeTrue();
        fs.File.Exists(absPrefix + "SomeOtherDir/SomeFile2.txt").ShouldBeTrue();

        fs.Directory.Exists(absPrefix + "SomeOtherDir").ShouldBeTrue();
        fs.Directory.Exists(absPrefix + "SomeDir").ShouldBeFalse();
    }

    [Fact]
    public void TryDeleteEntireFolderExceptSelf()
    {
        var fs = GetMockFileSystem();

        new DirectoryPath(absPrefix + "SomeDir")
            .TryDeleteEntireFolder(fileSystem: fs, deleteFolderItself: false);

        fs.File.Exists(absPrefix + "SomeDir/SubDir/SomeFile.txt").ShouldBeFalse();
        fs.File.Exists(absPrefix + "SomeDir/SubDir/SomeFile2.txt").ShouldBeFalse();
        fs.File.Exists(absPrefix + "SomeDir/SubDir/SubSubDir/SomeFile2.txt").ShouldBeFalse();
        fs.File.Exists(absPrefix + "SomeDir/SomeFile.txt").ShouldBeFalse();
        fs.File.Exists(absPrefix + "SomeDir/SomeFile2.txt").ShouldBeFalse();
        fs.File.Exists(absPrefix + "SomeFile.txt").ShouldBeTrue();
        fs.File.Exists(absPrefix + "SomeOtherDir/SomeFile2.txt").ShouldBeTrue();

        fs.Directory.Exists(absPrefix + "SomeOtherDir").ShouldBeTrue();
        fs.Directory.Exists(absPrefix + "SomeDir").ShouldBeTrue();
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

        fs.File.Exists(absPrefix + "SomeDir/SubDir/SomeFile.txt").ShouldBeTrue();
        fs.File.Exists(absPrefix + "SomeDir/SubDir/SomeFile2.txt").ShouldBeFalse();
        fs.File.Exists(absPrefix + "SomeDir/SubDir/SubSubDir/SomeFile2.txt").ShouldBeTrue();
        fs.File.Exists(absPrefix + "SomeDir/SomeFile.txt").ShouldBeTrue();
        fs.File.Exists(absPrefix + "SomeDir/SomeFile2.txt").ShouldBeTrue();
        fs.File.Exists(absPrefix + "SomeFile.txt").ShouldBeTrue();
        fs.File.Exists(absPrefix + "SomeOtherDir/SomeFile2.txt").ShouldBeTrue();

        fs.Directory.Exists(absPrefix + "SomeOtherDir").ShouldBeTrue();
        fs.Directory.Exists(absPrefix + "SomeDir").ShouldBeTrue();
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

        fs.File.Exists(absPrefix + "SomeDir/SubDir/SomeFile.txt").ShouldBeTrue();
        fs.File.Exists(absPrefix + "SomeDir/SubDir/SomeFile2.txt").ShouldBeFalse();
        fs.File.Exists(absPrefix + "SomeDir/SubDir/SubSubDir/SomeFile2.txt").ShouldBeTrue();
        fs.File.Exists(absPrefix + "SomeDir/SomeFile.txt").ShouldBeTrue();
        fs.File.Exists(absPrefix + "SomeDir/SomeFile2.txt").ShouldBeTrue();
        fs.File.Exists(absPrefix + "SomeFile.txt").ShouldBeTrue();
        fs.File.Exists(absPrefix + "SomeOtherDir/SomeFile2.txt").ShouldBeTrue();

        fs.Directory.Exists(absPrefix + "SomeOtherDir").ShouldBeTrue();
        fs.Directory.Exists(absPrefix + "SomeDir").ShouldBeTrue();
    }

    [Fact]
    public void CreateMakesFolder()
    {
        var fs = new MockFileSystem();
        var someDir = absPrefix + "SomeDir";
        new DirectoryPath(someDir)
            .Create(fs);
        fs.Directory.Exists(someDir).ShouldBeTrue();
    }

    [Fact]
    public void DeleteDestroysFolder()
    {
        var fs = new MockFileSystem();
        var someDir = absPrefix + "SomeDir";
        fs.Directory.CreateDirectory(someDir);
        new DirectoryPath(someDir)
            .Delete(fs);
        fs.Directory.Exists(someDir).ShouldBeFalse();
    }

    [Fact]
    public void CheckEmptyIsEmpty()
    {
        var fs = new MockFileSystem();
        var someDir = absPrefix + "SomeDir";
        fs.Directory.CreateDirectory(absPrefix + "SomeDir");
        new DirectoryPath(someDir)
            .CheckEmpty(fs)
            .ShouldBeTrue();
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
            .ShouldBeFalse();
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
            .ShouldBeFalse();
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
                .ShouldBe(expected);
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
                .ShouldBe(expected);
        }
    }

    [Theory, DefaultAutoData]
    public void EqualOperatorOverload(
        DirectoryPath dir)
    {
        DirectoryPath other = new DirectoryPath(dir.Path);
        Assert.True(dir == other);
    }

    [Theory, DefaultAutoData]
    public void NotEqualOperatorOverload(
        DirectoryPath dir,
        DirectoryPath dir2)
    {
        Assert.True(dir != dir2);
    }

    [Theory, DefaultAutoData]
    public void NotEqualOperatorToDefault(
        DirectoryPath dir)
    {
        Assert.True(dir != default);
    }
}