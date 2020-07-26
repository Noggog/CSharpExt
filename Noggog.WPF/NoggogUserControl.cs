using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Text;

namespace Noggog.WPF
{
    public class NoggogUserControl<TViewModel> : ReactiveUserControl<TViewModel>
        where TViewModel : class
    {
        public NoggogUserControl()
        {
            this.DataContextChanged += (o, e) =>
            {
                if (e.NewValue is TViewModel vm)
                {
                    this.ViewModel = vm;
                }
                else
                {
                    this.ViewModel = null!;
                }
            };
        }
    }
}
