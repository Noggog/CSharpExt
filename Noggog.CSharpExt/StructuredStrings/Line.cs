namespace Noggog.StructuredStrings;

public class Line : IDisposable
{
    readonly StructuredStringBuilder _sb;
    private readonly bool _appendNewLine;

    public Line(StructuredStringBuilder sb, bool appendNewLine = true)
    {
        _sb = sb;
        _appendNewLine = appendNewLine;
        _sb.Append(_sb.DepthStr);
    }

    public void Dispose()
    {
        if (_appendNewLine)
        {
            _sb.Append(Environment.NewLine);
        }
    }
}