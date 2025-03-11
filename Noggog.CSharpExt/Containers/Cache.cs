using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Noggog;

public class Cache<TObject, TKey> : ICache<TObject, TKey>
    where TKey : notnull
{
    public static readonly IReadOnlyCache<TObject, TKey> Empty = new Cache<TObject, TKey>(_ => default!);

    private readonly Func<TObject, TKey> _keySelector;
    private readonly Dictionary<TKey, TObject> _dict;

    public TObject this[TKey key] => _dict[key];

    public IEnumerable<TKey> Keys => _dict.Keys;

    public int Count => _dict.Count;

    public IEnumerable<TObject> Items => _dict.Values;

    public IEnumerable<KeyValuePair<TKey, TObject>> KeyValues => _dict;

    public Cache(Func<TObject, TKey> keySelector, IEqualityComparer<TKey>? equalityComparer = null)
    {
        _dict = new Dictionary<TKey, TObject>(equalityComparer);
        _keySelector = keySelector ?? throw new ArgumentNullException(nameof(keySelector));
    }

    public void Clear() => _dict.Clear();

    public bool ContainsKey(TKey key) => _dict.ContainsKey(key);

    public bool Remove(TKey key) => _dict.Remove(key);

    public void Remove(IEnumerable<TKey> keys)
    {
        foreach (var key in keys)
        {
            _dict.Remove(key);
        }
    }

    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TObject value) => _dict.TryGetValue(key, out value);
    
    public TObject? TryGetValue(TKey key)
    {
        if (_dict.TryGetValue(key, out var val))
        {
            return val;
        }

        return default;
    }

    public IEnumerator<IKeyValue<TKey, TObject>> GetEnumerator()
    {
        foreach (var item in _dict)
        {
            yield return new KeyValue<TKey, TObject>(item.Key, item.Value);
        }
    }

    IEnumerator<IKeyValue<TKey, TObject>> IEnumerable<IKeyValue<TKey, TObject>>.GetEnumerator() => GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void Set(TObject item) => _dict[_keySelector(item)] = item;

    public void Set(IEnumerable<TObject> items)
    {
        foreach (var item in items)
        {
            _dict[_keySelector(item)] = item;
        }
    }

    public void Add(TObject item)
    {
        _dict.Add(_keySelector(item), item);
    }

    public bool Remove(TObject obj) => _dict.Remove(_keySelector(obj));

    public void Remove(IEnumerable<TObject> objects)
    {
        foreach (var item in objects)
        {
            _dict.Remove(_keySelector(item));
        }
    }

    public TObject GetOrAdd(TKey key, Func<TKey, TObject> createFunc)
    {
        if (_dict.TryGetValue(key, out var val)) return val;
        val = createFunc(key);
        Set(val);
        return val;
    }
}
