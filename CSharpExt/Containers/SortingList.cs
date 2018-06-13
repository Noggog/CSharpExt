using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noggog
{
    public class SortingList<T> : ISortedList<T>
    {
        private readonly IList<T> _list;

        public T this[int index] { get => ((IList<T>)_list)[index]; set => ((IList<T>)_list)[index] = value; }

        public int Count => ((IList<T>)_list).Count;

        public bool IsReadOnly => ((IList<T>)_list).IsReadOnly;

        public SortingList()
        {
            _list = new List<T>();
        }

        protected SortingList(IList<T> list)
        {
            _list = list;
        }

        public static SortingList<T> Factory_Wrap_AssumeSorted(IList<T> list)
        {
            return new SortingList<T>(list);
        }

        public static SortingList<T> Factory_Wrap_Sort(List<T> list)
        {
            list.Sort();
            return new SortingList<T>(list);
        }

        public void Set(T item)
        {
            _list.BinarySearch(item);
            ((IList<T>)_list).Add(item);
        }

        public void Clear()
        {
            ((IList<T>)_list).Clear();
        }

        public bool Contains(T item)
        {
            return ((IList<T>)_list).Contains(item);
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
            return ((IList<T>)_list).IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            ((IList<T>)_list).Insert(index, item);
        }

        public bool Remove(T item)
        {
            return ((IList<T>)_list).Remove(item);
        }

        public void RemoveAt(int index)
        {
            ((IList<T>)_list).RemoveAt(index);
        }

        #region Hidden Interface Functions
        void ICollection<T>.Add(T item)
        {
            _list.BinarySearch(item);
            ((IList<T>)_list).Add(item);
        }
        #endregion

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IList<T>)_list).GetEnumerator();
        }

        #region PreSortedListExt
        public bool TryGetIndexInDirection(T item, bool higher, out int result)
        {
            return PreSortedListExt.TryGetIndexInDirection<T>(
                this,
                item,
                higher,
                out result);
        }

        public bool TryGetValueInDirection(T item, bool higher, out T result)
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

        public bool TryGetEncapsulatedValues(T lowerKey, T higherKey, out IEnumerable<KeyValuePair<int, T>> result)
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
