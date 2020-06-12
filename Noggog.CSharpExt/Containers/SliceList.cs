using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Noggog
{
    public class SliceList<T> : IExtendedList<MemorySlice<T>>, IReadOnlyList<ReadOnlyMemorySlice<T>>
    {
        private ExtendedList<MemorySlice<T>> _list = new ExtendedList<MemorySlice<T>>();

        public MemorySlice<T> this[int index] 
        { 
            get => _list[index]; 
            set => _list[index] = value;
        }

        MemorySlice<T> IReadOnlyList<MemorySlice<T>>.this[int index] => _list[index];

        ReadOnlyMemorySlice<T> IReadOnlyList<ReadOnlyMemorySlice<T>>.this[int index] => _list[index];

        public int Count => _list.Count;

        public bool IsReadOnly => ((ICollection<MemorySlice<T>>)_list).IsReadOnly;

        public void Add(MemorySlice<T> item)
        {
            ((ICollection<MemorySlice<T>>)_list).Add(item);
        }

        public void AddRange(IEnumerable<MemorySlice<T>> collection)
        {
            _list.AddRange(collection);
        }

        public void Clear()
        {
            ((ICollection<MemorySlice<T>>)_list).Clear();
        }

        public bool Contains(MemorySlice<T> item)
        {
            return ((ICollection<MemorySlice<T>>)_list).Contains(item);
        }

        public void CopyTo(MemorySlice<T>[] array, int arrayIndex)
        {
            ((ICollection<MemorySlice<T>>)_list).CopyTo(array, arrayIndex);
        }

        public IEnumerator<MemorySlice<T>> GetEnumerator()
        {
            return ((IEnumerable<MemorySlice<T>>)_list).GetEnumerator();
        }

        public int IndexOf(MemorySlice<T> item)
        {
            return ((IList<MemorySlice<T>>)_list).IndexOf(item);
        }

        public void Insert(int index, MemorySlice<T> item)
        {
            ((IList<MemorySlice<T>>)_list).Insert(index, item);
        }

        public void InsertRange(IEnumerable<MemorySlice<T>> collection, int index)
        {
            _list.InsertRange(collection, index);
        }

        public void Move(int original, int destination)
        {
            _list.Move(original, destination);
        }

        public bool Remove(MemorySlice<T> item)
        {
            return ((ICollection<MemorySlice<T>>)_list).Remove(item);
        }

        public void RemoveAt(int index)
        {
            ((IList<MemorySlice<T>>)_list).RemoveAt(index);
        }

        public void RemoveRange(int index, int count)
        {
            _list.RemoveRange(index, count);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_list).GetEnumerator();
        }

        IEnumerator<ReadOnlyMemorySlice<T>> IEnumerable<ReadOnlyMemorySlice<T>>.GetEnumerator()
        {
            foreach (var item in _list)
            {
                yield return item;
            }
        }
    }
}
