using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Noggog
{
    public interface IReadOnlyCache<out TValue, TKey> : IEnumerable<IKeyValue<TValue, TKey>>, IEnumerable
    {
        //
        // Summary:
        //     Gets the number of elements in the collection.
        //
        // Returns:
        //     The number of elements in the collection.
        int Count { get; }

        //
        // Summary:
        //     Gets the element that has the specified key in the read-only dictionary.
        //
        // Parameters:
        //   key:
        //     The key to locate.
        //
        // Returns:
        //     The element that has the specified key in the read-only dictionary.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     key is null.
        //
        //   T:System.Collections.Generic.KeyNotFoundException:
        //     The property is retrieved and key is not found.
        TValue this[TKey key] { get; }

        //
        // Summary:
        //     Gets an enumerable collection that contains the keys in the read-only dictionary.
        //
        // Returns:
        //     An enumerable collection that contains the keys in the read-only dictionary.
        IEnumerable<TKey> Keys { get; }
        //
        // Summary:
        //     Gets an enumerable collection that contains the values in the read-only dictionary.
        //
        // Returns:
        //     An enumerable collection that contains the values in the read-only dictionary.
        IEnumerable<TValue> Items { get; }

        //
        // Summary:
        //     Determines whether the read-only dictionary contains an element that has the
        //     specified key.
        //
        // Parameters:
        //   key:
        //     The key to locate.
        //
        // Returns:
        //     true if the read-only dictionary contains an element that has the specified key;
        //     otherwise, false.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     key is null.
        bool ContainsKey(TKey key);
    }
}
