namespace Noggog;

/// <summary>
/// A keyed value.
/// Useful compared to KeyValuePair as the interface is covariant
/// </summary>
/// <typeparam name="TKey">The type of the key.</typeparam>
/// <typeparam name="TObject">The type of the object.</typeparam>
public interface IKeyValue<out TKey, out TObject>
{
    /// <summary>
    /// The key
    /// </summary>
    TKey Key { get; }

    /// <summary>
    /// The value
    /// </summary>
    TObject Value { get; }
}
