using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noggog
{
    public static class IDisposableExt
    {
        public static readonly IDisposable Nothing = new IDisposableNothing();

        class IDisposableNothing : IDisposable
        {
            public void Dispose()
            {
            }
        }
    }
}
