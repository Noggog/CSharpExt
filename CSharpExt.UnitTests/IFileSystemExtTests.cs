using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using FluentAssertions;
using Noggog;
using Noggog.Testing.IO;
using NSubstitute;

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
        fileSystem.File.Exists(SomeFile).Should().BeFalse();
        fileSystem.File.Exists(SomeSubFile).Should().BeFalse();
        fileSystem.Directory.Exists(DirPath).Should().BeFalse();
        fileSystem.Directory.Exists(SomeSubDir).Should().BeFalse();
    }

    [Fact]
    public void DeleteEntireFolder_DontDeleteSelf()
    {
        var fileSystem = TypicalFileSystem();
        fileSystem.Directory.DeleteEntireFolder(DirPath, disableReadOnly: true, deleteFolderItself: false);
        fileSystem.File.Exists(SomeFile).Should().BeFalse();
        fileSystem.File.Exists(SomeSubFile).Should().BeFalse();
        fileSystem.Directory.Exists(DirPath).Should().BeTrue();
        fileSystem.Directory.Exists(SomeSubDir).Should().BeFalse();
    }

    [Fact]
    public void DeleteEntireFolder_ReadOnly()
    {
        var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            { SomeFile, new MockFileData("Boop") },
        });
        var file = fileSystem.FileInfo.FromFileName(SomeFile);
        file.IsReadOnly = true;
        fileSystem.Directory.DeleteEntireFolder(DirPath, disableReadOnly: true, deleteFolderItself: true);
        fileSystem.File.Exists(SomeFile).Should().BeFalse();
        fileSystem.Directory.Exists(DirPath).Should().BeFalse();
    }

    [Fact]
    public void DeleteEntireFolder_ReadOnlyBlocks()
    {
        var fileSystem = TypicalFileSystem();
        var file = fileSystem.FileInfo.FromFileName(SomeFile);
        file.IsReadOnly = true;
        Assert.Throws<UnauthorizedAccessException>(() =>
        {
            fileSystem.Directory.DeleteEntireFolder(DirPath, disableReadOnly: false, deleteFolderItself: true);
        });
        fileSystem.File.Exists(SomeFile).Should().BeTrue();
        fileSystem.File.Exists(SomeSubFile).Should().BeFalse();
        fileSystem.Directory.Exists(DirPath).Should().BeTrue();
        fileSystem.Directory.Exists(SomeSubDir).Should().BeFalse();
    }
        
    [Fact]
    public void TryDeleteEntireFolder_ReadOnlyBlocks()
    {
        var fileSystem = TypicalFileSystem();
        var file = fileSystem.FileInfo.FromFileName(SomeFile);
        file.IsReadOnly = true;
        fileSystem.Directory.TryDeleteEntireFolder(DirPath, disableReadOnly: false, deleteFolderItself: true);
        fileSystem.File.Exists(SomeFile).Should().BeTrue();
        fileSystem.File.Exists(SomeSubFile).Should().BeFalse();
        fileSystem.Directory.Exists(DirPath).Should().BeTrue();
        fileSystem.Directory.Exists(SomeSubDir).Should().BeFalse();
    }

    #endregion

    #region EnumerateFilesRecursive

    [Fact]
    public void EnumerateFiles()
    {
        var fileSystem = TypicalFileSystem();
        fileSystem.Directory.EnumerateFilePaths(DirPath)
            .Should().Equal(
                SomeFile,
                SomeFileTxt);
    }

    [Fact]
    public void EnumerateFiles_SearchPattern()
    {
        var fileSystem = TypicalFileSystem();
        fileSystem.Directory.EnumerateFilePaths(DirPath, "*.txt")
            .Should().Equal(
                SomeFileTxt);
    }
        
    [Fact]
    public void EnumerateFilesRecursive()
    {
        var fileSystem = TypicalFileSystem();
        fileSystem.Directory.EnumerateFilePaths(DirPath, recursive: true)
            .Should().Equal(
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
            .Should().Equal(
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
            .Should().Equal(
                DirPath,
                SomeSubDir);
    }

    [Fact]
    public void EnumerateDirectories_NoSelf()
    {
        var fileSystem = TypicalFileSystem();
        fileSystem.Directory.EnumerateDirectoryPaths(DirPath, includeSelf: false, recursive: true)
            .Should().Equal(
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
            .Should().Equal(
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
            .Should().BeTrue();
    }

    [Fact]
    public void IsNotSubfolderOf()
    {
        var fileSystem = TypicalFileSystem();
        fileSystem.Directory.IsSubfolderOf(DirPath, SomeSubDir)
            .Should().BeFalse();
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
        fileSystem.File.Exists(targetSomeFile).Should().BeTrue();
        fileSystem.File.Exists(targetSomeSubFile).Should().BeTrue();
        fileSystem.Directory.Exists(targetDir).Should().BeTrue();
        fileSystem.Directory.Exists(targetSomeSubDir).Should().BeTrue();
    }

    #endregion
}