namespace Noggog.StructuredStrings;

public class Brace : IDisposable
{
    private readonly StructuredStringBuilder _sb;
    private readonly bool _doIt;
    private string _start;
    private string _end;

    public Brace(StructuredStringBuilder sb, string start = "[", string end = "]", bool doIt = true)
    {
        _sb = sb;
        _doIt = doIt;
        _start = start;
        _end = end;
        if (doIt)
        {
            sb.AppendLine(_start);
            sb.Depth++;
        }
    }

    public void Dispose()
    {
        if (_doIt)
        {
            _sb.Depth--;
            _sb.AppendLine(_end);
        }
    }
}
