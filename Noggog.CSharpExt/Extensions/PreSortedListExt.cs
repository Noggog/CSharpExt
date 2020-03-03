using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noggog
{
    public static class PreSortedListExt
    {
        public static bool HasInDirection<T>(
            IReadOnlyList<T> sortedList,
            T item,
            bool higher)
        {
            return TryGetIndexInDirection(
                sortedList,
                item,
                higher,
                out int result);
        }

        public static bool TryGetIndexInDirection<T>(
            IReadOnlyList<T> sortedList,
            T item,
            bool higher,
            out int result)
        {
            var searchResult = sortedList.BinarySearch(item);
            if (searchResult >= 0)
            { // found
                result = searchResult;
                return true;
            }

            searchResult = ~searchResult;
            if (higher)
            {
                if (searchResult == sortedList.Count)
                {
                    result = -1;
                    return false;
                }
                result = searchResult;
                return true;
            }
            else
            {
                if (searchResult == 0)
                {
                    result = -1;
                    return false;
                }
                else
                {
                    result = searchResult - 1;
                    return true;
                }
            }
        }

        public static int? TryGetIndexInDirection<T>(
            IReadOnlyList<T> sortedList,
            T item,
            bool higher)
        {
            if (TryGetIndexInDirection<T>(
                sortedList,
                item,
                higher,
                out var result))
            {
                return result;
            }
            else
            {
                return null;
            }
        }

        public static bool TryGetValueInDirection<T>(
            IReadOnlyList<T> sortedList,
            T item,
            bool higher,
            out T result)
        {
            if (!TryGetIndexInDirection(
                sortedList,
                item: item,
                higher: higher,
                result: out int index))
            {
                result = default(T);
                return false;
            }
            result = sortedList[index];
            return true;
        }

        public static bool TryGetInDirection<T>(
            IReadOnlyList<T> sortedList,
            T item,
            bool higher,
            out KeyValuePair<int, T> result)
        {
            if (!TryGetIndexInDirection(
                sortedList,
                item: item,
                higher: higher,
                result: out int index))
            {
                result = default(KeyValuePair<int, T>);
                return false;
            }
            result = new KeyValuePair<int, T>(
                index,
                sortedList[index]);
            return true;
        }

        public static bool TryGetEncapsulatedIndices<T>(
            IReadOnlyList<T> sortedList,
            T lowerKey,
            T higherKey,
            out RangeInt32 result)
        {
            var comp = Comparer<T>.Default;
            if (comp.Compare(lowerKey, higherKey) > 0)
            {
                result = default(RangeInt32);
                return false;
            }
            if (!TryGetIndexInDirection(
                sortedList,
                item: lowerKey,
                higher: true,
                result: out var lowEnd))
            {
                result = default(RangeInt32);
                return false;
            }
            if (!TryGetIndexInDirection(
                sortedList,
                item: higherKey,
                higher: false,
                result: out var highEnd))
            {
                result = default(RangeInt32);
                return false;
            }
            if (lowEnd > highEnd)
            {
                result = default(RangeInt32);
                return false;
            }
            result = new RangeInt32(lowEnd, highEnd);
            return true;
        }

        public static bool TryGetEncapsulatedValues<T>(
            IReadOnlyList<T> sortedList,
            T lowerKey,
            T higherKey,
            [MaybeNullWhen(false)] out IEnumerable<KeyValuePair<int, T>> result)
        {
            if (!TryGetEncapsulatedIndices(
                sortedList: sortedList,
                lowerKey: lowerKey,
                higherKey: higherKey,
                result: out var range))
            {
                result = default!;
                return false;
            }
            result = range.Select((i) => new KeyValuePair<int, T>(i, sortedList[i]));
            return true;
        }

        public static void Set<T>(
            IList<T> sortedList,
            IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                Set(sortedList, item);
            }
        }

        public static void Set<T>(
            IList<T> sortedList,
            T item)
        {
            var binSearch = sortedList.BinarySearch(item);
            if (binSearch >= 0)
            {
                sortedList[binSearch] = item;
            }
            else
            {
                binSearch = ~binSearch;
                sortedList.Insert(binSearch, item);
            }
        }
    }
}
