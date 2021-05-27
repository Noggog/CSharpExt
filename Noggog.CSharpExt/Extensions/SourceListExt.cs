using System.Collections;
using System.Collections.Generic;
using DynamicData;

namespace Noggog
{
    public static class SourceListExt
    {
        public static void SetTo<T>(this ISourceList<T> list, IEnumerable<T> items, bool checkEquality = false)
        {
            list.Edit(l =>
            {
                int i = 0;
                foreach (var item in items)
                {
                    if (i >= l.Count)
                    {
                        l.Add(item);
                    }
                    else if (checkEquality)
                    {
                        if (!EqualityComparer<T>.Default.Equals(l[i], item))
                        {
                            l[i] = item;
                        }
                    }
                    else
                    {
                        l[i] = item;
                    }
                    i++;
                }

                l.RemoveToCount(i);
            });
        }

        public static void RemoveToCount<T>(this ISourceList<T> list, int count)
        {
            list.Edit(l =>
            {
                var toRemove = l.Count - count;
                for (; toRemove > 0; toRemove--)
                {
                    l.RemoveAt(l.Count - 1);
                }
            });
        }
    }
}