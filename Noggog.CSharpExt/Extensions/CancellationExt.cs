namespace Noggog;

public static class CancellationExt
{
    public static CancellationToken Combine(this CancellationToken token, CancellationToken other)
    {
        return CancellationTokenSource.CreateLinkedTokenSource(token, other).Token;
    }

    public static CancellationToken Combine(this CancellationToken token, params CancellationToken[] other)
    {
        return CancellationTokenSource.CreateLinkedTokenSource(token.AsEnumerable().And(other).ToArray()).Token;
    }
    
    public static Task WhenCanceled(this CancellationToken cancellationToken)
    {
        var tcs = new TaskCompletionSource<bool>();
        cancellationToken.Register(s => s.SetResult(true), tcs);
        return tcs.Task;
    }
    
    public static CancellationTokenRegistration Register<T>(this CancellationToken cancellationToken, Action<T> toDo, T obj)
    {
        return cancellationToken.Register(o => toDo((T)o!), obj);
    }
}