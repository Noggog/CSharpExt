namespace Noggog.StructuredStrings.CSharp;

public class CurlyBrace : IDisposable
{
    private readonly StructuredStringBuilder _sb;
    private readonly bool _doIt;

    public bool AppendParenthesis;
    public bool AppendSemicolon;
    public bool AppendComma;

    public CurlyBrace(StructuredStringBuilder sb, bool doIt = true)
    {
        _sb = sb;
        _doIt = doIt;
        if (doIt)
        {
            sb.AppendLine("{");
            sb.Depth++;
        }
    }

    public void Dispose()
    {
        if (_doIt)
        {
            _sb.Depth--;
            _sb.AppendLine("}"
                           + (AppendParenthesis ? ")" : string.Empty)
                           + (AppendSemicolon ? ";" : string.Empty)
                           + (AppendComma ? "," : string.Empty));
        }
    }
}
