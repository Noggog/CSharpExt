using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Noggog.WPF
{
    public class NoggogControl : Control
    {
        protected readonly CompositeDisposable _unloadDisposable = new CompositeDisposable();
        protected readonly CompositeDisposable _templateDisposable = new CompositeDisposable();

        public NoggogControl()
        {
            this.Unloaded += (_, _) =>
            {
                _templateDisposable.Dispose();
                _unloadDisposable.Dispose();
            };
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _templateDisposable.Clear();
        }
    }
}
