using System.Reactive.Disposables;

namespace Noggog.Utility;

public class AsyncLock
{
    private readonly SemaphoreSlim _lock = new(1, 1);

    public async Task<IDisposable> WaitAsync()
    {
        await _lock.WaitAsync();
        return Disposable.Create(_lock, l => l.Release());
    }
}