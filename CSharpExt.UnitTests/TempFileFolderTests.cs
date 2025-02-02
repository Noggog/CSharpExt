using Noggog.IO;
using Shouldly;

namespace CSharpExt.UnitTests;

public class TempFileFolderTests
{
    [Fact]
    public void TempFolder_Exists()
    {
        var filepath = Path.Combine(Path.GetTempPath(), $"CSharpEXT/Test");
        using (var tmp = TempFolder.FactoryByPath(filepath, deleteAfter: true))
        {
            Directory.Exists(filepath).ShouldBeTrue();
        }
        Directory.Exists(filepath).ShouldBeFalse();
    }

    [Fact]
    public void TempFile_DeleteAfter()
    {
        string path;
        using (var tmp = new TempFile(deleteAfter: true))
        {
            path = tmp.File.Path.ToString();
        }
        File.Exists(path).ShouldBeFalse();
    }
}