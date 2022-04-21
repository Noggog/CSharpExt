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
}