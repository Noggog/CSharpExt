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
            this.Loaded += (_, _) => OnLoaded();
            this.Loaded += (_, _) =>
            {
                if (Template != null)
                {
                    OnApplyTemplate();
                }
            };
            this.Unloaded += (_, _) =>
            {
                _templateDisposable.Clear();
                _unloadDisposable.Clear();
            };
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _templateDisposable.Clear();
        }

        protected virtual void OnLoaded()
        {
        }
    }
}
