using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noggog
{
    public static class MathExt
    {
        public static int Min(IEnumerable<int> e)
        {
            int? rhs = null;
            foreach (var i in e)
            {
                rhs = Math.Min(i, rhs ?? int.MaxValue);
            }
            if (rhs == null)
            {
                throw new ArgumentException("Enumerable contained no items.");
            }
            return rhs.Value;
        }

        public static int Min(params int[] e)
        {
            return Min((IEnumerable<int>)e);
        }

        public static int Max(IEnumerable<int> e)
        {
            int? rhs = null;
            foreach (var i in e)
            {
                rhs = Math.Max(i, rhs ?? int.MinValue);
            }
            if (rhs == null)
            {
                throw new ArgumentException("Enumerable contained no items.");
            }
            return rhs.Value;
        }

        public static int Max(params int[] e)
        {
            return Max((IEnumerable<int>)e);
        }
    }
}
