using System.Runtime.CompilerServices;

namespace Noggog;
// https://devblogs.microsoft.com/pfxteam/asynclazyt/

public class AsyncLazy<T> : Lazy<Task<T>>
{
    public AsyncLazy(Func<T> valueFactory)
        : base(() => Task.Factory.StartNew(valueFactory))
    {
    }

    public AsyncLazy(Func<T> valueFactory, LazyThreadSafetyMode mode)
        : base(() => Task.Factory.StartNew(valueFactory), mode)
    {
    }

    public AsyncLazy(Func<Task<T>> taskFactory)
        : base(() => Task.Factory.StartNew(() => taskFactory()).Unwrap())
    {
    }

    public AsyncLazy(Func<Task<T>> taskFactory, LazyThreadSafetyMode mode)
        : base(() => Task.Factory.StartNew(() => taskFactory()).Unwrap(), mode)
    {
    }

    public TaskAwaiter<T> GetAwaiter() => Value.GetAwaiter();
}