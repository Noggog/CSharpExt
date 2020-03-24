using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noggog
{
    /// <summary>
    /// A readonly interface of ICollection<T>.
    /// ICollection<T> itself will not implement this, which makes some APIs a bit tricky.
    /// </summary>
    /// <typeparam name="T">Type of object the collection contains</typeparam>
    public interface ICollectionGetter<T> : IEnumerable<T>, IEnumerable
    {
        /// <summary>
        /// Gets the number of elements contained in the collection
        /// </summary>
        int Count { get; }
        
        /// <summary>
        /// Determines whether the collection contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the collection.</param>
        /// <returns>true if item is found in the collection; otherwise, false.</returns>
        bool Contains(T item);
        
        /// <summary>
        /// Copies the elements of the System.Collections.ICollection to an System.Array,
        /// starting at a particular System.Array index.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional System.Array that is the destination of the elements copied
        /// from System.Collections.ICollection. The System.Array must have zero-based indexing.
        /// </param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        /// <exception cref="ArgumentOutOfRangeException">arrayIndex is less than zero.</exception>
        /// <exception cref="ArgumentException">
        /// The number of elements in the source is greater than the available space from arrayIndex 
        /// to the end of the destination array.
        /// </exception>
        void CopyTo(T[] array, int arrayIndex);
    }
    
    /// <summary>
    /// A wrapper class around an ICollection to expose it as an ICollectionGetter
    /// ICollection itself does not implement the Getter interface, which makes this wrapper necessary.
    /// </summary>
    /// <typeparam name="T">Type of object the collection contains</typeparam>
    public class CollectionGetterWrapper<T> : ICollectionGetter<T>
    {
        private readonly ICollection<T> _source;

        /// <summary>
        /// Constructor that wraps an existing collection
        /// </summary>
        /// <param name="source">Collection to wrap</param>
        public CollectionGetterWrapper(ICollection<T> source)
        {
            this._source = source;
        }
        
        /// <inheritdoc />
        public int Count => _source.Count;
        
        /// <inheritdoc />
        public bool Contains(T item)
        {
            return _source.Contains(item);
        }

        /// <inheritdoc />
        public void CopyTo(T[] array, int arrayIndex)
        {
            _source.CopyTo(array, arrayIndex);
        }

        /// <inheritdoc />
        public IEnumerator<T> GetEnumerator()
        {
            return _source.GetEnumerator();
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _source.GetEnumerator();
        }
    }
}
