#if NETSTANDARD2_0
#else
using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Noggog
{
    public class DictionaryCovariantWrapper<TKey, TSourceValue, TTargetValue> : IReadOnlyDictionary<TKey, TTargetValue>
        where TSourceValue : TTargetValue
    {
        private readonly IReadOnlyDictionary<TKey, TSourceValue> _dict;

        public DictionaryCovariantWrapper(IReadOnlyDictionary<TKey, TSourceValue> dict)
        {
            _dict = dict;
        }

        public TTargetValue this[TKey key] => _dict[key];

        public IEnumerable<TKey> Keys => _dict.Keys;

        public IEnumerable<TTargetValue> Values => _dict.Values.Select<TSourceValue, TTargetValue>(v => v);

        public int Count => _dict.Count;

        public bool ContainsKey(TKey key)
        {
            return _dict.ContainsKey(key);
        }

        public IEnumerator<KeyValuePair<TKey, TTargetValue>> GetEnumerator()
        {
            return _dict.Select(kv => new KeyValuePair<TKey, TTargetValue>(kv.Key, kv.Value)).GetEnumerator();
        }

        public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TTargetValue value)
        {
            if (!_dict.TryGetValue(key, out var source))
            {
                value = default;
                return false;
            }
            value = source;
            return true;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_dict).GetEnumerator();
        }
    }
}
#endif