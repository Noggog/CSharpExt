using System;

namespace System
{
    public static class IntExt
    {
        public static int Clamp(this int a, int min, int max)
        {
            return Math.Min(Math.Max(a, min), max);
        }
    }
}
