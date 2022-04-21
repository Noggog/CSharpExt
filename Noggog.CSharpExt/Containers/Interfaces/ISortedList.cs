using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Noggog;

public interface ISortedListGetter<T> : IReadOnlyList<T>
{
    bool TryGetIndexInDirection(
        T item,
        bool higher,
        out int result);

    bool TryGetValueInDirection(
        T item,
        bool higher,
        [MaybeNullWhen(false)] out T result);

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
        [MaybeNullWhen(false)] out IEnumerable<KeyValuePair<int, T>> result);
}

public interface ISortedList<T> : ISortedListGetter<T>, IList<T>, ICollection
{
    bool Add(T item, bool replaceIfMatch);
}