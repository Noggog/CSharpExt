using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noggog
{
    /*
     * Threadsafe dictionary that copies to a second internal dictionary when modified mid usage
     */
    public class FireDictionary<K, V> : IDictionary<K, V>
    {
        private readonly Dictionary<K, V> _dict = new Dictionary<K, V>();
        private Dictionary<K, V> _fireDict;

        public V this[K key]
        {
            get => GetFireDictionary()[key];
            set => Set(key, value);
        }

        public ICollection<K> Keys => GetFireDictionary().Keys;
        public ICollection<V> Values => GetFireDictionary().Values;
        public int Count => GetFireDictionary().Count;
        public bool IsReadOnly => false;

        public void Set(K key, V value)
        {
            lock (_dict)
            {
                _dict[key] = value;
                _fireDict = null;
            }
        }

        public void Add(K key, V value)
        {
            lock (_dict)
            {
                _dict.Add(key, value);
                _fireDict = null;
            }
        }

        public void Add(KeyValuePair<K, V> item)
        {
            lock (_dict)
            {
                _dict.Add(item.Key, item.Value);
                _fireDict = null;
            }
        }

        public void Clear()
        {
            lock (_dict)
            {
                _dict.Clear();
                if (_fireDict.Count > 0)
                {
                    _fireDict = null;
                }
            }
        }

        public bool Contains(KeyValuePair<K, V> item)
        {
            return GetFireDictionary().Contains(item);
        }

        public bool ContainsKey(K key)
        {
            return GetFireDictionary().ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<K, V>[] array, int arrayIndex)
        {
            GetFireDictionary().CopyTo(array, arrayIndex);
        }

        public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
        {
            return GetFireDictionary().GetEnumerator();
        }

        public bool Remove(K key)
        {
            lock (_dict)
            {
                if (!_dict.Remove(key)) return false;
                _fireDict = null;
                return true;
            }
        }

        public bool Remove(KeyValuePair<K, V> item)
        {
            lock (_dict)
            {
                if (!((IDictionary<K, V>)_dict).Remove(item)) return false;
                _fireDict = null;
                return true;
            }
        }

        public bool TryGetValue(K key, out V value)
        {
            lock (_dict)
            {
                return _dict.TryGetValue(key, out value);
            }
        }

        public V TryCreateValue(K key, Func<V> newFunc)
        {
            lock (_dict)
            {
                return _dict.TryCreateValue(key, newFunc);
            }
        }

        public IDictionary<K, V> GetFireDictionary()
        {
            lock (_dict)
            {
                if (_fireDict == null)
                {
                    _fireDict = new Dictionary<K, V>(_dict);
                }
                return _fireDict;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetFireDictionary().GetEnumerator();
    }
}
