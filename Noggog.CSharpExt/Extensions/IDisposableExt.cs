using System.Reactive.Disposables;

namespace Noggog;

public static class IDisposableExt
{
    public static T DisposeWith<T>(this T disposable, IDisposableDropoff compositeDisposable)
        where T : IDisposable
    {
        compositeDisposable.Add(disposable);
        return disposable;
    }

    public static T DisposeWithComposite<T>(this T disposable, CompositeDisposable compositeDisposable)
        where T : IDisposable
    {
        compositeDisposable.Add(disposable);
        return disposable;
    }
}