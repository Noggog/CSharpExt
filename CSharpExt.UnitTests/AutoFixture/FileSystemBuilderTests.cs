using System.IO.Abstractions;
using AutoFixture;
using AutoFixture.Kernel;
using Noggog;
using Noggog.Testing.AutoFixture;
using Noggog.Testing.IO;
using Shouldly;

namespace CSharpExt.UnitTests.AutoFixture;

public class FileSystemBuilderTests
{
    private static readonly FilePath RegisteredFile =
        Path.Combine(PathBuilder.ExistingDirectory, "Registered.txt");

    private class RegisteredFileInstructions : DefaultFileSystemEnvironmentInstructions
    {
        public override IEnumerable<FilePath> FilePaths(ISpecimenContext context)
        {
            yield return RegisteredFile;
        }
    }

    private class InstructionsBuilder : ISpecimenBuilder
    {
        public object Create(object request, ISpecimenContext context)
        {
            if (request is SeededRequest seed)
            {
                request = seed.Request;
            }

            if (request is Type t && t == typeof(IFileSystemEnvironmentInstructions))
            {
                return new RegisteredFileInstructions();
            }

            return new NoSpecimen();
        }
    }

    private static IFileSystem GetFileSystem()
    {
        var fixture = new Fixture();
        fixture.Customize(new DefaultCustomization());
        fixture.Customizations.Insert(0, new InstructionsBuilder());
        return fixture.Create<IFileSystem>();
    }

    [Fact]
    public void RegisteredFileExists()
    {
        var fileSystem = GetFileSystem();
        fileSystem.File.Exists(RegisteredFile).ShouldBeTrue();
    }

    [Fact]
    public void RegisteredFileHandleIsReleased()
    {
        var fileSystem = GetFileSystem();

        Should.NotThrow(() =>
        {
            using var stream = fileSystem.FileStream.New(
                RegisteredFile.Path, FileMode.Open, FileAccess.Read, FileShare.Read);
        });
    }

    [Fact]
    public void RegisteredFileIsWritable()
    {
        var fileSystem = GetFileSystem();

        Should.NotThrow(() =>
        {
            fileSystem.File.WriteAllText(RegisteredFile, "content");
        });
        fileSystem.File.ReadAllText(RegisteredFile).ShouldBe("content");
    }
}
