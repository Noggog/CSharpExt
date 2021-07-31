using System;
using System.Collections.Generic;
using System.Linq;

namespace Noggog.Testing.TestClassData
{
    public static class MemberData
    {
        public static IEnumerable<object[]> TestPerItem(params object[] objs)
        {
            foreach (var obj in objs)
            {
                yield return new[] {obj};
            }
        }

        public static IEnumerable<object[]> AlternatingBools(int size)
        {
            return Enumerable.Range(0, (int)Math.Pow(2, size))
                .Select(i =>
                    Enumerable.Range(0, size)
                        .Select(b => ((i & (1 << b)) > 0))
                        .Select(x => (object)x)
                        .ToArray()
                ).ToArray();
        }
    }
}