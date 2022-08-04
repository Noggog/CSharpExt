#if NETSTANDARD2_0 
#else
using System.Reactive.Linq;

namespace Noggog.WorkEngine;

public interface INumWorkThreadsController
{
    IObservable<int?> NumDesiredThreads { get; }
}

public class NumWorkThreadsUnopinionated : INumWorkThreadsController
{
    public IObservable<int?> NumDesiredThreads => Observable.Return(default(int?));
}
#endif