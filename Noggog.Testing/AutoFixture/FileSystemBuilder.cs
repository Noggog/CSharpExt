using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using AutoFixture;
using AutoFixture.Kernel;
using Noggog.Testing.FileSystem;

namespace Noggog.Testing.AutoFixture
{
    public class FileSystemBuilder : ISpecimenBuilder
    {
        public object Create(object request, ISpecimenContext context)
        {
            if (request is not Type t) return new NoSpecimen();
            
            if (t == typeof(IFileSystem))
            {
                return context.Create<MockFileSystem>();
            }
            else if (t == typeof(MockFileSystem))
            {
                var ret = new MockFileSystem(new Dictionary<string, MockFileData>())
                {
                    FileSystemWatcher = context.Create<IFileSystemWatcherFactory>()
                };
                ret.Directory.CreateDirectory("C:\\ExistingDirectory");
                return ret;
            }
            else if (t == typeof(IFileSystemWatcherFactory))
            {
                return context.Create<Noggog.Testing.FileSystem.MockFileSystemWatcherFactory>();
            }
            else if (t == typeof(Noggog.Testing.FileSystem.MockFileSystemWatcherFactory))
            {
                return new Noggog.Testing.FileSystem.MockFileSystemWatcherFactory(
                    context.Create<MockFileSystemWatcher>());
            }
            else if (t == typeof(MockFileSystemWatcher))
            {
                return new MockFileSystemWatcher();
            }
            else if (t == typeof(FilePath))
            {
                return new FilePath("C:\\ExistingDirectory\\File");
            }
            else if (t == typeof(DirectoryPath))
            {
                return new DirectoryPath("C:\\ExistingDirectory");
            }
            return new NoSpecimen();
        }
    }
}