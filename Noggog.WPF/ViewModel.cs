using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive.Disposables;

namespace Noggog.WPF
{
    public class ViewModel : ReactiveObject, IDisposable
    {
        private readonly Lazy<CompositeDisposable> _CompositeDisposable = new Lazy<CompositeDisposable>();
        public CompositeDisposable CompositeDisposable => _CompositeDisposable.Value;

        public virtual void Dispose()
        {
            if (_CompositeDisposable.IsValueCreated)
            {
                _CompositeDisposable.Value.Dispose();
            }
        }
    }
}
