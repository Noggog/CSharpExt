using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noggog
{
    public interface ICollectionGetter<T> : IEnumerable<T>, IEnumerable
    {
        int Count { get; }
        bool Contains(T item);
        void CopyTo(T[] array, int arrayIndex);
    }

    public struct CollectionGetterWrapper<T> : ICollectionGetter<T>
    {
        private readonly ICollection<T> _source;

        public CollectionGetterWrapper(ICollection<T> source)
        {
            this._source = source;
        }

        public int Count => _source.Count;
        
        public bool Contains(T item)
        {
            return _source.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _source.CopyTo(array, arrayIndex);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _source.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _source.GetEnumerator();
        }
    }
}
