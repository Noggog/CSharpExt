using System.IO.Abstractions.TestingHelpers;
using AutoFixture;
using AutoFixture.Kernel;

namespace Noggog.Testing.AutoFixture;

public interface IMakeFileExist
{
    void MakeExist(FilePath path, ISpecimenContext context);
}

public class MakeFileExist : IMakeFileExist
{
    public void MakeExist(FilePath path, ISpecimenContext context)
    {
        var fs = context.Create<MockFileSystem>();
        fs.Directory.CreateDirectory(path.Directory!);
        fs.File.Create(path);
    }
}