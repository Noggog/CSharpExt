namespace Noggog.StructuredStrings;

public class Line : IDisposable
{
    readonly StructuredStringBuilder _sb;

    public Line(StructuredStringBuilder sb)
    {
        _sb = sb;
        _sb.Append(_sb.DepthStr);
    }

    public void Dispose()
    {
        _sb.Append(Environment.NewLine);
    }
}