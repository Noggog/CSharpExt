using Noggog;

namespace CSharpExt.UnitTests;

public class SortingListDictionaryTests
{
    public const int TypicalCount = 3;
    public const int LowIndex = 0;
    public const int MiddleIndex = 1;
    public const int HighIndex = 2;
    public static WrappedInt LowKey => new WrappedInt(-3);
    public static WrappedInt MiddleKey => new WrappedInt(6);
    public static WrappedInt HighKey => new WrappedInt(155);
    public static string LowItem => "Test1";
    public static string MiddleItem => "Test2";
    public static string HighItem => "Test3";
    public static WrappedInt MissingMiddleKey => new WrappedInt(8);
    public static WrappedInt MissingLowKey => new WrappedInt(-1000);
    public static WrappedInt MissingHighKey => new WrappedInt(1000);
    public static string MissingLowItem => "MissingTest1";
    public static string MissingMiddleItem => "MissingTest2";
    public static string MissingHighItem => "MissingTest3";

    public List<WrappedInt> TypicalSortedKeys()
    {
        return new List<WrappedInt>()
        {
            LowKey,
            MiddleKey,
            HighKey
        };
    }

    public List<string> TypicalSortedValues()
    {
        return new List<string>()
        {
            LowItem,
            MiddleItem,
            HighItem,
        };
    }

    public SortingListDictionary<WrappedInt, string> Typical()
    {
        return SortingListDictionary<WrappedInt, string>.Factory_Wrap_AssumeSorted(
            TypicalSortedKeys(),
            TypicalSortedValues());
    }

    #region Factories
    [Fact]
    public void Factory_Wrap_AssumeSorted()
    {
        var list = SortingListDictionary<WrappedInt, string>.Factory_Wrap_AssumeSorted(
            TypicalSortedKeys(),
            TypicalSortedValues());
        Assert.Equal(TypicalCount, list.Count);
        Assert.True(Typical().SequenceEqual(list));
    }

    [Fact]
    public void Factory_Wrap_AssumeSorted_But_Not()
    {
        var badInts = new int[] { 4, 2, 5 };
        var list = SortingListDictionary<int, string>.Factory_Wrap_AssumeSorted(
            new List<int>(badInts),
            TypicalSortedValues());
        Assert.Equal(badInts.Length, list.Count);
        Assert.True(badInts.SequenceEqual(list.Keys));
        Assert.True(TypicalSortedValues().SequenceEqual(list.Values));
    }
    #endregion

    #region Add
    #region Missing
    [Fact]
    public void Add_Missing()
    {
        var list = Typical();
        list.Add(MissingMiddleKey, MissingMiddleItem);
        Assert.Equal(TypicalCount + 1, list.Count);
        var rhsKeys = TypicalSortedKeys();
        rhsKeys.Insert(2, MissingMiddleKey);
        var rhsValues = TypicalSortedValues();
        rhsValues.Insert(2, MissingMiddleItem);
        Assert.True(
            rhsKeys
                .SequenceEqual(list.Keys));
        Assert.True(
            rhsValues
                .SequenceEqual(list.Values));
    }

    [Fact]
    public void Add_MissingLow()
    {
        var list = Typical();
        list.Add(MissingLowKey, MissingLowItem);
        Assert.Equal(TypicalCount + 1, list.Count);
        var rhsKeys = TypicalSortedKeys();
        rhsKeys.Insert(0, MissingLowKey);
        var rhsValues = TypicalSortedValues();
        rhsValues.Insert(0, MissingLowItem);
        Assert.True(
            rhsKeys
                .SequenceEqual(list.Keys));
        Assert.True(
            rhsValues
                .SequenceEqual(list.Values));
    }

    [Fact]
    public void Add_MissingHigh()
    {
        var list = Typical();
        list.Add(MissingHighKey, MissingHighItem);
        Assert.Equal(TypicalCount + 1, list.Count);
        var rhsKeys = TypicalSortedKeys();
        rhsKeys.Add(MissingHighKey);
        var rhsValues = TypicalSortedValues();
        rhsValues.Add(MissingHighItem);
        Assert.True(
            rhsKeys
                .SequenceEqual(list.Keys));
        Assert.True(
            rhsValues
                .SequenceEqual(list.Values));
    }
    #endregion
    #region Collide
    [Fact]
    public void Add_Collide()
    {
        var list = Typical();
        list.Add(MiddleKey, MiddleItem);
        Assert.Equal(TypicalCount, list.Count);
        Assert.True(Typical().SequenceEqual(list));
    }

    [Fact]
    public void Add_CollideLow()
    {
        var list = Typical();
        list.Add(LowKey, LowItem);
        Assert.Equal(TypicalCount, list.Count);
        Assert.True(Typical().SequenceEqual(list));
    }

    [Fact]
    public void Add_CollideHigh()
    {
        var list = Typical();
        list.Add(HighKey, HighItem);
        Assert.Equal(TypicalCount, list.Count);
        Assert.True(Typical().SequenceEqual(list));
    }
    #endregion
    #endregion

    #region Misc
    [Fact]
    public void Empty()
    {
        var list = new SortingListDictionary<WrappedInt, string>();
        Assert.Empty(list);
    }

    [Fact]
    public void Count()
    {
        var list = Typical();
        Assert.Equal(TypicalCount, list.Count);
    }

    [Fact]
    public void Clear_Empty()
    {
        var list = new SortingListDictionary<WrappedInt, string>();
        list.Clear();
        Assert.Empty(list);
    }

    [Fact]
    public void Clear()
    {
        var list = Typical();
        list.Clear();
        Assert.Empty(list);
    }
    #endregion

    #region IndexOf
    [Fact]
    public void IndexOf_Empty()
    {
        var list = new SortingListDictionary<int, string>();
        Assert.Equal(-1, list.IndexOf(4));
    }

    [Fact]
    public void IndexOf()
    {
        var list = Typical();
        Assert.Equal(MiddleIndex, list.IndexOf(MiddleKey));
    }

    [Fact]
    public void IndexOf_Missing()
    {
        var list = Typical();
        Assert.Equal(-1, list.IndexOf(MissingMiddleKey));
    }
    #endregion

    #region ContainsKey
    [Fact]
    public void ContainsKey_Empty()
    {
        var list = new SortingListDictionary<int, string>();
        Assert.False(list.ContainsKey(4));
    }

    [Fact]
    public void Contains()
    {
        var list = Typical();
        Assert.True(list.ContainsKey(MiddleKey));
    }

    [Fact]
    public void Contains_Missing()
    {
        var list = Typical();
        Assert.False(list.ContainsKey(MissingMiddleKey));
    }
    #endregion

    #region Removes
    #region Remove
    [Fact]
    public void RemoveAt()
    {
        var list = Typical();
        list.RemoveAt(MiddleIndex);
        Assert.Equal(TypicalCount - 1, list.Count);
        Assert.Equal(LowItem, list.Values[0]);
        Assert.Equal(HighItem, list.Values[1]);
        Assert.Equal(LowKey, list.Keys[0]);
        Assert.Equal(HighKey, list.Keys[1]);
    }

    [Fact]
    public void Remove()
    {
        var list = Typical();
        Assert.True(list.Remove(MiddleKey));
        Assert.Equal(TypicalCount - 1, list.Count);
        Assert.Equal(LowItem, list.Values[0]);
        Assert.Equal(HighItem, list.Values[1]);
        Assert.Equal(LowKey, list.Keys[0]);
        Assert.Equal(HighKey, list.Keys[1]);
    }

    [Fact]
    public void Remove_Missing()
    {
        var list = Typical();
        Assert.False(list.Remove(MissingMiddleKey));
    }
    #endregion
    #endregion
}