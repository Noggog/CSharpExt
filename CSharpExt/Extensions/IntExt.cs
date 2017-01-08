using System;

namespace System
{
    public static class IntExt
    {
        public static int? TryParse(string str)
        {
            int i;
            if (int.TryParse(str, out i))
            {
                return i;
            }
            else
            {
                return null;
            }
        }

        public static int TryGetValue(this int? val, int backup)
        {
            return val ?? backup;
        }

        public static int TryGetValue(this int? val, Func<int> backup)
        {
            return val ?? backup();
        }

        public static int Clamp(this int a, int min, int max)
        {
            return Math.Min(Math.Max(a, min), max);
        }
    }
}
