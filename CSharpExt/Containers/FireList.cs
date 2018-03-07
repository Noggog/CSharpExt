using System;
using System.Collections.Generic;
using System.Linq;

namespace Noggog.Notifying
{
    public class FireList<T> : IEnumerable<T>
    {
        List<T> _list = new List<T>();
        List<T> _fireList;

        public int Count => GetFireList().Count;

        public void Add(T t)
        {
            lock (this)
            {
                _list.Add(t);
                _fireList = null;
            }
        }

        public void Remove(T t)
        {
            lock (this)
            {
                _list.Remove(t);
                _fireList = null;
            }
        }

        public void Clear()
        {
            lock (this)
            {
                _list.Clear();
                _fireList = null;
            }
        }

        public List<T> GetFireList()
        {
            lock (this)
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
