using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using FluentAssertions;
using Noggog;
using NSubstitute;
using Xunit;

namespace CSharpExt.UnitTests.Structs.FileSystems
{
    public class FilePathTests
    {
        private static string AbsoluteTextPath = "C:\\Directory\\Test.txt";
        private static string RelativeTextPath = "Directory\\Test.txt";
        private static string JustFileName = "Test.txt";
        private static string RelativeFileWithNoExtension = "Directory\\Test";

        class TestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator() => Data().Select(x => new object[] {x}).GetEnumerator();

            private IEnumerable<string> Data()
            {
                yield return AbsoluteTextPath;
                yield return RelativeTextPath;
                yield return JustFileName;
                yield return RelativeFileWithNoExtension;
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
                .Path.Should().Be(Path.GetFullPath(path));
        }
        
        [Fact]
        public void EmptyPathExposesEmptyPath()
        {
            new FilePath()
                .Path.Should().Be(string.Empty);
        }
        
        [Theory]
        [ClassData(typeof(TestData))]
        [InlineData("")]
        public void RelativePathExposesGivenPath(string path)
        {
            new FilePath(path)
                .RelativePath.Should().Be(path);
        }
        
        [Theory]
        [ClassData(typeof(TestData))]
        [InlineData("")]
        public void NameSameAsSystem(string path)
        {
            new FilePath(path)
                .Name.Should().Be(new FileName(Path.GetFileName(path)));
        }
        
        [Theory]
        [ClassData(typeof(TestData))]
        [InlineData("")]
        public void ExtensionSameAsSystem(string path)
        {
            new FilePath(path)
                .Extension.Should().Be(Path.GetExtension(path));
        }
        
        [Theory]
        [ClassData(typeof(TestData))]
        [InlineData("")]
        public void NameWithoutExtensionSameAsSystem(string path)
        {
            new FilePath(path)
                .NameWithoutExtension.Should().Be(Path.GetFileNameWithoutExtension(path));
        }
        
        [Theory]
        [ClassData(typeof(TestData))]
        public void DirectorySameAsSystem(string path)
        {
            new FilePath(path)
                .Directory.Should().Be(
                    new DirectoryPath(Path.GetDirectoryName(Path.GetFullPath(path))!));
        }
        
        [Theory]
        [InlineData("")]
        public void EmptyDirectoryNull(string path)
        {
            new FilePath(path)
                .Directory.Should().BeNull();
        }

        [Fact]
        public void DefaultFilePathSameAsEmpty()
        {
            new FilePath().Should().Be(new FilePath(string.Empty));
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void CheckExists(bool shouldExist)
        {
            var path = "C:\\SomeFile";
            var fs = Substitute.For<IFileSystem>();
            fs.File.Exists(path).Returns(shouldExist);
            new FilePath(path).CheckExists(fs).Should().Be(shouldExist);
        }

        [Fact]
        public void PathAdjustsForwardSlashes()
        {
            new FilePath("C:/SomeFile")
                .Path.Should().Be("C:\\SomeFile");
        }

        [Fact]
        public void RelativePathAdjustsForwardSlashes()
        {
            new FilePath("SomeDir/SomeFile")
                .RelativePath.Should().Be("SomeDir\\SomeFile");
        }

        [Theory]
        [ClassData(typeof(TestData))]
        [InlineData("")]
        public void EqualsSelf(string path)
        {
            new FilePath(path).Should().Be(new FilePath(path));
        }

        [Theory]
        [ClassData(typeof(TestData))]
        [InlineData("")]
        public void EqualsDifferentCase(string path)
        {
            new FilePath(path)
                .Should().Be(new FilePath(path.ToUpper()));
        }

        [Fact]
        public void EmptyEqualsDefault()
        {
            new FilePath().Should().Be(new FilePath(string.Empty));
        }

        [Theory]
        [ClassData(typeof(TestData))]
        [InlineData("")]
        public void DoesNotEqualRawPath(string path)
        {
            new FilePath(path).Should().NotBe(path);
        }

        [Theory]
        [ClassData(typeof(TestData))]
        [InlineData("")]
        public void HashEqualsSelf(string path)
        {
            var fp = new FilePath(path);
            fp.GetHashCode().Should().Be(fp.GetHashCode());
        }

        [Theory]
        [ClassData(typeof(TestData))]
        [InlineData("")]
        public void HashEqualsDifferentCase(string path)
        {
            new FilePath(path).GetHashCode()
                .Should().Be(new FilePath(path.ToUpper()).GetHashCode());
        }

        [Fact]
        public void EmptyHashEqualsDefaultHash()
        {
            new FilePath().GetHashCode()
                .Should().Be(new FilePath(string.Empty).GetHashCode());
        }

        [Theory]
        [InlineData("C:\\Directory\\Text.txt", "C:\\OtherDirectory", "..\\Directory\\Text.txt")]
        [InlineData("C:\\Directory\\Text.txt", "C:\\Directory", "Text.txt")]
        [InlineData("C:\\Directory\\Text.txt", "D:\\Directory", "C:\\Directory\\Text.txt")]
        public void GetRelativePathToDirectory(string from, string to, string expected)
        {
            new FilePath(from)
                .GetRelativePathTo(new DirectoryPath(to))
                .Should().Be(expected);
        }

        [Theory]
        [InlineData("C:\\Directory\\Text.txt", "C:\\OtherDirectory\\SomeFile.txt", "..\\Directory\\Text.txt")]
        [InlineData("C:\\Directory\\Text.txt", "C:\\Directory\\SomeFile.txt", "Text.txt")]
        [InlineData("C:\\Directory\\Text.txt", "D:\\Directory\\SomeFile.txt", "C:\\Directory\\Text.txt")]
        public void GetRelativePathToFile(string from, string to, string expected)
        {
            new FilePath(from)
                .GetRelativePathTo(new FilePath(to))
                .Should().Be(expected);
        }

        [Theory]
        [InlineData("C:\\Directory\\Text.txt", "C:\\OtherDirectory", false)]
        [InlineData("C:\\Directory\\SubDir\\Text.txt", "C:\\Directory", true)]
        [InlineData("C:\\Directory\\SubDir\\Text.txt", "C:\\Directory\\SubDir", true)]
        [InlineData("C:\\Directory\\Text.txt", "D:\\Directory", false)]
        [InlineData("C:\\Directory\\Text.txt", "C:\\Directory", true)]
        [InlineData("C:\\DIRECTORY\\Text.txt", "C:\\Directory", true)]
        public void IsUnderneath(string from, string to, bool expected)
        {
            new FilePath(from)
                .IsUnderneath(new DirectoryPath(to))
                .Should().Be(expected);
        }

        [Theory]
        [ClassData(typeof(TestData))]
        public void ToStringReturnsFullPath(string path)
        {
            new FilePath(path).ToString().Should().Be(Path.GetFullPath(path));
        }

        [Fact]
        public void EmptyToStringReturnsEmpty()
        {
            new FilePath(string.Empty).ToString().Should().Be(string.Empty);
        }

        [Fact]
        public void DefaultToStringReturnsEmpty()
        {
            new FilePath().ToString().Should().Be(string.Empty);
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
            str.Should().Be(fp.RelativePath);
        }

        [Theory, BasicAutoData]
        public void EqualOperatorOverload(
            FilePath file)
        {
            FilePath other = new FilePath(file.Path);
            Assert.True(file == other);
        }

        [Theory, BasicAutoData]
        public void NotEqualOperatorOverload(
            FilePath file,
            FilePath file2)
        {
            Assert.True(file != file2);
        }

        [Theory, BasicAutoData]
        public void NotEqualOperatorToDefault(
            FilePath file)
        {
            Assert.True(file != default);
        }
    }
}