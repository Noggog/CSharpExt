using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noggog
{
    /// <summary>
    /// A readonly interface of IDictionary<K, V>.
    /// IDictionary<K, V> itself will not implement this, which makes some APIs a bit tricky.
    /// </summary>
    /// <typeparam name="K">Key Type</typeparam>
    /// <typeparam name="V">Value Type</typeparam>
    public interface IDictionaryGetter<K, V> : ICollectionGetter<KeyValuePair<K, V>>
    {
        /// <summary>
        /// Gets the element with the specified key.
        /// </summary>
        V this[K key] { get; }
        
        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key whose value to get.</param>
        /// <param name="value">
        /// When this method returns, the value associated with the specified key, if the
        /// key is found; otherwise, the default value for the type of the value parameter.
        /// This parameter is passed uninitialized.
        /// </param>
        /// <returns>
        /// true if the object that implements IDictionaryGetter contains
        /// an element with the specified key; otherwise, false.
        /// </returns>
        /// <exception cref="ArgumentNullException">key is null.</exception>
        bool TryGetValue(K key, out V value);
        
        /// <summary>
        /// An ICollection containing the keys of the object that implements IDictionaryGetter.
        /// </summary>
        ICollectionGetter<K> Keys { get; }
        
        /// <summary>
        /// An ICollection containing the values of the object that implements IDictionaryGetter.
        /// </summary>
        ICollectionGetter<V> Values { get; }
    }

    /// <summary>
    /// A wrapper class around an IDictionary to expose it as an IDictionaryGetter
    /// IDictionary itself does not implement the Getter interface, which makes this wrapper necessary.
    /// </summary>
    /// <typeparam name="K">Key Type</typeparam>
    /// <typeparam name="V">Value Type</typeparam>
    public class DictionaryGetterWrapper<K, V> : IDictionaryGetter<K, V>
    {
        protected IDictionary<K, V> dict;

        /// <inheritdoc />
        public int Count => dict.Count;

        /// <inheritdoc />
        public ICollectionGetter<K> Keys => new CollectionGetterWrapper<K>(dict.Keys);

        /// <inheritdoc />
        public ICollectionGetter<V> Values => new CollectionGetterWrapper<V>(dict.Values);

        /// <inheritdoc />
        public V this[K key] => dict[key];

        public DictionaryGetterWrapper(IDictionary<K, V> dict)
        {
            this.dict = dict;
        }

        /// <inheritdoc />
        public bool Contains(KeyValuePair<K, V> item)
        {
            return dict.Contains(item);
        }

        /// <inheritdoc />
        public void CopyTo(KeyValuePair<K, V>[] array, int arrayIndex)
        {
            dict.CopyTo(array, arrayIndex);
        }

        /// <inheritdoc />
        public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
        {
            return dict.GetEnumerator();
        }

        /// <inheritdoc />
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
