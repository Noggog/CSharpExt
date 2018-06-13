using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noggog
{
    public interface ISortedListGetter<T> : IReadOnlyList<T>
    {
        bool TryGetIndexInDirection(
            T item,
            bool higher,
            out int result);

        bool TryGetValueInDirection(
            T item,
            bool higher,
            out T result);

        bool TryGetInDirection(
            T item,
            bool higher,
            out KeyValuePair<int, T> result);

        bool TryGetEncapsulatedIndices(
            T lowerKey,
            T higherKey,
            out RangeInt32 result);

        bool TryGetEncapsulatedValues(
            T lowerKey,
            T higherKey,
            out IEnumerable<KeyValuePair<int, T>> result);
    }

    public interface ISortedList<T> : ISortedListGetter<T>, IList<T>
    {
    }
}
