using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noggog
{
    public class SortingListDictionary<TKey, TValue> : ISortingListDictionary<TKey, TValue>
        where TKey : notnull
    {
        private readonly IList<TKey> _internalKeys;
        private readonly IList<TValue> _internalValues;
        private readonly SortingList<TKey> _internalSortedKeys;
        private readonly SortingList<TValue> _internalSortedValues;
        public ISortedListGetter<TKey> Keys => _internalSortedKeys;
        public ISortedListGetter<TValue> Values => _internalSortedValues;
        public int Count => _internalKeys.Count;

        public TValue this[TKey key]
        {
            get => Get(key);
            set => Add(key, value);
        }

        public SortingListDictionary()
        {
            _internalKeys = new List<TKey>();
            _internalValues = new List<TValue>();
            _internalSortedKeys = SortingList<TKey>.FactoryWrapAssumeSorted(_internalKeys);
            _internalSortedValues = SortingList<TValue>.FactoryWrapAssumeSorted(_internalValues);
        }

        private SortingListDictionary(
            IList<TKey> keys,
            IList<TValue> values)
        {
            _internalKeys = keys;
            _internalValues = values;
            _internalSortedKeys = SortingList<TKey>.FactoryWrapAssumeSorted(_internalKeys);
            _internalSortedValues = SortingList<TValue>.FactoryWrapAssumeSorted(_internalValues);
        }

        public SortingListDictionary(IEnumerable<KeyValuePair<TKey, TValue>> e)
            : this()
        {
            foreach (var item in e)
            {
                this.Add(item);
            }
        }

        public static SortingListDictionary<TKey, TValue> Factory_Wrap_AssumeSorted(
            IList<TKey> keys,
            IList<TValue> values)
        {
            if (keys.Count != values.Count)
            {
                throw new ArgumentException($"Key and value counts did not match: {keys.Count} != {values.Count}");
            }

            return new SortingListDictionary<TKey, TValue>(
                keys,
                values);
        }

        private TValue Get(TKey key)
        {
            var search = _internalKeys.BinarySearch(key);
            if (search >= 0)
            {
                return _internalValues[search];
            }
            throw new KeyNotFoundException();
        }

        public void Add(TKey key, TValue value)
        {
            var search = _internalKeys.BinarySearch(key);
            if (search >= 0)
            {
                _internalKeys[search] = key;
                _internalValues[search] = value;
            }
            else
            {
                search = ~search;
                _internalKeys.Insert(search, key);
                _internalValues.Insert(search, value);
            }
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            this.Add(item.Key, item.Value);
        }

        public int IndexOf(TKey key)
        {
            return _internalSortedKeys.IndexOf(key);
        }

        public bool ContainsKey(TKey key)
        {
            return _internalSortedKeys.Contains(key);
        }

        public bool Remove(TKey key)
        {
            var search = _internalKeys.BinarySearch(key);
            if (search < 0)
            {
                return false;
            }
            _internalKeys.RemoveAt(search);
            _internalValues.RemoveAt(search);
            return true;
        }

        public void RemoveAt(int index)
        {
            _internalKeys.RemoveAt(index);
            _internalValues.RemoveAt(index);
        }

#pragma warning disable CS8767 // Nullability of reference types in type of parameter doesn't match implicitly implemented member (possibly because of nullability attributes).
        public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
#pragma warning restore CS8767 // Nullability of reference types in type of parameter doesn't match implicitly implemented member (possibly because of nullability attributes).
        {
            var search = _internalKeys.BinarySearch(key);
            if (search < 0)
            {
                value = default;
                return false;
            }
            value = _internalValues[search];
            return true;
        }

        public void Clear()
        {
            _internalValues.Clear();
            _internalKeys.Clear();
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            for (int i = 0; i < _internalKeys.Count; i++)
            {
                yield return new KeyValuePair<TKey, TValue>(
                    _internalKeys[i],
                    _internalValues[i]);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #region Hidden Interfaces

        ICollection<TKey> IDictionary<TKey, TValue>.Keys => _internalKeys;
        ICollection<TValue> IDictionary<TKey, TValue>.Values => _internalValues;
        ICollection IDictionary.Keys => _internalSortedKeys;
        ICollection IDictionary.Values => _internalSortedValues;
        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => _internalKeys;
        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => _internalValues;
        object ICollection.SyncRoot => throw new NotImplementedException();
        bool ICollection.IsSynchronized => throw new NotImplementedException();
        bool IDictionary.IsFixedSize => false;
        bool IDictionary.IsReadOnly => false;
        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => false;

        object? IDictionary.this[object? key]
        {
            get
            {
                if (key == null) throw new NullReferenceException();
                return this.Get((TKey)key);
            }
            set
            {
                if (key == null) throw new NullReferenceException();
                if (!(value is TValue v)) throw new ArgumentException();
                this.Add((TKey)key, v);
            }
        }

        bool IDictionary.Contains(object key)
        {
            return this.ContainsKey((TKey)key);
        }

        void IDictionary.Add(object key, object value)
        {
            this.Add((TKey)key, (TValue)value);
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        void IDictionary.Remove(object key)
        {
            this.Remove((TKey)key);
        }

        void ICollection.CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
        {
            if (!this.TryGetValue(item.Key, out var value))
            {
                return false;
            }
            return EqualityComparer<TValue>.Default.Equals(item.Value, value);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            var search = _internalKeys.BinarySearch(item.Key);
            if (search < 0)
            {
                return false;
            }
            _internalKeys.RemoveAt(search);
            _internalValues.RemoveAt(search);
            return true;
        }
        #endregion

        #region PreSortedListExt
        public bool TryGetIndexInDirection(TKey key, bool higher, out int result)
        {
            return _internalSortedKeys.TryGetIndexInDirection(
                item: key,
                higher: higher,
                result: out result);
        }

        public bool TryGetValueInDirection(TKey key, bool higher, [MaybeNullWhen(false)] out TValue result)
        {
            if (_internalSortedKeys.TryGetIndexInDirection(
                item: key,
                higher: higher,
                result: out var index))
            {
                result = _internalSortedValues[index];
                return true;
            }
            result = default;
            return false;
        }

        public bool TryGetInDirection(TKey key, bool higher, out KeyValuePair<int, TValue> result)
        {
            if (_internalSortedKeys.TryGetIndexInDirection(
                item: key,
                higher: higher,
                result: out var index))
            {
                result = new KeyValuePair<int, TValue>(
                    index,
                    _internalSortedValues[index]);
                return true;
            }
            result = default;
            return false;
        }

        public bool TryGetEncapsulatedIndices(TKey lowerKey, TKey higherKey, out RangeInt32 result)
        {
            return _internalSortedKeys.TryGetEncapsulatedIndices(
                lowerKey,
                higherKey,
                out result);
        }

        public bool TryGetEncapsulatedValues(TKey lowerKey, TKey higherKey, [MaybeNullWhen(false)] out IEnumerable<KeyValuePair<int, TValue>> result)
        {
            if (!_internalSortedKeys.TryGetEncapsulatedIndices(
                lowerKey,
                higherKey,
                out var indices))
            {
                result = default;
                return false;
            }
            result = indices.Select((i) => new KeyValuePair<int, TValue>(i, _internalSortedValues[i]));
            return true;
        }
        #endregion
    }
}
