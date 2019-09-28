using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Noggog
{
    public class SetList<T> : ISetList<T>
    {
        private readonly List<T> _list;

        public int Count => _list.Count;
        public bool HasBeenSet { get; set; }
        public bool IsReadOnly => ((IList<T>)_list).IsReadOnly;
        public T this[int index]
        {
            get => _list[index];
            set => _list[index] = value;
        }

        public SetList()
        {
            this._list = new List<T>();
        }

        public SetList(IEnumerable<T> collection)
        {
            this._list = new List<T>(collection);
        }

        public SetList(int capacity)
        {
            this._list = new List<T>(capacity);
        }

        public void Add(T item)
        {
            this.HasBeenSet = true;
            _list.Add(item);
        }

        public void Clear()
        {
            this.HasBeenSet = true;
            _list.Clear();
        }

        public bool Contains(T item)
        {
            return _list.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return ((IList<T>)_list).GetEnumerator();
        }

        public int IndexOf(T item)
        {
            return _list.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            this.HasBeenSet = true;
            _list.Insert(index, item);
        }

        public bool Remove(T item)
        {
            this.HasBeenSet = true;
            return _list.Remove(item);
        }

        public void RemoveAt(int index)
        {
            this.HasBeenSet = true;
            _list.RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IList<T>)_list).GetEnumerator();
        }

        public void Unset()
        {
            this.HasBeenSet = false;
            _list.Clear();
        }
    }
}
