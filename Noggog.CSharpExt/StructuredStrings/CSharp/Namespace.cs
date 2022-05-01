namespace Noggog.StructuredStrings.CSharp;

public class Namespace : IDisposable
{
    private readonly StructuredStringBuilder _sb;
    private readonly bool _doThings;
    private readonly bool _fileScoped;

    public Namespace(StructuredStringBuilder sb, string str, bool fileScoped = true)
    {
        _fileScoped = fileScoped;
        _sb = sb;
        _doThings = !string.IsNullOrWhiteSpace(str);
        if (_doThings)
        {
            sb.AppendLine($"namespace {str}{(_fileScoped ? ";" : null)}");
            if (_fileScoped)
            {
                _sb.AppendLine();
            }
            else
            {
                sb.AppendLine("{");
                sb.Depth++;
            }
        }
    }

    public void Dispose()
    {
        if (!_doThings) return;
        if (!_fileScoped)
        {
            _sb.Depth--;
            _sb.AppendLine("}");
        }
    }
}