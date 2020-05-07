using Noggog;
using System;

namespace Noggog
{
    public static class RangeIntExt
    {
        public static int Get(this RangeInt32 range, RandomSource rand)
        {
            if (range.Min == range.Max)
            {
                return range.Min;
            }
            else
            {
                return rand.Next(range.Min, range.Max + 1);
            }
        }

        public static int GetNormalDist(this RangeInt32 range, RandomSource rand)
        {
            if (range.Min == range.Max)
            {
                return range.Min;
            }
            else
            {
                return rand.NextNormalDist(range.Min, range.Max + 1);
            }
        }
    }
}
