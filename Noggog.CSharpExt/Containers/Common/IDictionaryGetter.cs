using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noggog
{
    public interface IDictionaryGetter<K, V> : ICollectionGetter<KeyValuePair<K, V>>
    {
        V this[K key] { get; }
        bool TryGetValue(K key, out V val);
        ICollectionGetter<K> Keys { get; }
        ICollectionGetter<V> Values { get; }
    }

    public class DictionaryGetterWrapper<K, V> : IDictionaryGetter<K, V>
    {
        protected IDictionary<K, V> dict;

        public int Count => dict.Count;

        public ICollectionGetter<K> Keys => new CollectionGetterWrapper<K>(dict.Keys);

        public ICollectionGetter<V> Values => new CollectionGetterWrapper<V>(dict.Values);

        public V this[K key] => dict[key];

        public DictionaryGetterWrapper(IDictionary<K, V> dict)
        {
            this.dict = dict;
        }

        public bool Contains(KeyValuePair<K, V> item)
        {
            return dict.Contains(item);
        }

        public void CopyTo(KeyValuePair<K, V>[] array, int arrayIndex)
        {
            dict.CopyTo(array, arrayIndex);
        }

        public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
        {
            return dict.GetEnumerator();
        }

        public bool TryGetValue(K key, out V val)
        {
            return dict.TryGetValue(key, out val);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
