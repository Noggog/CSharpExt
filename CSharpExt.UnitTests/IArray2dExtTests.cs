using Noggog;
using Shouldly;

namespace CSharpExt.UnitTests;

public class IArray2dExtTests
{
    [Fact]
    public void SequenceEqualNullableSameValues()
    {
        IReadOnlyArray2d<int> lhs = Numbered(2, 3);
        IReadOnlyArray2d<int> rhs = Numbered(2, 3);
        lhs.SequenceEqualNullable(rhs).ShouldBeTrue();
    }

    [Fact]
    public void SequenceEqualNullableDifferentValue()
    {
        IReadOnlyArray2d<int> lhs = Numbered(2, 3);
        var rhs = Numbered(2, 3);
        rhs[1, 2] = -1;
        lhs.SequenceEqualNullable(rhs).ShouldBeFalse();
    }

    [Fact]
    public void SequenceEqualNullableDifferentSize()
    {
        IReadOnlyArray2d<int> lhs = new Array2d<int>(2, 3, 0);
        IReadOnlyArray2d<int> rhs = new Array2d<int>(3, 2, 0);
        lhs.SequenceEqualNullable(rhs).ShouldBeFalse();
    }

    [Fact]
    public void SequenceEqualNullableNull()
    {
        IReadOnlyArray2d<int>? missing = null;
        missing.SequenceEqualNullable(null).ShouldBeTrue();
        missing.SequenceEqualNullable(Numbered(2, 3)).ShouldBeFalse();
        ((IReadOnlyArray2d<int>)Numbered(2, 3)).SequenceEqualNullable(null).ShouldBeFalse();
    }

    private static Array2d<int> Numbered(int width, int height)
    {
        var array = new Array2d<int>(width, height, 0);
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                array[x, y] = y * width + x;
            }
        }
        return array;
    }
}
