namespace Noggog.StructuredStrings.CSharp;

public class If : IDisposable
{
    private readonly StructuredStringBuilder _sb;
    private bool _first;
    private readonly bool _ands;
    
    public List<(string str, bool wrap)> Checks = new();
    public Action<StructuredStringBuilder>? Body;
    public bool Empty => Checks.Count == 0;

    public If(StructuredStringBuilder sb, bool ANDs, bool first = true)
    {
        _ands = ANDs;
        _first = first;
        _sb = sb;
    }

    public void Add(string str, bool wrapInParens = false)
    {
        Checks.Add((str, wrapInParens));
    }

    private string Get(int index)
    {
        var item = Checks[index];
        if (!item.wrap || Checks.Count <= 1)
        {
            return item.str;
        }
        return $"({item.str})";
    }

    private void GenerateIf()
    {
        using (_sb.Line())
        {
            if (!_first)
            {
                _sb.Append("else ");
            }
            _sb.Append("if (");
            _sb.Append(Get(0));
            if (Checks.Count == 1)
            {
                _sb.Append(")");
                return;
            }
        }
        using (_sb.IncreaseDepth())
        {
            for (int i = 1; i < Checks.Count; i++)
            {
                using (_sb.Line())
                {
                    if (_ands)
                    {
                        _sb.Append("&& ");
                    }
                    else
                    {
                        _sb.Append("|| ");
                    }
                    _sb.Append(Get(i));
                    if (i == Checks.Count - 1)
                    {
                        _sb.Append(")");
                    }
                }
            }
        }
    }

    public void Dispose()
    {
        if (Checks.Count == 0)
        {
            Body?.Invoke(_sb);
            return;
        }
        GenerateIf();
        if (Body != null)
        {
            using (_sb.CurlyBrace())
            {
                Body(_sb);
            }
        }
    }
}