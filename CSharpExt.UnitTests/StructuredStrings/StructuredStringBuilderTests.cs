using Noggog.StructuredStrings;
using Xunit;

namespace CSharpExt.UnitTests.StructuredStrings;

public class StructuredStringBuilderTests
{
    [Fact]
    public void AppendWithEmbeddedNewLine()
    {
        var sb = new StructuredStringBuilder();
        sb.AppendLine($"A{Environment.NewLine}B");
        Assert.Equal(new string[] { "A", "B" }, sb);
    }

    [Fact]
    public void AppendNull()
    {
        var sb = new StructuredStringBuilder();
        sb.AppendLine(null);
        Assert.Equal(new string[] { "" }, sb);
    }

    [Fact]
    public void AppendEmpty()
    {
        var sb = new StructuredStringBuilder();
        sb.AppendLine(String.Empty);
        Assert.Equal(new string[] { "" }, sb);
    }
}