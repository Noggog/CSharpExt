namespace Noggog.StructuredStrings.CSharp;

public class PropertyCtor : IDisposable
{
    private readonly StructuredStringBuilder _sb;
    private readonly List<string[]> _args = new();

    public PropertyCtor(StructuredStringBuilder sb)
    {
        _sb = sb;
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
        if (removeSemicolon)
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
        using (_sb.CurlyBrace(appendSemiColon: true))
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
                            _sb.AppendLine($"{item}");
                        });
                });
        }
    }
}