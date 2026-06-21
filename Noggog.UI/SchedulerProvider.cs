using System.Reactive.Concurrency;
using Noggog.Reactive;
using ReactiveUI;

namespace Noggog.UI;

public class SchedulerProvider : ISchedulerProvider
{
    public IScheduler MainThread => RxSchedulers.MainThreadScheduler;
    public IScheduler TaskPool  => RxSchedulers.TaskpoolScheduler;
}