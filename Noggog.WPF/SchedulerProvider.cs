using System.Reactive.Concurrency;
using Noggog.Reactive;
using ReactiveUI;

namespace Noggog.WPF
{
    public class SchedulerProvider : ISchedulerProvider
    {
        public IScheduler MainThread => RxApp.MainThreadScheduler;
        public IScheduler TaskPool  => RxApp.TaskpoolScheduler;
    }
}