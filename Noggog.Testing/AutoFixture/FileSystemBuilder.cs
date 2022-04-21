using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using AutoFixture;
using AutoFixture.Kernel;
using Noggog.Testing.FileSystem;
using NSubstitute;

namespace Noggog.Testing.AutoFixture;

public class FileSystemBuilder : ISpecimenBuilder
{
    private readonly bool _useMockFileSystem;
    private MockFileSystem? _mockFileSystem;
    private MockFileSystemWatcher? _fileSystemWatcher;

    public FileSystemBuilder(bool useMockFileSystem = true)
    {
        _useMockFileSystem = useMockFileSystem;
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
            if (_useMockFileSystem)
            {
                return context.Create<MockFileSystem>();
            }
            else
            {
                return Substitute.For<IFileSystem>();
            }
        }
        else if (t == typeof(MockFileSystem))
        {
            if (_mockFileSystem == null)
            {
                _mockFileSystem = new NoggogMockFileSystem(
                    new Dictionary<string, MockFileData>(),
                    fileSystemWatcher: context.Create<IFileSystemWatcherFactory>());
                _mockFileSystem.Directory.CreateDirectory(PathBuilder.ExistingDirectory);
                _mockFileSystem.File.Create(PathBuilder.ExistingFile);
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
                context.Create<MockFileSystemWatcher>());
        }
        else if (t == typeof(MockFileSystemWatcher))
        {
            if (_fileSystemWatcher == null)
            {
                _fileSystemWatcher = new MockFileSystemWatcher();
            }

            return _fileSystemWatcher;
        }
        return new NoSpecimen();
    }
}