using System.IO.Abstractions;
using Noggog;
using Noggog.Testing.AutoFixture;
using Noggog.Testing.IO;
using Shouldly;

namespace CSharpExt.UnitTests.AutoFixture;

public class PathBuilderTests
{
    [Theory]
    [DefaultAutoData]
    public void File(
        IFileSystem fileSystem,
        FilePath file)
    {
        file.IsUnderneath(PathingUtil.DrivePrefix).ShouldBeTrue();
        fileSystem.File.Exists(file).ShouldBeFalse();
    }
    
    [Theory]
    [DefaultAutoData]
    public void Dir(
        IFileSystem fileSystem,
        DirectoryPath dir)
    {
        dir.IsUnderneath(PathingUtil.DrivePrefix).ShouldBeTrue();
        fileSystem.Directory.Exists(dir).ShouldBeFalse();
    }
    
    [Theory]
    [DefaultAutoData]
    public void DifferentFiles(
        FilePath file1,
        FilePath file2)
    {
        file1.Path.ShouldNotBe(file2.Path);
    }
    
    [Theory]
    [DefaultAutoData]
    public void DifferentDirectories(
        DirectoryPath dir1,
        DirectoryPath dir2)
    {
        dir1.Path.ShouldNotBe(dir2.Path);
    }
    
    [Theory]
    [DefaultAutoData]
    public void ExistingFile(
        IFileSystem fileSystem,
        FilePath existingFile)
    {
        existingFile.IsUnderneath(PathingUtil.DrivePrefix).ShouldBeTrue();
        fileSystem.File.Exists(existingFile).ShouldBeTrue();
    }
    
    [Theory]
    [DefaultAutoData]
    public void ExistingDir(
        IFileSystem fileSystem,
        DirectoryPath existingDir)
    {
        existingDir.IsUnderneath(PathingUtil.DrivePrefix).ShouldBeTrue();
        fileSystem.Directory.Exists(existingDir).ShouldBeTrue();
    }
    
    [Theory]
    [DefaultAutoData]
    public void ExistingFilesDifferent(
        IFileSystem fileSystem,
        FilePath existingFile1,
        FilePath existingFile2)
    {
        existingFile1.Path.ShouldNotBe(existingFile2.Path);
    }
    
    [Theory]
    [DefaultAutoData]
    public void ExistingDirsDifferent(
        IFileSystem fileSystem,
        DirectoryPath existingDir1,
        DirectoryPath existingDir2)
    {
        existingDir1.Path.ShouldNotBe(existingDir2.Path);
    }
}