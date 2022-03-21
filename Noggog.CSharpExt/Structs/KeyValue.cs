namespace Noggog;

public readonly struct KeyValue<TKey, TValue> : IKeyValue<TKey, TValue>
{
    public TKey Key { get; }
    public TValue Value { get; }

    public KeyValue(TKey key, TValue value)
    {
        this.Key = key;
        this.Value = value;
    }
}
