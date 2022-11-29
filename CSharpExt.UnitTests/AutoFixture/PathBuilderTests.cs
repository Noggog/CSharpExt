using System.IO.Abstractions;
using FluentAssertions;
using Noggog;
using Noggog.Testing.AutoFixture;
using Noggog.Testing.IO;
using Xunit;

namespace CSharpExt.UnitTests.AutoFixture;

public class PathBuilderTests
{
    [Theory]
    [DefaultAutoData]
    public void File(
        IFileSystem fileSystem,
        FilePath file)
    {
        file.IsUnderneath(PathingUtil.DrivePrefix).Should().BeTrue();
        fileSystem.File.Exists(file).Should().BeFalse();
    }
    
    [Theory]
    [DefaultAutoData]
    public void Dir(
        IFileSystem fileSystem,
        DirectoryPath dir)
    {
        dir.IsUnderneath(PathingUtil.DrivePrefix).Should().BeTrue();
        fileSystem.Directory.Exists(dir).Should().BeFalse();
    }
    
    [Theory]
    [DefaultAutoData]
    public void DifferentFiles(
        FilePath file1,
        FilePath file2)
    {
        file1.Path.Should().NotBe(file2.Path);
    }
    
    [Theory]
    [DefaultAutoData]
    public void DifferentDirectories(
        DirectoryPath dir1,
        DirectoryPath dir2)
    {
        dir1.Path.Should().NotBe(dir2.Path);
    }
    
    [Theory]
    [DefaultAutoData]
    public void ExistingFile(
        IFileSystem fileSystem,
        FilePath existingFile)
    {
        existingFile.IsUnderneath(PathingUtil.DrivePrefix).Should().BeTrue();
        fileSystem.File.Exists(existingFile).Should().BeTrue();
    }
    
    [Theory]
    [DefaultAutoData]
    public void ExistingDir(
        IFileSystem fileSystem,
        DirectoryPath existingDir)
    {
        existingDir.IsUnderneath(PathingUtil.DrivePrefix).Should().BeTrue();
        fileSystem.Directory.Exists(existingDir).Should().BeTrue();
    }
    
    [Theory]
    [DefaultAutoData]
    public void ExistingFilesDifferent(
        IFileSystem fileSystem,
        FilePath existingFile1,
        FilePath existingFile2)
    {
        existingFile1.Path.Should().NotBe(existingFile2.Path);
    }
    
    [Theory]
    [DefaultAutoData]
    public void ExistingDirsDifferent(
        IFileSystem fileSystem,
        DirectoryPath existingDir1,
        DirectoryPath existingDir2)
    {
        existingDir1.Path.Should().NotBe(existingDir2.Path);
    }
}