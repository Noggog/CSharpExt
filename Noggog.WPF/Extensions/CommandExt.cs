using ReactiveUI;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Input;

namespace Noggog.WPF;

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

    public static ReactiveCommand<Unit, Unit> CreateCombinedAny(params ReactiveCommand<Unit, Unit>[] commands)
    {

        return ReactiveCommand.CreateFromTask(
            execute: () =>
            {
                return Task.WhenAll(commands.Select(async c =>
                {
                    if (((ICommand)c).CanExecute(Unit.Default))
                    {
                        await c.Execute();
                    }
                }));
            },
            canExecute: Noggog.ObservableExt.Any(commands.Select(x => x.CanExecute).ToArray()));
    }
}