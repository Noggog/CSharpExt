using FluentAssertions;
using Noggog;
using Xunit;

namespace CSharpExt.UnitTests;

public class OrderedDictionaryTests
{
    private OrderedDictionary<string, int> TypicalDict()
    {
        return new OrderedDictionary<string, int>()
        {
            { "Hello", 2 },
            { "World", 5 },
        };
    }

    [Fact]
    public void EmptyHasZeroCount()
    {
        new OrderedDictionary<string, int>().Count.Should().Be(0);
    }
        
    [Fact]
    public void TypicalCount()
    {
        TypicalDict().Count.Should().Be(2);
    }
        
    [Fact]
    public void Get()
    {
        var coll = TypicalDict();
        coll.Get("Hello").Should().Be(2);
        coll.Get("World").Should().Be(5);
    }
        
    [Fact]
    public void GetOutOfRangeThrows()
    {
        var coll = TypicalDict();
        Assert.Throws<KeyNotFoundException>(() =>
        {
            coll.Get("Derp");
        });
    }
        
    [Fact]
    public void GetAtIndex()
    {
        var coll = TypicalDict();
        coll.GetAtIndex(0).Should().Be(new KeyValuePair<string, int>("Hello", 2));
        coll.GetAtIndex(1).Should().Be(new KeyValuePair<string, int>("World", 5));
    }
        
    [Fact]
    public void GetAtIndexOutOfRangeThrows()
    {
        var coll = TypicalDict();
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            coll.GetAtIndex(4);
        });
    }
        
    [Fact]
    public void Enumeration()
    {
        IEnumerable<KeyValuePair<string, int>> e = TypicalDict();
        e.Should().Equal(
            new KeyValuePair<string, int>("Hello", 2),
            new KeyValuePair<string, int>("World", 5));
    }
        
    [Fact]
    public void ContainsKey()
    {
        var dict = TypicalDict();
        dict.ContainsKey("Hello").Should().BeTrue();
        dict.ContainsKey("World").Should().BeTrue();
        dict.ContainsKey("Missing").Should().BeFalse();
    }
        
    [Fact]
    public void TryGetValue()
    {
        var dict = TypicalDict();
        dict.TryGetValue("Hello", out var index).Should().BeTrue();
        index.Should().Be(2);
        dict.TryGetValue("World", out index).Should().BeTrue();
        index.Should().Be(5);
        dict.TryGetValue("Miss", out index).Should().BeFalse();
    }
        
    [Fact]
    public void Add()
    {
        var dict = TypicalDict();
        dict.Add("New", 6);
        dict.ContainsKey("New").Should().BeTrue();
    }
        
    [Fact]
    public void AddCollisionThrows()
    {
        var dict = TypicalDict();
        Assert.Throws<ArgumentException>(() =>
        {
            dict.Add("Hello", 6);
        });
    }
        
    [Fact]
    public void AddOrReplaceAdds()
    {
        var dict = TypicalDict();
        dict.AddOrReplace("New", 6);
        dict.ContainsKey("New").Should().BeTrue();
    }
        
    [Fact]
    public void AddOrReplaceReplaces()
    {
        var dict = TypicalDict();
        dict.AddOrReplace("Hello", 6);
        dict.Get("Hello").Should().Be(6);
    }
        
    [Fact]
    public void InsertAtOnEmpty()
    {
        var dict = new OrderedDictionary<string, int>();
        dict.InsertAt("Hello", 0, 2);
        dict.GetAtIndex(0).Should().Be(new KeyValuePair<string, int>("Hello", 2));
        dict.Get("Hello").Should().Be(2);
    }
        
    [Fact]
    public void InsertAtEnd()
    {
        var dict = TypicalDict();
        dict.InsertAt("New", 2, 6);
        dict.Get("Hello").Should().Be(2);
        dict.GetAtIndex(0).Should().Be(new KeyValuePair<string, int>("Hello", 2));
        dict.Get("World").Should().Be(5);
        dict.GetAtIndex(1).Should().Be(new KeyValuePair<string, int>("World", 5));
        dict.Get("New").Should().Be(6);
        dict.GetAtIndex(2).Should().Be(new KeyValuePair<string, int>("New", 6));
    }
        
    [Fact]
    public void InsertAtMiddle()
    {
        var dict = TypicalDict();
        dict.InsertAt("New", 1, 6);
        dict.Get("Hello").Should().Be(2);
        dict.GetAtIndex(0).Should().Be(new KeyValuePair<string, int>("Hello", 2));
        dict.Get("New").Should().Be(6);
        dict.GetAtIndex(1).Should().Be(new KeyValuePair<string, int>("New", 6));
        dict.Get("World").Should().Be(5);
        dict.GetAtIndex(2).Should().Be(new KeyValuePair<string, int>("World", 5));
    }
        
    [Fact]
    public void InsertOutOfRangeThrows()
    {
        var dict = TypicalDict();
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            dict.InsertAt("New", 5, 6);
        });
    }
        
    [Fact]
    public void RemoveAt()
    {
        var dict = TypicalDict();
        dict.InsertAt("New", 1, 6);
        dict.RemoveAt(1);
        IEnumerable<KeyValuePair<string, int>> e = TypicalDict();
        e.Should().Equal(
            new KeyValuePair<string, int>("Hello", 2),
            new KeyValuePair<string, int>("World", 5));
    }
        
    [Fact]
    public void RemoveAtZero()
    {
        var dict = TypicalDict();
        dict.RemoveAt(0);
        dict.Count.Should().Be(1);
        dict.GetAtIndex(0).Should().Be(new KeyValuePair<string, int>("World", 5));
    }
        
    [Fact]
    public void RemoveAtEnd()
    {
        var dict = TypicalDict();
        dict.RemoveAt(1);
        dict.Count.Should().Be(1);
        dict.GetAtIndex(0).Should().Be(new KeyValuePair<string, int>("Hello", 2));
    }
        
    [Fact]
    public void RemoveKey()
    {
        var dict = TypicalDict();
        dict.RemoveKey("World").Should().BeTrue();
        dict.Count.Should().Be(1);
        dict.GetAtIndex(0).Should().Be(new KeyValuePair<string, int>("Hello", 2));
    }
        
    [Fact]
    public void RemoveMissingKeyThrows()
    {
        var dict = TypicalDict();
        dict.RemoveKey("Missing").Should().BeFalse();
    }
}