using System;
using System.Collections.Generic;
using System.Text;

namespace Noggog
{
    public interface ICache<TObject, TKey> : IReadOnlyCache<TObject, TKey>
    {
        /// <summary>
        /// Adds or updates the cache to contain item using the specified key
        /// </summary>
        /// <param name="item">The item.</param>
        /// <exception cref="ArgumentException">If an item with the given key already exists within the cache</exception>
        void Add(TObject item);

        /// <summary>
        /// Adds or updates the cache to contain item, based on its implicit key
        /// </summary>
        /// <param name="item">The item.</param>
        void Set(TObject item);

        /// <summary>
        /// Adds or updates the cache to contain item, based on its implicit key
        /// </summary>
        /// <param name="items">The items.</param>
        void Set(IEnumerable<TObject> items);

        /// <summary>
        /// Removes the item matching the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        bool Remove(TKey key);

        /// <summary>
        /// Removes the specified key retrieved from the given item.
        /// </summary>
        /// <param name="obj">An object to retrieve a key from to remove</param>
        bool Remove(TObject obj);

        /// <summary>
        /// Removes all items matching the specified keys
        /// </summary>
        /// <param name="objects">The items.</param>
        void Remove(IEnumerable<TObject> objects);

        /// <summary>
        /// Removes all items matching the specified keys
        /// </summary>
        /// <param name="keys">The keys.</param>
        void Remove(IEnumerable<TKey> keys);

        /// <summary>
        /// Clears all items
        /// </summary>
        void Clear();

        new bool ContainsKey(TKey key);

        TObject GetOrAdd(TKey key, Func<TKey, TObject> createFunc);
    }
}
