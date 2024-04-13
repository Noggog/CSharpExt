using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using AutoFixture;
using AutoFixture.Kernel;
using Noggog.Testing.FileSystem;
using NSubstitute;

namespace Noggog.Testing.AutoFixture;

public class FileSystemBuilder : ISpecimenBuilder
{
    private readonly TargetFileSystem _targetFileSystem;
    private MockFileSystem? _mockFileSystem;
    private MockFileSystemWatcher? _fileSystemWatcher;

    public FileSystemBuilder(TargetFileSystem targetFileSystem = TargetFileSystem.Fake)
    {
        _targetFileSystem = targetFileSystem;
    }

    public object Create(object request, ISpecimenContext context)
    {
        if (request is SeededRequest seed)
        {
            request = seed.Request;
        }

        if (request is not Type t) return new NoSpecimen();
            
        if (t == typeof(IFileSystem))
        {
            switch (_targetFileSystem)
            {
                case TargetFileSystem.Fake:
                    return context.Create<MockFileSystem>();
                case TargetFileSystem.Substitute:
                    return Substitute.For<IFileSystem>();
                case TargetFileSystem.Real:
                    return new System.IO.Abstractions.FileSystem();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        else if (t == typeof(MockFileSystem))
        {
            if (_mockFileSystem == null)
            {
                _mockFileSystem = GetFs(context);
            }
            return _mockFileSystem;
        }
        else if (t == typeof(IFileSystemWatcherFactory))
        {
            return context.Create<FileSystem.MockFileSystemWatcherFactory>();
        }
        else if (t == typeof(FileSystem.MockFileSystemWatcherFactory))
        {
            return new FileSystem.MockFileSystemWatcherFactory(
                context.Create<IFileSystem>(),
                context.Create<MockFileSystemWatcher>());
        }
        else if (t == typeof(MockFileSystemWatcher))
        {
            if (_fileSystemWatcher == null)
            {
                GetFs(context);
            }

            return _fileSystemWatcher!;
        }
        else if (t == typeof(IFileSystemEnvironmentInstructions))
        {
            return new DefaultFileSystemEnvironmentInstructions();
        }
        return new NoSpecimen();
    }

    public NoggogMockFileSystem GetFs(ISpecimenContext context)
    {
        var mockFs = new NoggogMockFileSystem(
            new Dictionary<string, MockFileData>());
        _fileSystemWatcher ??= new MockFileSystemWatcher(mockFs);
        mockFs.SetFileSystemWatcherFactory(
            new FileSystem.MockFileSystemWatcherFactory(
                mockFs,
                _fileSystemWatcher));
        var instrs = context.Create<IFileSystemEnvironmentInstructions>();
        foreach (var dirs in instrs.DirectoryPaths(context))
        {
            mockFs.Directory.CreateDirectory(dirs);
        }
        foreach (var file in instrs.FilePaths(context))
        {
            if (file.Directory is { } parent)
            {
                mockFs.Directory.CreateDirectory(parent);
            }
            mockFs.File.Create(file);
        }
        return mockFs;
    }
}