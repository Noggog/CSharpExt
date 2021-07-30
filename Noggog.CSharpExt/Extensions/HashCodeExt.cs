using System;
using System.Collections.Generic;

namespace Noggog
{
    public static class HashCodeExt
    {
        public static void AddEnumerable<TValue>(this HashCode hashCode, IEnumerable<TValue> col)
        {
            foreach (var val in col)
            {
                hashCode.Add(val);
            }
        }
    }
}