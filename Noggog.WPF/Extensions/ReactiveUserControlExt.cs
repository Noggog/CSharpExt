using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Text;

namespace Noggog.WPF
{
    public static class ReactiveUserControlExt
    {
        public static void WhenActivated<TViewModel>(this ReactiveUserControl<TViewModel> control, Action<TViewModel, CompositeDisposable> block)
            where TViewModel : class
        {
            control.WhenActivated((disp) =>
            {
                var vm = control.ViewModel;
                if (vm == null) return;
                block(vm, disp);
            });
        }
    }
}
