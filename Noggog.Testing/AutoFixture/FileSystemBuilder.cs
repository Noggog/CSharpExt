using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using AutoFixture;
using AutoFixture.Kernel;
using Noggog.Testing.FileSystem;
using NSubstitute;

namespace Noggog.Testing.AutoFixture
{
    public class FileSystemBuilder : ISpecimenBuilder
    {
        private readonly bool _useMockFileSystem;

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
                var ret = new MockFileSystem(new Dictionary<string, MockFileData>())
                {
                    FileSystemWatcher = context.Create<IFileSystemWatcherFactory>()
                };
                ret.Directory.CreateDirectory(PathBuilder.ExistingDirectory);
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
            return new NoSpecimen();
        }
    }
}