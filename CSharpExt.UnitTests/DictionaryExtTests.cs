using System.Collections.Concurrent;
using FluentAssertions;
using Noggog;
using Noggog.Testing.AutoFixture;

namespace CSharpExt.UnitTests;

public class DictionaryExtTests
{
    [Theory, DefaultAutoData]
    public void GetOrAddFunc(
        string key,
        int value,
        int value2)
    {
        var d = new Dictionary<string, int>();
        var s = d.GetOrAdd(key, (s) =>
        {
            s.Should().Be(key);
            return value;
        });
        s.Should().Be(value);
        var s2 = d.GetOrAdd(key, (s) =>
        {
            s.Should().Be(key);
            return value2;
        });
        s2.Should().Be(value);
    }
    
    [Theory, DefaultAutoData]
    public void GetOrAddDefault(
        string key)
    {
        var d = new Dictionary<string, int>();
        var s = d.GetOrAdd(key);
        s.Should().Be(default(int));
        var s2 = d.GetOrAdd(key);
        s2.Should().Be(default(int));
    }
    
    [Theory, DefaultAutoData]
    public void ConcurrentDictionaryGetOrAdd(
        string key,
        int value,
        int value2)
    {
        var d = new ConcurrentDictionary<string, int>();
        var s = d.GetOrAdd(key, (s) =>
        {
            s.Should().Be(key);
            return value;
        });
        s.Should().Be(value);
        var s2 = d.GetOrAdd(key, (s) =>
        {
            s.Should().Be(key);
            return value2;
        });
        s2.Should().Be(value);
    }
    
    [Theory, DefaultAutoData]
    public void UpdateOrAddWithKey(
        string key,
        int value,
        int value2)
    {
        var d = new Dictionary<string, int>();
        var s = d.UpdateOrAdd(key, (k, s) =>
        {
            k.Should().Be(key);
            s.Should().Be(default(int));
            return value;
        });
        s.Should().Be(value);
        var s2 = d.UpdateOrAdd(key, (k, s) =>
        {
            k.Should().Be(key);
            s.Should().Be(value);
            return value2;
        });
        s2.Should().Be(value2);
    }
    
    [Theory, DefaultAutoData]
    public void IDictionaryUpdateOrAddWithKey(
        string key,
        int value,
        int value2)
    {
        IDictionary<string, int> d = new Dictionary<string, int>();
        var s = d.UpdateOrAdd(key, (k, s) =>
        {
            k.Should().Be(key);
            s.Should().Be(default(int));
            return value;
        });
        s.Should().Be(value);
        var s2 = d.UpdateOrAdd(key, (k, s) =>
        {
            k.Should().Be(key);
            s.Should().Be(value);
            return value2;
        });
        s2.Should().Be(value2);
    }
    
    [Theory, DefaultAutoData]
    public void UpdateOrAdd(
        string key,
        int value,
        int value2)
    {
        var d = new Dictionary<string, int>();
        var s = d.UpdateOrAdd(key, (s) =>
        {
            s.Should().Be(default(int));
            return value;
        });
        s.Should().Be(value);
        var s2 = d.UpdateOrAdd(key, (s) =>
        {
            s.Should().Be(value);
            return value2;
        });
        s2.Should().Be(value2);
    }
    
    [Theory, DefaultAutoData]
    public void IDictionaryUpdateOrAdd(
        string key,
        int value,
        int value2)
    {
        IDictionary<string, int> d = new Dictionary<string, int>();
        var s = d.UpdateOrAdd(key, (s) =>
        {
            s.Should().Be(default(int));
            return value;
        });
        s.Should().Be(value);
        var s2 = d.UpdateOrAdd(key, (s) =>
        {
            s.Should().Be(value);
            return value2;
        });
        s2.Should().Be(value2);
    }
}