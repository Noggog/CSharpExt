using System.IO.Abstractions.TestingHelpers;
using AutoFixture.Kernel;
using Noggog;
using Noggog.Testing.AutoFixture;
using Noggog.Testing.AutoFixture.Testing;
using NSubstitute;
using Xunit;

namespace CSharpExt.UnitTests.AutoFixture
{
    public class MakeFileExistTests
    {
        [Theory, BasicAutoData]
        public void CreatesMockFileSystem(
            FilePath path,
            ISpecimenContext context,
            MakeFileExist sut)
        {
            context.MockToReturn<MockFileSystem>();
            sut.MakeExist(path, context);
            context.ShouldHaveCreated<MockFileSystem>();
        }
        
        [Theory, BasicAutoData]
        public void FileMadeInFileSystem(
            FilePath path,
            MockFileSystem mockFileSystem,
            ISpecimenContext context,
            MakeFileExist sut)
        {
            context.MockToReturn(mockFileSystem);
            sut.MakeExist(path, context);
            mockFileSystem.File.Exists(path);
        }
    }
}