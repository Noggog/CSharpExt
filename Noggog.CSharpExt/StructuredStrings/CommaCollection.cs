namespace Noggog.StructuredStrings;

public class CommaCollection : IDisposable
{
    private readonly StructuredStringBuilder _sb;
    private readonly string _delimiter;
    private readonly List<string> _items = new();

    public CommaCollection(StructuredStringBuilder sb, string delimiter = ",")
    {
        _sb = sb;
        _delimiter = delimiter;
    }

    public void Add(string item)
    {
        _items.Add(item);
    }

    public void Add(params string[] items)
    {
        _items.AddRange(items);
    }

    public void Add(Action<StructuredStringBuilder> generator)
    {
        var gen = new StructuredStringBuilder();
        generator(gen);
        if (gen.Empty) return;
        Add(gen.ToArray());
    }

    public void Dispose()
    {
        foreach (var item in _items.IterateMarkLast())
        {
            if (item.Last)
            {
                _sb.AppendLine(item.Item);
            }
            else
            {
                using (_sb.Line())
                {
                    _sb.Append(item.Item);
                    _sb.Append(_delimiter);
                }
            }
        }
    }
}