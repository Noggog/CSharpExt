using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Reflection;
using AutoFixture;
using AutoFixture.Kernel;
using Noggog.Testing.FileSystem;

namespace Noggog.Testing.AutoFixture
{
    public class FileSystemCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customizations.Add(new FileSystemSpecimenBuilder());
        }
    }

    public class FileSystemSpecimenBuilder : ISpecimenBuilder
    {
        public object Create(object request, ISpecimenContext context)
        {
            if (request is not Type t) return new NoSpecimen();
            if (t == typeof(MockFileSystem)
                || t == typeof(IFileSystem))
            {
                var ret = new MockFileSystem(new Dictionary<string, MockFileData>())
                {
                    FileSystemWatcher = context.Create<IFileSystemWatcherFactory>()
                };
                ret.Directory.CreateDirectory("C:\\ExistingDirectory");
                return ret;
            }
            else if (t == typeof(Noggog.Testing.FileSystem.MockFileSystemWatcherFactory)
                || t == typeof(IFileSystemWatcherFactory))
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