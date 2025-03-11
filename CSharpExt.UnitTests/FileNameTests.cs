using Noggog;
using Shouldly;

namespace CSharpExt.UnitTests;

public class FileNameTests
{
    [Fact]
    public static void FileNameTypical()
    {
        new FileName("Test.txt");
    }

    [Fact]
    public static void FileNameNoExtension()
    {
        new FileName("Test");
    }

    [Fact]
    public static void FileNameHasDirectory()
    {
        Assert.Throws<ArgumentException>(() =>
        {
            new FileName("Directory/Test.txt");
        });
    }

    [Fact]
    public static void FileNameEmpty()
    {
        new FileName(string.Empty);
    }

    [Fact]
    public void NullableImplicitOperator()
    {
        FilePath? fp = new FilePath("Test");
        string? str = fp;
        str.ShouldBe("Test");
        fp = null;
        str = fp;
        str.ShouldBeNull();
    }
}