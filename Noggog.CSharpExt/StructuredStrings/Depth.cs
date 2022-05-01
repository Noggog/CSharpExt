namespace Noggog.StructuredStrings;

public class Depth : IDisposable
{
    private readonly StructuredStringBuilder _sb;
    private readonly bool _doIt;

    public Depth(
        StructuredStringBuilder sb,
        bool doIt = true)
    {
        _sb = sb;
        _doIt = doIt;
        if (doIt)
        {
            _sb.Depth++;
        }
    }

    public void Dispose()
    {
        if (_doIt)
        {
            _sb.Depth--;
        }
    }
}
