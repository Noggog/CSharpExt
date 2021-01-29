using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;

namespace Noggog
{
    public static class CompositeDisposableExt
    {
        public static void Add(this CompositeDisposable dispose, IEnumerable<IDisposable> disposables)
        {
            foreach (var disp in disposables)
            {
                dispose.Add(disp);
            }
        }
    }
}
