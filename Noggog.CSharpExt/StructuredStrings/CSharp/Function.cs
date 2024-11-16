namespace Noggog.StructuredStrings.CSharp;

public class Function : IDisposable
{
    private readonly StructuredStringBuilder _sb;
    private readonly List<string[]> _args = new();
    private readonly string _initialLine;
    
    public bool SemiColon = false;
    public List<string> Wheres = new();
    
    private bool SemiColonAfterParenthesis => SemiColon && Wheres.Count == 0;

    public Function(
        StructuredStringBuilder sb,
        string initialLine)
    {
        _sb = sb;
        _initialLine = initialLine;
    }

    public void AddPassArg(string str)
    {
        Add($"{str}: {str}");
    }

    public void Add(params string[] lines)
    {
        foreach (var line in lines)
        {
            _args.Add(new string[] { line });
        }
    }

    public void Add(string line)
    {
        _args.Add(new string[] { line });
    }

    public void Add(Action<StructuredStringBuilder> generator)
    {
        var gen = new StructuredStringBuilder();
        generator(gen);
        if (gen.Empty) return;
        _args.Add(gen.ToArray());
    }

    public void Dispose()
    {
        if (_args.Count <= 1)
        {
            if (_args.Count == 0)
            {
                _sb.AppendLine($"{_initialLine}(){(SemiColonAfterParenthesis ? ";" : null)}");
            }
            else if (_args[0].Length == 1)
            {
                _sb.AppendLine($"{_initialLine}({_args[0][0]}){(SemiColonAfterParenthesis ? ";" : null)}");
            }
            else
            {
                _sb.AppendLine($"{_initialLine}({(_args.Count == 1 ? _args[0][0] : null)}");
                for (int i = 1; i < _args[0].Length - 1; i++)
                {
                    _sb.AppendLine(_args[0][i]);
                }
                _sb.AppendLine($"{_args[0][_args[0].Length - 1]}){(SemiColonAfterParenthesis ? ";" : null)}");
            }
            _sb.Depth++;
            foreach (var where in Wheres.WhereNotNull().IterateMarkLast())
            {
                _sb.AppendLine($"{where.Item}{(SemiColon && where.Last ? ";" : null)}");
            }
            _sb.Depth--;
            return;
        }

        _sb.AppendLine($"{_initialLine}(");
        _sb.Depth++;
        if (_args.Count != 0)
        {
            _args.Last(
                each: (arg) =>
                {
                    arg.Last(
                        each: (item, last) =>
                        {
                            _sb.AppendLine($"{item}{(last ? "," : string.Empty)}");
                        });
                },
                last: (arg) =>
                {
                    arg.Last(
                        each: (item, last) =>
                        {
                            _sb.AppendLine($"{item}{(last ? $"){(SemiColonAfterParenthesis ? ";" : string.Empty)}" : string.Empty)}");
                        });
                });
        }
        foreach (var where in Wheres.IterateMarkLast())
        {
            _sb.AppendLine($"{where.Item}{(SemiColon && where.Last ? ";" : null)}");
        }
        _sb.Depth--;
    }
}