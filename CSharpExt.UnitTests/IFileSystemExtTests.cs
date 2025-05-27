using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using Noggog;
using Noggog.Testing.Extensions;
using Noggog.Testing.IO;
using NSubstitute;
using Shouldly;

namespace CSharpExt.UnitTests;

public class IFileSystemExtTests
{
    public readonly static DirectoryPath DirPath = $"{PathingUtil.DrivePrefix}SomeDir";
    public readonly static FilePath SomeFile = Path.Combine(DirPath, "SomeFile");
    public readonly static FilePath SomeFileTxt = Path.Combine(DirPath, "SomeFile.txt");
    public readonly static DirectoryPath SomeSubDir = Path.Combine(DirPath, "SubDir");
    public readonly static FilePath SomeSubFile = Path.Combine(DirPath, "SubDir", "SubFile");
    public readonly static FilePath SomeSubFileTxt = Path.Combine(DirPath, "SubDir", "SubFile.txt");

    private static MockFileSystem TypicalFileSystem()
    {
        return new MockFileSystem(new Dictionary<string, MockFileData>
        {
            { SomeFile, new MockFileData("Boop") },
            { SomeFileTxt, new MockFileData("Noop") },
            { SomeSubFile, new MockFileData("Doop") },
            { SomeSubFileTxt, new MockFileData("Zoop") },
        });
    }
        
    #region DeleteEntireFolder

    [Fact]
    public void DeleteEntireFolder_NotExists()
    {
        var dir = Substitute.For<IDirectory>();
        dir.Exists(Arg.Any<string>()).Returns(false);
        dir.DeleteEntireFolder(default, default, default);
        dir.DidNotReceiveWithAnyArgs().GetFiles(default!);
    }

    [Fact]
    public void DeleteEntireFolder_Typical()
    {
        var fileSystem = TypicalFileSystem();
        fileSystem.Directory.DeleteEntireFolder(DirPath, disableReadOnly: true, deleteFolderItself: true);
        fileSystem.File.Exists(SomeFile).ShouldBeFalse();
        fileSystem.File.Exists(SomeSubFile).ShouldBeFalse();
        fileSystem.Directory.Exists(DirPath).ShouldBeFalse();
        fileSystem.Directory.Exists(SomeSubDir).ShouldBeFalse();
    }

    [Fact]
    public void DeleteEntireFolder_DontDeleteSelf()
    {
        var fileSystem = TypicalFileSystem();
        fileSystem.Directory.DeleteEntireFolder(DirPath, disableReadOnly: true, deleteFolderItself: false);
        fileSystem.File.Exists(SomeFile).ShouldBeFalse();
        fileSystem.File.Exists(SomeSubFile).ShouldBeFalse();
        fileSystem.Directory.Exists(DirPath).ShouldBeTrue();
        fileSystem.Directory.Exists(SomeSubDir).ShouldBeFalse();
    }

    [Fact]
    public void DeleteEntireFolder_ReadOnly()
    {
        var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            { SomeFile, new MockFileData("Boop") },
        });
        var file = fileSystem.FileInfo.New(SomeFile);
        file.IsReadOnly = true;
        fileSystem.Directory.DeleteEntireFolder(DirPath, disableReadOnly: true, deleteFolderItself: true);
        fileSystem.File.Exists(SomeFile).ShouldBeFalse();
        fileSystem.Directory.Exists(DirPath).ShouldBeFalse();
    }

    [Fact]
    public void DeleteEntireFolder_ReadOnlyBlocks()
    {
        var fileSystem = TypicalFileSystem();
        var file = fileSystem.FileInfo.New(SomeFile);
        file.IsReadOnly = true;
        Assert.Throws<UnauthorizedAccessException>(() =>
        {
            fileSystem.Directory.DeleteEntireFolder(DirPath, disableReadOnly: false, deleteFolderItself: true);
        });
        fileSystem.File.Exists(SomeFile).ShouldBeTrue();
        fileSystem.File.Exists(SomeSubFile).ShouldBeFalse();
        fileSystem.Directory.Exists(DirPath).ShouldBeTrue();
        fileSystem.Directory.Exists(SomeSubDir).ShouldBeFalse();
    }
        
    [Fact]
    public void TryDeleteEntireFolder_ReadOnlyBlocks()
    {
        var fileSystem = TypicalFileSystem();
        var file = fileSystem.FileInfo.New(SomeFile);
        file.IsReadOnly = true;
        fileSystem.Directory.TryDeleteEntireFolder(DirPath, disableReadOnly: false, deleteFolderItself: true);
        fileSystem.File.Exists(SomeFile).ShouldBeTrue();
        fileSystem.File.Exists(SomeSubFile).ShouldBeFalse();
        fileSystem.Directory.Exists(DirPath).ShouldBeTrue();
        fileSystem.Directory.Exists(SomeSubDir).ShouldBeFalse();
    }

    #endregion

    #region EnumerateFilesRecursive

    [Fact]
    public void EnumerateFiles()
    {
        var fileSystem = TypicalFileSystem();
        fileSystem.Directory.EnumerateFilePaths(DirPath)
            .ShouldEqualEnumerable(
                SomeFile,
                SomeFileTxt);
    }

    [Fact]
    public void EnumerateFiles_SearchPattern()
    {
        var fileSystem = TypicalFileSystem();
        fileSystem.Directory.EnumerateFilePaths(DirPath, "*.txt")
            .ShouldEqualEnumerable(
                SomeFileTxt);
    }
        
    [Fact]
    public void EnumerateFilesRecursive()
    {
        var fileSystem = TypicalFileSystem();
        fileSystem.Directory.EnumerateFilePaths(DirPath, recursive: true)
            .ShouldEqualEnumerable(
                SomeFile,
                SomeFileTxt,
                SomeSubFile,
                SomeSubFileTxt);
    }

    [Fact]
    public void EnumerateFilesRecursive_SearchPattern()
    {
        var fileSystem = TypicalFileSystem();
        fileSystem.Directory.EnumerateFilePaths(DirPath, "*.txt", recursive: true)
            .ShouldEqualEnumerable(
                SomeFileTxt,
                SomeSubFileTxt);
    }

    #endregion

    #region EnumerateDirectories

    [Fact]
    public void EnumerateDirectories()
    {
        var subSubDir = Path.Combine(SomeSubDir, "SubSubDir");
        var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            { SomeFile, new MockFileData("Boop") },
            { SomeSubFile, new MockFileData("Doop") },
            { subSubDir, new MockFileData("Doop") },
        });
        fileSystem.Directory.EnumerateDirectoryPaths(DirPath, includeSelf: true, recursive: true)
            .ShouldEqualEnumerable(
                DirPath,
                SomeSubDir);
    }

    [Fact]
    public void EnumerateDirectories_NoSelf()
    {
        var fileSystem = TypicalFileSystem();
        fileSystem.Directory.EnumerateDirectoryPaths(DirPath, includeSelf: false, recursive: true)
            .ShouldEqualEnumerable(
                SomeSubDir);
    }

    [Fact]
    public void EnumerateDirectories_NoRecursive()
    {
        var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            { SomeFile, new MockFileData("Boop") },
            { SomeSubFile, new MockFileData("Doop") },
            { Path.Combine(SomeSubDir, "SubSubDir"), new MockFileData("Doop") },
        });
        fileSystem.Directory.EnumerateDirectoryPaths(DirPath, includeSelf: true, recursive: false)
            .ShouldEqualEnumerable(
                DirPath,
                SomeSubDir);
    }

    #endregion

    #region IsSubfolderOf

    [Fact]
    public void IsSubfolderOf()
    {
        var fileSystem = TypicalFileSystem();
        fileSystem.Directory.IsSubfolderOf(SomeSubDir, DirPath)
            .ShouldBeTrue();
    }

    [Fact]
    public void IsNotSubfolderOf()
    {
        var fileSystem = TypicalFileSystem();
        fileSystem.Directory.IsSubfolderOf(DirPath, SomeSubDir)
            .ShouldBeFalse();
    }

    #endregion

    #region DeepCopy

    [Fact]
    public void DeepCopy()
    {
        var fileSystem = TypicalFileSystem();
        var targetDir = Path.Combine(DirPath.Directory!.Value.Path, "SomeTargetDir");
        FilePath targetSomeFile = Path.Combine(DirPath, "SomeFile.txt");
        DirectoryPath targetSomeSubDir = Path.Combine(DirPath, "SubDir");
        FilePath targetSomeSubFile = Path.Combine(DirPath, "SubDir", "SubFile");

        fileSystem.Directory.DeepCopy(DirPath, targetDir);
        fileSystem.File.Exists(targetSomeFile).ShouldBeTrue();
        fileSystem.File.Exists(targetSomeSubFile).ShouldBeTrue();
        fileSystem.Directory.Exists(targetDir).ShouldBeTrue();
        fileSystem.Directory.Exists(targetSomeSubDir).ShouldBeTrue();
    }

    #endregion
}