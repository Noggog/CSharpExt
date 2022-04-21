using System.Reactive.Disposables;

namespace Noggog;

public static class CompositeDisposableExt
{
    public static void Add(this CompositeDisposable dispose, IEnumerable<IDisposable> disposables)
    {
        foreach (var disp in disposables)
        {
            dispose.Add(disp);
        }
    }
}