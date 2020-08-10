using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;

namespace Noggog.WPF
{
    public static class CommandExt
    {
        public static IObservable<Unit> StartingExecution(this IReactiveCommand cmd)
        {
            return cmd.IsExecuting
                .DistinctUntilChanged()
                .Where(x => x)
                .Unit();
        }

        public static IObservable<Unit> EndingExecution(this IReactiveCommand cmd)
        {
            return cmd.IsExecuting
                .DistinctUntilChanged()
                .Pairwise()
                .Where(x => x.Previous && !x.Current)
                .Unit();
        }
    }
}
