namespace Noggog;

public interface IReadOnlyCache<out TValue, TKey> : IReadOnlyCollection<IKeyValue<TKey, TValue>>
{
    /// <summary>
    /// Gets the element that has the specified key in the read-only dictionary.
    /// </summary>
    /// <param name="key">The key to locate.</param>
    /// <exception cref="ArgumentNullException">key is null</exception>
    /// <exception cref="KeyNotFoundException">The property is retrieved and key is not found.</exception>
    TValue this[TKey key] { get; }

    /// <summary>
    /// An enumerable collection that contains the keys in the read-only dictionary.
    /// </summary>
    IEnumerable<TKey> Keys { get; }
        
    /// <summary>
    /// An enumerable collection that contains the values in the read-only dictionary.
    /// </summary>
    IEnumerable<TValue> Items { get; }

    /// <summary>
    /// Determines whether the read-only dictionary contains an element that has the specified key.
    /// </summary>
    /// <param name="key">The key to locate.</param>
    /// <returns>true if the read-only dictionary contains an element that has the specified key; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">key is null.</exception>
    bool ContainsKey(TKey key);

    /// <summary>
    /// Try to retrieve item with the given key
    /// </summary>
    /// <param name="key">The key to query.</param>
    /// <returns>A TryGet struct with the results</returns>
    TValue? TryGetValue(TKey key);
}