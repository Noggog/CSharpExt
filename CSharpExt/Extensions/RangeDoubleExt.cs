using Noggog;
using System;

namespace System
{
    public static class RangeDoubleExt
    {
        public static double Get(this RangeDouble d, RandomSource rand)
        {
            return rand.NextDouble(d.Max - d.Min) + d.Min;
        }

        public static double GetNormalDist(this RangeDouble d, RandomSource rand)
        {
            return rand.NextNormalDist(d.Min, d.Max);
        }
    }
}
