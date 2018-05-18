using System;
using System.Collections.Generic;
using System.Linq;

namespace Noggog
{
    /*
     * Threadsafe list that copies to a second internal dictionary when modified mid usage
     */
    public class FireList<T> : IEnumerable<T>
    {
        private readonly object _lock = new object();
        private readonly List<T> _list = new List<T>();
        private List<T> _fireList;

        public int Count => GetFireList().Count;

        public void Add(T t)
        {
            lock (_lock)
            {
                _list.Add(t);
                _fireList = null;
            }
        }

        public void Remove(T t)
        {
            lock (_lock)
            {
                _list.Remove(t);
                _fireList = null;
            }
        }

        public void Clear()
        {
            lock (_lock)
            {
                _list.Clear();
                _fireList = null;
            }
        }

        public List<T> GetFireList()
        {
            lock (_lock)
            {
                if (_fireList == null)
                {
                    _fireList = _list.ToList();
                }
                return _fireList;
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (var item in GetFireList())
            {
                yield return item;
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
