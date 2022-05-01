using Noggog.StructuredStrings;
using Xunit;

namespace CSharpExt.UnitTests.StructuredStrings;

public class CommentTests
{
    [Fact]
    public void EmptyComments()
    {
        var sb = new StructuredStringBuilder();
        using (var commentWrapper = new Comment(sb))
        {
            
        }
        Assert.True(sb.Empty);
    }

    [Fact]
    public void OneSummaryLine()
    {
        var sb = new StructuredStringBuilder();
        using (var commentWrapper = new Comment(sb))
        {
            commentWrapper.Summary.AppendLine("An awesome summary.");
        }

        var expected = new string[]
        {
            "/// <summary>",
            "/// An awesome summary.",
            "/// </summary>",
        };

        Assert.Equal(expected, sb);
    }

    [Fact]
    public void OneParameter()
    {
        var sb = new StructuredStringBuilder();

        using (var commentWrapper = new Comment(sb))
        {
            commentWrapper.AddParameter("name", "The name of the thing");
        }

        var expected = new string[]
        {
            "/// <param name=\"name\">The name of the thing</param>",
        };

        Assert.Equal(expected, sb);
    }

    [Fact]
    public void TwoParameters()
    {
        var sb = new StructuredStringBuilder();
        using (var commentWrapper = new Comment(sb))
        {
            commentWrapper.AddParameter("name", "The name of the thing");
            commentWrapper.AddParameter("thing", "The thing of the thing");
        }

        var expected = new string[]
        {
            "/// <param name=\"name\">The name of the thing</param>",
            "/// <param name=\"thing\">The thing of the thing</param>",
        };

        Assert.Equal(expected, sb);
    }

    [Fact]
    public void MultiLineParameter()
    {
        var sb = new StructuredStringBuilder();

        using (var commentWrapper = new Comment(sb))
        {
            var description = new StructuredStringBuilder();
            description.AppendLine("The name of");
            description.AppendLine("the thing");

            commentWrapper.Parameters["name"] = description;
        }

        var expected = new string[]
        {
            "/// <param name=\"name\">",
            "/// The name of",
            "/// the thing",
            "/// </param>",
        };

        Assert.Equal(expected, sb);
    }

    [Fact]
    public void Returns()
    {
        var sb = new StructuredStringBuilder();

        using (var commentWrapper = new Comment(sb))
        {
            commentWrapper.Return.AppendLine("Awesomeness");
        }

        var expected = new string[]
        {
            "/// <returns>Awesomeness</returns>",
        };

        Assert.Equal(expected, sb);
    }

    [Fact]
    public void MultiLineReturns()
    {
        var sb = new StructuredStringBuilder();

        using (var commentWrapper = new Comment(sb))
        {
            commentWrapper.Return.AppendLine("Awesomeness,");
            commentWrapper.Return.AppendLine("sheer awesomeness!");
        }

        var expected = new string[]
        {
            "/// <returns>",
            "/// Awesomeness,",
            "/// sheer awesomeness!",
            "/// </returns>",
        };

        Assert.Equal(expected, sb);
    }

    [Fact]
    public void WriteOnDispose()
    {
        var sb = new StructuredStringBuilder();

        using (var commentWrapper = new Comment(sb))
        {
            commentWrapper.Return.AppendLine("Awesomeness,");
            var description = new StructuredStringBuilder();
            description.AppendLine("The name of");
            description.AppendLine("the thing");

            commentWrapper.Parameters["name"] = description;

            commentWrapper.Summary.AppendLine("An awesome summary.");
            commentWrapper.Return.AppendLine("sheer awesomeness!");

            commentWrapper.AddParameter("thing", "The thing of the thing");
        }

        var expected = new string[]
        {
            "/// <summary>",
            "/// An awesome summary.",
            "/// </summary>",
            "/// <param name=\"name\">",
            "/// The name of",
            "/// the thing",
            "/// </param>",
            "/// <param name=\"thing\">The thing of the thing</param>",
            "/// <returns>",
            "/// Awesomeness,",
            "/// sheer awesomeness!",
            "/// </returns>",
        };

        Assert.Equal(expected, sb);
    }
}
