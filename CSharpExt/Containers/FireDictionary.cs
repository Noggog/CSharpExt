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
        private readonly object _lock = new object();
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
            lock (_lock)
            {
                _dict[key] = value;
                _fireDict = null;
            }
        }

        public void Add(K key, V value)
        {
            lock (_lock)
            {
                _dict.Add(key, value);
                _fireDict = null;
            }
        }

        public void Add(KeyValuePair<K, V> item)
        {
            lock (_lock)
            {
                _dict.Add(item.Key, item.Value);
                _fireDict = null;
            }
        }

        public void Clear()
        {
            lock (_lock)
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
            lock (_lock)
            {
                if (!_dict.Remove(key)) return false;
                _fireDict = null;
                return true;
            }
        }

        public bool Remove(KeyValuePair<K, V> item)
        {
            lock (_lock)
            {
                if (!((IDictionary<K, V>)_dict).Remove(item)) return false;
                _fireDict = null;
                return true;
            }
        }

        public bool TryGetValue(K key, out V value)
        {
            return GetFireDictionary().TryGetValue(key, out value);
        }

        public IDictionary<K, V> GetFireDictionary()
        {
            lock (_lock)
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
