namespace Noggog.StructuredStrings.CSharp;

public class Call : IDisposable
{
    private readonly StructuredStringBuilder _sb;
    private readonly List<string[]> _args = new();
    private readonly string? _initialLine;
    private readonly string? _suffixLine;
    
    public bool SemiColon = true;

    public Call(
        StructuredStringBuilder sb,
        string? initialLine = null,
        string? suffixLine = null,
        bool semiColon = true)
    {
        _sb = sb;
        SemiColon = semiColon;
        _initialLine = initialLine;
        _suffixLine = suffixLine;
    }

    public void Add(params string[] lines)
    {
        foreach (var line in lines)
        {
            _args.Add(new string[] { line });
        }
    }

    public void AddPassArg(string str)
    {
        Add($"{str}: {str}");
    }

    public void Add(Action<StructuredStringBuilder> generator, bool removeSemicolon = true)
    {
        var gen = new StructuredStringBuilder();
        generator(gen);
        if (gen.Empty) return;
        if (removeSemicolon && gen.Count != 0)
        {
            gen[gen.Count - 1] = gen[gen.Count - 1].TrimEnd(';'); 
        }
        _args.Add(gen.ToArray());
    }

    public async Task Add(Func<StructuredStringBuilder, Task> generator)
    {
        var gen = new StructuredStringBuilder();
        await generator(gen);
        if (gen.Empty) return;
        _args.Add(gen.ToArray());
    }

    public void Dispose()
    {
        if (_initialLine != null)
        {
            if (_args.Count == 0)
            {
                _sb.AppendLine($"{_initialLine}(){_suffixLine}{(SemiColon ? ";" : string.Empty)}");
                return;
            }
            else if (_args.Count == 1
                     && _args[0].Length == 1)
            {
                _sb.AppendLine($"{_initialLine}({_args[0][0]}){_suffixLine}{(SemiColon ? ";" : string.Empty)}");
                return;
            }
            else
            {
                _sb.AppendLine($"{_initialLine}(");
            }
        }
        _sb.Depth++;
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
                        _sb.AppendLine($"{item}{(last ? $"){_suffixLine}{(SemiColon ? ";" : string.Empty)}" : string.Empty)}");
                    });
            });
        _sb.Depth--;
    }
}
