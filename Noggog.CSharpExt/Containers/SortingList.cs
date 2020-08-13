using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noggog
{
    public class SortingList<T> : ISortedList<T>
    {
        private readonly IList<T> _list;

        public T this[int index]
        {
            get => _list[index];
            set => _list[index] = value;
        }

        public int Count => _list.Count;

        public SortingList()
        {
            _list = new List<T>();
        }

        protected SortingList(IList<T> list)
        {
            _list = list;
        }

        public static SortingList<T> FactoryWrapAssumeSorted(IList<T> list)
        {
            return new SortingList<T>(list);
        }

        public static SortingList<T> Factory_Wrap_Sort(List<T> list)
        {
            list.Sort();
            return new SortingList<T>(list);
        }

        //
        // Adds item to the sorting list at the index it sorts into.
        // If collisions occur, the given item will replace the existing.
        // 
        // Parameters:
        //    item: Item to put into the list
        public void Add(T item)
        {
            Add(item, replaceIfMatch: true);
        }

        /*
         * Adds item to the sorting list at the index it sorts into.
         * 
         * Parameters:
         *    item: Item to put into the list
         * 
         *    replaceIfMatch:
         *     Swaps the given item into the list if there is a collision.
         *     False will discard the given item if a match occurs, and leave
         *     the existing value
         * 
         * Returns: true if there was a match, whether it was replaced or not
         */
        public bool Add(T item, bool replaceIfMatch)
        {
            var search = _list.BinarySearch(item);
            if (search >= 0)
            {
                if (replaceIfMatch)
                {
                    _list[search] = item;
                }
                return true;
            }
            search = ~search;
            _list.Insert(search, item);
            return false;
        }

        public void Clear()
        {
            ((IList<T>)_list).Clear();
        }

        public bool Contains(T item)
        {
            return ListExt.BinarySearch(_list, item) >= 0;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            ((IList<T>)_list).CopyTo(array, arrayIndex);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return ((IList<T>)_list).GetEnumerator();
        }

        public int IndexOf(T item)
        {
            var search = ListExt.BinarySearch(_list, item);
            if (search < 0) return -1;
            return search;
        }

        public bool Remove(T item)
        {
            var search = _list.BinarySearch(item);
            if (search < 0) return false;
            _list.RemoveAt(search);
            return true;
        }

        public void RemoveAt(int index)
        {
            ((IList<T>)_list).RemoveAt(index);
        }

        #region Hidden Interface Functions
        void ICollection<T>.Add(T item)
        {
            this.Add(item);
        }

        bool ICollection<T>.IsReadOnly => false;

        public object SyncRoot => throw new NotImplementedException();

        public bool IsSynchronized => throw new NotImplementedException();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IList<T>)_list).GetEnumerator();
        }

        void IList<T>.Insert(int index, T item)
        {
            throw new NotImplementedException("Cannot insert at a specific index on a sorted list.");
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region PreSortedListExt
        public bool TryGetIndexInDirection(T item, bool higher, out int result)
        {
            return PreSortedListExt.TryGetIndexInDirection<T>(
                this,
                item,
                higher,
                out result);
        }

        public bool TryGetValueInDirection(T item, bool higher, [MaybeNullWhen(false)] out T result)
        {
            return PreSortedListExt.TryGetValueInDirection<T>(
                this,
                item,
                higher,
                out result);
        }

        public bool TryGetInDirection(T item, bool higher, out KeyValuePair<int, T> result)
        {
            return PreSortedListExt.TryGetInDirection<T>(
                this,
                item,
                higher,
                out result);
        }

        public bool TryGetEncapsulatedIndices(T lowerKey, T higherKey, out RangeInt32 result)
        {
            return PreSortedListExt.TryGetEncapsulatedIndices<T>(
                this,
                lowerKey,
                higherKey,
                out result);
        }

        public bool TryGetEncapsulatedValues(T lowerKey, T higherKey, [MaybeNullWhen(false)] out IEnumerable<KeyValuePair<int, T>> result)
        {
            return PreSortedListExt.TryGetEncapsulatedValues<T>(
                this,
                lowerKey,
                higherKey,
                out result);
        }
        #endregion
    }
}
