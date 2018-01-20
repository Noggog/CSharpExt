using Noggog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public static class SortedListExt
    {
        public static bool TryGetInDirectionIndex<K, V>(
            this SortedList<K, V> sortedList,
            K key,
            bool higher, 
            out int result)
        {
            var binSearch = sortedList.Keys.BinarySearch(key);
            if (binSearch >= 0)
            { // found
                result = binSearch;
                return true;
            }

            binSearch = ~binSearch;
            if (higher)
            {
                if (binSearch == sortedList.Count)
                {
                    result = -1;
                    return false;
                }
                result = binSearch;
                return true;
            }
            else
            {
                if (binSearch == 0)
                {
                    result = -1;
                    return false;
                }
                else
                {
                    result = binSearch - 1;
                    return true;
                }
            }
        }

        public static bool TryGetInDirection<K, V>(
            this SortedList<K, V> sortedList,
            K key,
            bool higher, 
            out KeyValuePair<int, V> result)
        {
            if (!sortedList.TryGetInDirectionIndex(
                key: key,
                higher: higher,
                result: out int index))
            {
                result = default(KeyValuePair<int, V>);
                return false;
            }
            result = new KeyValuePair<int, V>(
                index,
                sortedList.Values[index]);
            return true;
        }

        public static bool TryGetEncapsulatedIndices<K, V>(
            this SortedList<K, V> sortedList,
            K lowerKey,
            K higherKey,
            out RangeInt32 result)
        {
            var comp = Comparer<K>.Default;
            if (comp.Compare(lowerKey, higherKey) > 0)
            {
                result = default(RangeInt32);
                return false;
            }
            if (!sortedList.TryGetInDirectionIndex(
                key: lowerKey,
                higher: true,
                result: out var lowEnd))
            {
                result = default(RangeInt32);
                return false;
            }
            if (!sortedList.TryGetInDirectionIndex(
                key: higherKey,
                higher: false,
                result: out var highEnd))
            {
                result = default(RangeInt32);
                return false;
            }
            result = new RangeInt32(lowEnd, highEnd);
            return true;
        }

        public static void Add<K, V>(
            this SortedList<K, V> sortedList,
            IEnumerable<KeyValuePair<K, V>> vals)
        {
            foreach(var val in vals)
            {
                sortedList[val.Key] = val.Value;
            }
        }
    }
}
