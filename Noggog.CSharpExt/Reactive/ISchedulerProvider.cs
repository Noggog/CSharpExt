using System.Reactive.Concurrency;

namespace Noggog.Reactive;

public interface ISchedulerProvider
{
    public IScheduler MainThread { get; }
    public IScheduler TaskPool { get; }
}