using System.Collections.Concurrent;
using Noggog;
using Noggog.Testing.AutoFixture;
using Shouldly;

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
            s.ShouldBe(key);
            return value;
        });
        s.ShouldBe(value);
        var s2 = d.GetOrAdd(key, (s) =>
        {
            s.ShouldBe(key);
            return value2;
        });
        s2.ShouldBe(value);
    }
    
    [Theory, DefaultAutoData]
    public void GetOrAddNoKeyFunc(
        string key,
        int value,
        int value2)
    {
        var d = new Dictionary<string, int>();
        var s = d.GetOrAdd(key, () =>
        {
            return value;
        });
        s.ShouldBe(value);
        var s2 = d.GetOrAdd(key, () =>
        {
            return value2;
        });
        s2.ShouldBe(value);
    }
    
    [Theory, DefaultAutoData]
    public void GetOrAddDefault(
        string key)
    {
        var d = new Dictionary<string, int>();
        var s = d.GetOrAdd(key);
        s.ShouldBe(default(int));
        var s2 = d.GetOrAdd(key);
        s2.ShouldBe(default(int));
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
            s.ShouldBe(key);
            return value;
        });
        s.ShouldBe(value);
        var s2 = d.GetOrAdd(key, (s) =>
        {
            s.ShouldBe(key);
            return value2;
        });
        s2.ShouldBe(value);
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
            k.ShouldBe(key);
            s.ShouldBe(default(int));
            return value;
        });
        s.ShouldBe(value);
        var s2 = d.UpdateOrAdd(key, (k, s) =>
        {
            k.ShouldBe(key);
            s.ShouldBe(value);
            return value2;
        });
        s2.ShouldBe(value2);
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
            k.ShouldBe(key);
            s.ShouldBe(default(int));
            return value;
        });
        s.ShouldBe(value);
        var s2 = d.UpdateOrAdd(key, (k, s) =>
        {
            k.ShouldBe(key);
            s.ShouldBe(value);
            return value2;
        });
        s2.ShouldBe(value2);
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
            s.ShouldBe(default(int));
            return value;
        });
        s.ShouldBe(value);
        var s2 = d.UpdateOrAdd(key, (s) =>
        {
            s.ShouldBe(value);
            return value2;
        });
        s2.ShouldBe(value2);
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
            s.ShouldBe(default(int));
            return value;
        });
        s.ShouldBe(value);
        var s2 = d.UpdateOrAdd(key, (s) =>
        {
            s.ShouldBe(value);
            return value2;
        });
        s2.ShouldBe(value2);
    }
}