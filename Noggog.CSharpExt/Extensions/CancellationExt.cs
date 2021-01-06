using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Noggog
{
    public static class CancellationExt
    {
        public static CancellationToken Combine(this CancellationToken token, CancellationToken other)
        {
            return CancellationTokenSource.CreateLinkedTokenSource(token, other).Token;
        }

        public static CancellationToken Combine(this CancellationToken token, params CancellationToken[] other)
        {
            return CancellationTokenSource.CreateLinkedTokenSource(token.AsEnumerable().And(other).ToArray()).Token;
        }
    }
}
