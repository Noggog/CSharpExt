namespace Noggog.StructuredStrings;

public class Comment : IDisposable
{
    private readonly StructuredStringBuilder _sb;
    public readonly StructuredStringBuilder Summary = new();
    public readonly Dictionary<string, StructuredStringBuilder> Parameters = new();
    public readonly StructuredStringBuilder Return = new();

    public Comment(StructuredStringBuilder sb)
    {
        _sb = sb;
    }

    public void AddParameter(string name, string comment)
    {
        var sb = new StructuredStringBuilder();
        sb.AppendLine(comment);
        Parameters[name] = sb;
    }

    public void Apply(StructuredStringBuilder sb)
    {
        if (Summary.Count > 0)
        {
            sb.AppendLine("/// <summary>");
            foreach (var line in Summary)
            {
                sb.AppendLine($"/// {line}");
            }
            sb.AppendLine("/// </summary>");
        }
        foreach (var param in Parameters)
        {
            if (param.Value.Count > 1)
            {
                sb.AppendLine($"/// <param name=\"{param.Key}\">");
                foreach (var line in param.Value)
                {
                    sb.AppendLine($"/// {line}");
                }
                sb.AppendLine("/// </param>");
            }
            else
            {
                sb.AppendLine($"/// <param name=\"{param.Key}\">{param.Value[0]}</param>");
            }
        }
        if (Return.Count == 1)
        {
            sb.AppendLine($"/// <returns>{Return[0]}</returns>");
        }
        else if (Return.Count > 0)
        {
            sb.AppendLine("/// <returns>");
            foreach (var line in Return)
            {
                sb.AppendLine($"/// {line}");
            }
            sb.AppendLine("/// </returns>");
        }
    }

    public void Dispose()
    {
        Apply(_sb);
    }
}