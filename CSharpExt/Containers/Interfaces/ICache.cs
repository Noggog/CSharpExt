using System;
using System.Collections.Generic;
using System.Text;

namespace Noggog
{
    public interface ICache<TObject, TKey> : IReadOnlyCache<TObject, TKey>
    {
        /// <inheritdoc />
        new IEnumerable<TKey> Keys { get; }

        /// <inheritdoc />
        new int Count { get; }

        /// <summary>
        /// Adds or updates the item using the specified key
        /// </summary>
        /// <param name="item">The item.</param>
        void Set(TObject item);

        /// <summary>
        /// Adds or updates the item using the specified key
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

        TObject TryCreateValue(TKey key, Func<TKey, TObject> createFunc);

        new IEnumerator<IKeyValue<TObject, TKey>> GetEnumerator();
    }
}
