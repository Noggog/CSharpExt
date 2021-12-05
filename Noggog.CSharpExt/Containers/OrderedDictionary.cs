using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Noggog
{
    public interface IReadOnlyOrderedDictionary<TKey, TValue> : 
        IReadOnlyDictionary<TKey, TValue>, 
        IReadOnlyList<TValue>
        where TKey : notnull
    {
        new int Count { get; }
        new IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator();
        TValue Get(TKey key);
        KeyValuePair<TKey, TValue> GetAtIndex(int index);
    }

    public interface IOrderedDictionary<TKey, TValue> : IReadOnlyOrderedDictionary<TKey, TValue>
        where TKey : notnull
    {
        public void Add(TKey key, TValue value);

        public void AddOrReplace(TKey key, TValue value);

        public void InsertAt(TKey key, int index, TValue value);

        public void RemoveAt(int index);

        public bool RemoveKey(TKey key);
    }

    public class OrderedDictionary<TKey, TValue> : IOrderedDictionary<TKey, TValue>
        where TKey : notnull
    {
        private readonly Dictionary<TKey, int> _dictionary = new();
        private readonly List<KeyValuePair<TKey, TValue>> _list = new();

        TValue IReadOnlyDictionary<TKey, TValue>.this[TKey key] => Get(key);

        public IEnumerable<TKey> Keys => _dictionary.Keys;
        public IEnumerable<TValue> Values => _list.Select(x => x.Value);

        public int Count => _list.Count;

        TValue IReadOnlyList<TValue>.this[int index] => GetAtIndex(index).Value;
        
        IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator() => Values.GetEnumerator();

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            foreach (var index in _dictionary.Values)
            {
                yield return _list[index];
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        
        public TValue Get(TKey key) => _list[_dictionary[key]].Value;
        
        public KeyValuePair<TKey, TValue> GetAtIndex(int index) => _list[index];

        public bool ContainsKey(TKey key) => _dictionary.ContainsKey(key);

        public bool TryGetValue(TKey key, out TValue value)
        {
            if (!_dictionary.TryGetValue(key, out var index))
            {
                value = default!;
                return false;
            }

            value = _list[index].Value;
            return true;
        }

        public void Add(TKey key, TValue value)
        {
            ThrowIfExists(key);

            _dictionary[key] = _list.Count;
            _list.Add(new KeyValuePair<TKey, TValue>(key, value));
        }

        public void AddOrReplace(TKey key, TValue value)
        {
            if (!_dictionary.TryGetValue(key, out var index))
            {
                AddInternal(key, value);
                return;
            }

            _list[index] = new KeyValuePair<TKey, TValue>(key, value);
        }

        public void InsertAt(TKey key, int index, TValue value)
        {
            ThrowIfExists(key);
            InsertInternal(key, value, index);
        }

        public void RemoveAt(int index)
        {
            RemoveKey(_list[index].Key);
        }

        public bool RemoveKey(TKey key)
        {
            if (!_dictionary.TryGetValue(key, out var index)) return false;
            _list.RemoveAt(index);
            _dictionary.Remove(key);
            return true;
        }

        private void AddInternal(TKey key, TValue value)
        {
            SetInternal(key, value, _list.Count);
        }

        private void SetInternal(TKey key, TValue value, int index)
        {
            _dictionary[key] = index;
            _list.Add(new KeyValuePair<TKey, TValue>(key, value));
        }

        private void InsertInternal(TKey key, TValue value, int index)
        {
            _list.Insert(index, new KeyValuePair<TKey, TValue>(key, value));
            for (int i = index; i < _list.Count; i++)
            {
                var kv = _list[i];
                _dictionary[kv.Key] = i;
            }
        }

        private void ThrowIfExists(TKey key)
        {
            if (_dictionary.ContainsKey(key))
            {
                throw new ArgumentException($"Key already exists in collection: {key}");
            }
        }
    }
}