using Noggog.StructuredStrings;

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
    public void AppendWithTrailingNewLine()
    {
        var sb = new StructuredStringBuilder();
        sb.AppendLine($"A{Environment.NewLine}");
        Assert.Equal(new string[] { "A", "" }, sb);
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

    [Fact]
    public void AppendContinuesTheLine()
    {
        var sb = new StructuredStringBuilder();
        sb.Append("A");
        sb.Append("B", endWithNewLine: true);
        sb.Append("C");
        Assert.Equal(new string[] { "AB" }, sb);
        Assert.Equal($"AB{Environment.NewLine}C", sb.ToString());
    }

    [Fact]
    public void AppendLineIndentsByDepth()
    {
        var sb = new StructuredStringBuilder();
        sb.AppendLine("A");
        using (sb.Brace())
        {
            sb.AppendLine("B");
            using (sb.IncreaseDepth())
            {
                sb.AppendLine("C");
            }
        }
        Assert.Equal(new string[] { "A", "[", "    B", "        C", "]" }, sb);
    }

    [Fact]
    public void AppendLineWithExtraLine()
    {
        var sb = new StructuredStringBuilder();
        sb.AppendLine("A", extraLine: true);
        sb.AppendLine("B");
        Assert.Equal(new string[] { "A", "", "B" }, sb);
    }

    [Fact]
    public void AppendItemNamesTheValue()
    {
        var sb = new StructuredStringBuilder();
        sb.AppendItem(5, "Count");
        sb.AppendItem(6);
        sb.AppendItem((string?)null, "Missing");
        Assert.Equal(new string[] { "Count => 5", "6" }, sb);
    }

    [Fact]
    public void Empty()
    {
        var sb = new StructuredStringBuilder();
        Assert.True(sb.Empty);
        sb.Append("  ");
        Assert.True(sb.Empty);
        sb.AppendLine();
        Assert.False(sb.Empty);
    }

    [Fact]
    public void EditLines()
    {
        var sb = new StructuredStringBuilder();
        sb.AppendLine("A");
        sb.AppendLine("B");
        sb.Insert(1, "X");
        sb[0] = "Y";
        Assert.Equal("X", sb[1]);
        sb.RemoveAt(2);
        Assert.Equal(2, sb.Count);
        Assert.Equal(new string[] { "Y", "X" }, sb);
        Assert.Throws<IndexOutOfRangeException>(() => sb[2]);
    }
}
