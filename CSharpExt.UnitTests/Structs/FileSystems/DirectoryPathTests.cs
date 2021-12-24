using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using FluentAssertions;
using Noggog;
using NSubstitute;
using Xunit;

namespace CSharpExt.UnitTests.Structs.FileSystems
{
    public class DirectoryPathTests
    {
        private static string RelativeMightBeFile = "Directory\\Test.txt";
        private static string Relative = "Directory\\Test";
        private static string UpwardsNavigation = "..\\Test";
        private static string Absolute = "C:\\Directory\\Test";
        private static string JustName = "Test";
        private static string JustNameMightBeFile = "Test.txt";

        class TestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator() => Data().Select(x => new object[] {x}).GetEnumerator();

            private IEnumerable<string> Data()
            {
                yield return RelativeMightBeFile;
                yield return Relative;
                yield return Absolute;
                yield return UpwardsNavigation;
                yield return JustName;
                yield return JustNameMightBeFile;
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
            var path = "C:\\SomeFolder";
            var fs = Substitute.For<IFileSystem>();
            fs.Directory.Exists(path).Returns(shouldExist);
            new DirectoryPath(path).CheckExists(fs).Should().Be(shouldExist);
        }

        [Fact]
        public void PathAdjustsForwardSlashes()
        {
            new DirectoryPath("C:/SomeDir")
                .Path.Should().Be("C:\\SomeDir");
        }

        [Fact]
        public void RelativePathAdjustsForwardSlashes()
        {
            new DirectoryPath("SomeDir/SubDir")
                .RelativePath.Should().Be("SomeDir\\SubDir");
        }

        [Fact]
        public void PathTrimsTrailingBackSlashes()
        {
            new DirectoryPath("C:\\SomeDir\\")
                .Path.Should().Be("C:\\SomeDir");
        }

        [Fact]
        public void RelativePathTrimsTrailingBackSlashes()
        {
            new DirectoryPath("SomeDir\\SubDir\\")
                .RelativePath.Should().Be("SomeDir\\SubDir");
        }

        [Fact]
        public void PathTrimsTrailingForwardSlashes()
        {
            new DirectoryPath("C:/SomeDir/")
                .Path.Should().Be("C:\\SomeDir");
        }

        [Fact]
        public void RelativePathTrimsTrailingForwardSlashes()
        {
            new DirectoryPath("SomeDir/SubDir/")
                .RelativePath.Should().Be("SomeDir\\SubDir");
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
            new DirectoryPath(from)
                .IsSubfolderOf(new DirectoryPath(to))
                .Should().Be(expected);
        }

        public MockFileSystem GetMockFileSystem()
        {
            return new MockFileSystem(new Dictionary<string, MockFileData>()
            {
                { "C:/SomeDir/SubDir/SomeFile.txt", string.Empty },
                { "C:/SomeDir/SubDir/SomeFile2.txt", string.Empty },
                { "C:/SomeDir/SubDir/SubSubDir/SomeFile2.txt", string.Empty },
                { "C:/SomeDir/SomeFile.txt", string.Empty },
                { "C:/SomeDir/SomeFile2.txt", string.Empty },
                { "C:/SomeFile.txt", string.Empty },
                { "C:/SomeOtherDir/SomeFile2.txt", string.Empty },
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

            new DirectoryPath("C:/SomeDir")
                .DeleteEntireFolder(fileSystem: fs, disableReadOnly: locked);

            fs.File.Exists("C:/SomeDir/SubDir/SomeFile.txt").Should().BeFalse();
            fs.File.Exists("C:/SomeDir/SubDir/SomeFile2.txt").Should().BeFalse();
            fs.File.Exists("C:/SomeDir/SubDir/SubSubDir/SomeFile2.txt").Should().BeFalse();
            fs.File.Exists("C:/SomeDir/SomeFile.txt").Should().BeFalse();
            fs.File.Exists("C:/SomeDir/SomeFile2.txt").Should().BeFalse();
            fs.File.Exists("C:/SomeFile.txt").Should().BeTrue();
            fs.File.Exists("C:/SomeOtherDir/SomeFile2.txt").Should().BeTrue();

            fs.Directory.Exists("C:/SomeOtherDir").Should().BeTrue();
            fs.Directory.Exists("C:/SomeDir").Should().BeFalse();
        }

        [Fact]
        public void DeleteEntireFolderExceptSelf()
        {
            var fs = GetMockFileSystem();

            new DirectoryPath("C:/SomeDir")
                .DeleteEntireFolder(fileSystem: fs, deleteFolderItself: false);

            fs.File.Exists("C:/SomeDir/SubDir/SomeFile.txt").Should().BeFalse();
            fs.File.Exists("C:/SomeDir/SubDir/SomeFile2.txt").Should().BeFalse();
            fs.File.Exists("C:/SomeDir/SubDir/SubSubDir/SomeFile2.txt").Should().BeFalse();
            fs.File.Exists("C:/SomeDir/SomeFile.txt").Should().BeFalse();
            fs.File.Exists("C:/SomeDir/SomeFile2.txt").Should().BeFalse();
            fs.File.Exists("C:/SomeFile.txt").Should().BeTrue();
            fs.File.Exists("C:/SomeOtherDir/SomeFile2.txt").Should().BeTrue();

            fs.Directory.Exists("C:/SomeOtherDir").Should().BeTrue();
            fs.Directory.Exists("C:/SomeDir").Should().BeTrue();
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

            new DirectoryPath("C:/SomeDir")
                .TryDeleteEntireFolder(fileSystem: fs, disableReadOnly: locked);

            fs.File.Exists("C:/SomeDir/SubDir/SomeFile.txt").Should().BeFalse();
            fs.File.Exists("C:/SomeDir/SubDir/SomeFile2.txt").Should().BeFalse();
            fs.File.Exists("C:/SomeDir/SubDir/SubSubDir/SomeFile2.txt").Should().BeFalse();
            fs.File.Exists("C:/SomeDir/SomeFile.txt").Should().BeFalse();
            fs.File.Exists("C:/SomeDir/SomeFile2.txt").Should().BeFalse();
            fs.File.Exists("C:/SomeFile.txt").Should().BeTrue();
            fs.File.Exists("C:/SomeOtherDir/SomeFile2.txt").Should().BeTrue();

            fs.Directory.Exists("C:/SomeOtherDir").Should().BeTrue();
            fs.Directory.Exists("C:/SomeDir").Should().BeFalse();
        }

        [Fact]
        public void TryDeleteEntireFolderExceptSelf()
        {
            var fs = GetMockFileSystem();

            new DirectoryPath("C:/SomeDir")
                .TryDeleteEntireFolder(fileSystem: fs, deleteFolderItself: false);

            fs.File.Exists("C:/SomeDir/SubDir/SomeFile.txt").Should().BeFalse();
            fs.File.Exists("C:/SomeDir/SubDir/SomeFile2.txt").Should().BeFalse();
            fs.File.Exists("C:/SomeDir/SubDir/SubSubDir/SomeFile2.txt").Should().BeFalse();
            fs.File.Exists("C:/SomeDir/SomeFile.txt").Should().BeFalse();
            fs.File.Exists("C:/SomeDir/SomeFile2.txt").Should().BeFalse();
            fs.File.Exists("C:/SomeFile.txt").Should().BeTrue();
            fs.File.Exists("C:/SomeOtherDir/SomeFile2.txt").Should().BeTrue();

            fs.Directory.Exists("C:/SomeOtherDir").Should().BeTrue();
            fs.Directory.Exists("C:/SomeDir").Should().BeTrue();
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
                new DirectoryPath("C:/SomeDir")
                    .DeleteEntireFolder(fileSystem: fs, disableReadOnly: false);
            });
        }

        [Fact]
        public void DeleteEntireFolderLockedDeletesWhatItCan()
        {
            var fs = GetMockFileSystem();
            foreach (var file in fs.AllFiles)
            {
                if (new FilePath(file).Equals("C:/SomeDir/SubDir/SomeFile2.txt")) continue;
                fs.File.SetAttributes(file, fs.File.GetAttributes(file).SetFlag(FileAttributes.ReadOnly, true));
            }

            Assert.Throws<UnauthorizedAccessException>(() =>
            {
                new DirectoryPath("C:/SomeDir")
                    .DeleteEntireFolder(fileSystem: fs, disableReadOnly: false);
            });

            fs.File.Exists("C:/SomeDir/SubDir/SomeFile.txt").Should().BeTrue();
            fs.File.Exists("C:/SomeDir/SubDir/SomeFile2.txt").Should().BeFalse();
            fs.File.Exists("C:/SomeDir/SubDir/SubSubDir/SomeFile2.txt").Should().BeTrue();
            fs.File.Exists("C:/SomeDir/SomeFile.txt").Should().BeTrue();
            fs.File.Exists("C:/SomeDir/SomeFile2.txt").Should().BeTrue();
            fs.File.Exists("C:/SomeFile.txt").Should().BeTrue();
            fs.File.Exists("C:/SomeOtherDir/SomeFile2.txt").Should().BeTrue();

            fs.Directory.Exists("C:/SomeOtherDir").Should().BeTrue();
            fs.Directory.Exists("C:/SomeDir").Should().BeTrue();
        }

        [Fact]
        public void TryDeleteEntireFolderLockedDoesNotThrow()
        {
            var fs = GetMockFileSystem();
            foreach (var file in fs.AllFiles)
            {
                fs.File.SetAttributes(file, fs.File.GetAttributes(file).SetFlag(FileAttributes.ReadOnly, true));
            }

            new DirectoryPath("C:/SomeDir")
                .TryDeleteEntireFolder(fileSystem: fs, disableReadOnly: false);
        }

        [Fact]
        public void TryDeleteEntireFolderDeletesWhatItCan()
        {
            var fs = GetMockFileSystem();
            foreach (var file in fs.AllFiles)
            {
                if (new FilePath(file).Equals("C:/SomeDir/SubDir/SomeFile2.txt")) continue;
                fs.File.SetAttributes(file, fs.File.GetAttributes(file).SetFlag(FileAttributes.ReadOnly, true));
            }

            new DirectoryPath("C:/SomeDir")
                .TryDeleteEntireFolder(fileSystem: fs, disableReadOnly: false);

            fs.File.Exists("C:/SomeDir/SubDir/SomeFile.txt").Should().BeTrue();
            fs.File.Exists("C:/SomeDir/SubDir/SomeFile2.txt").Should().BeFalse();
            fs.File.Exists("C:/SomeDir/SubDir/SubSubDir/SomeFile2.txt").Should().BeTrue();
            fs.File.Exists("C:/SomeDir/SomeFile.txt").Should().BeTrue();
            fs.File.Exists("C:/SomeDir/SomeFile2.txt").Should().BeTrue();
            fs.File.Exists("C:/SomeFile.txt").Should().BeTrue();
            fs.File.Exists("C:/SomeOtherDir/SomeFile2.txt").Should().BeTrue();

            fs.Directory.Exists("C:/SomeOtherDir").Should().BeTrue();
            fs.Directory.Exists("C:/SomeDir").Should().BeTrue();
        }

        [Fact]
        public void CreateMakesFolder()
        {
            var fs = new MockFileSystem();
            var someDir = "C:/SomeDir";
            new DirectoryPath(someDir)
                .Create(fs);
            fs.Directory.Exists(someDir).Should().BeTrue();
        }

        [Fact]
        public void DeleteDestroysFolder()
        {
            var fs = new MockFileSystem();
            var someDir = "C:/SomeDir";
            fs.Directory.CreateDirectory(someDir);
            new DirectoryPath(someDir)
                .Delete(fs);
            fs.Directory.Exists(someDir).Should().BeFalse();
        }

        [Fact]
        public void CheckEmptyIsEmpty()
        {
            var fs = new MockFileSystem();
            var someDir = "C:/SomeDir";
            fs.Directory.CreateDirectory("C:/SomeDir");
            new DirectoryPath(someDir)
                .CheckEmpty(fs)
                .Should().BeTrue();
        }

        [Fact]
        public void CheckEmptyWithFileReturnsFalse()
        {
            var fs = new MockFileSystem();
            var someDir = "C:/SomeDir";
            fs.Directory.CreateDirectory("C:/SomeDir");
            fs.File.Create(Path.Combine(someDir, "SomeFile"));
            new DirectoryPath(someDir)
                .CheckEmpty(fs)
                .Should().BeFalse();
        }

        [Fact]
        public void CheckEmptyOnFileThrows()
        {
            var fs = new MockFileSystem();
            var someDir = "C:/SomeDir";
            fs.Directory.CreateDirectory("C:/SomeDir");
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
            var someDir = "C:/SomeDir";
            fs.Directory.CreateDirectory("C:/SomeDir");
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
            new DirectoryPath(from)
                .GetRelativePathTo(new DirectoryPath(to))
                .Should().Be(expected);
        }

        [Theory]
        [InlineData("C:\\Directory\\Text.txt", "C:\\OtherDirectory\\SomeFile.txt", "..\\Directory\\Text.txt")]
        [InlineData("C:\\Directory\\Text.txt", "C:\\Directory\\SomeFile.txt", "Text.txt")]
        [InlineData("C:\\Directory\\Text.txt", "D:\\Directory\\SomeFile.txt", "C:\\Directory\\Text.txt")]
        public void GetRelativePathToFile(string from, string to, string expected)
        {
            new DirectoryPath(from)
                .GetRelativePathTo(new FilePath(to))
                .Should().Be(expected);
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
}