using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noggog
{
    public interface ISortingListDictionaryGetter<TKey, TValue> :
        IEnumerable,
        IReadOnlyDictionary<TKey, TValue>,
        IReadOnlyCollection<KeyValuePair<TKey, TValue>>
    {
        bool TryGetIndexInDirection(
            TKey item,
            bool higher,
            out int result);

        bool TryGetValueInDirection(
            TKey item,
            bool higher,
            out TValue result);

        bool TryGetInDirection(
            TKey item,
            bool higher,
            out KeyValuePair<int, TValue> result);

        bool TryGetEncapsulatedIndices(
            TKey lowerKey,
            TKey higherKey,
            out RangeInt32 result);

        bool TryGetEncapsulatedValues(
            TKey lowerKey,
            TKey higherKey,
            [MaybeNullWhen(false)] out IEnumerable<KeyValuePair<int, TValue>> result);
    }

    public interface ISortingListDictionary<TKey, TValue> : 
        ISortingListDictionaryGetter<TKey, TValue>,
        IDictionary<TKey, TValue>,
        ICollection<KeyValuePair<TKey, TValue>>,
        IEnumerable<KeyValuePair<TKey, TValue>>,
        IEnumerable,
        IDictionary,
        ICollection,
        IReadOnlyDictionary<TKey, TValue>,
        IReadOnlyCollection<KeyValuePair<TKey, TValue>>
    {
    }
}
