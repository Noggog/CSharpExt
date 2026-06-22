using System.IO.Abstractions.TestingHelpers;
using AutoFixture.Kernel;
using Noggog;
using Noggog.Testing.AutoFixture;
using Noggog.Testing.AutoFixture.Testing;
using Shouldly;

namespace CSharpExt.UnitTests.AutoFixture;

public class MakeFileExistTests
{
    [Theory, DefaultAutoData]
    public void FileMadeInFileSystem(
        FilePath path,
        MockFileSystem mockFileSystem,
        ISpecimenContext context,
        MakeFileExist sut)
    {
        context.MockToReturn(mockFileSystem);
        sut.MakeExist(path, context);
        mockFileSystem.File.Exists(path).ShouldBeTrue();
    }

    [Theory, DefaultAutoData]
    public void FileHandleReleased(
        FilePath path,
        MockFileSystem mockFileSystem,
        ISpecimenContext context,
        MakeFileExist sut)
    {
        context.MockToReturn(mockFileSystem);
        sut.MakeExist(path, context);

        Should.NotThrow(() =>
        {
            using var stream = mockFileSystem.FileStream.New(
                path.Path, FileMode.Open, FileAccess.Read, FileShare.Read);
        });
    }
}
