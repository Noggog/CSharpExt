using System.Collections;
using System.Diagnostics.CodeAnalysis;
#if NETSTANDARD2_0
#else
using System.Reactive.Subjects;
#endif
using System.Text;

namespace Noggog.StructuredStrings;

public class StructuredStringBuilder : IEnumerable<string>
{
    public int Depth;
    // The line being written is kept apart from the finished ones, so that appending to it
    // copies nothing until it ends.
    private readonly List<string> _lines = new();
    private readonly StringBuilder _line = new();
    public string DepthStr => new string(' ', Depth * _depthCount);
    public bool Empty => _lines.Count == 0 && string.IsNullOrWhiteSpace(_line.ToString());

#if NETSTANDARD2_0
#else
        // Debug inspection members
        private static readonly Subject<string> _lineAppended = new();
        public static IObservable<string> LineAppended => _lineAppended;
#endif
    private readonly int _depthCount;

    public int Count => _lines.Count;

    public string this[int index]
    {
        get
        {
            CheckIndex(index);
            return _lines[index];
        }
        set
        {
            CheckIndex(index);
            _lines[index] = value;
        }
    }

    public StructuredStringBuilder()
        : this(depthCount: 4)
    {
    }

    public StructuredStringBuilder(int depthCount)
    {
        _depthCount = depthCount;
    }

    public void Insert(int index, string str)
    {
        CheckIndex(index);
        _lines.Insert(index, str);
    }

    private void CheckIndex(int index)
    {
        if (index >= _lines.Count || index < 0)
        {
            throw new IndexOutOfRangeException();
        }
    }

    public void Append(string? str, bool endWithNewLine = false)
    {
        if (str == null) str = string.Empty;
#if NETSTANDARD2_0
#else
            _lineAppended.OnNext(str);
#endif
        if (str.StartsWith(Environment.NewLine, StringComparison.Ordinal))
        {
            EndLine();
            return;
        }
        var start = 0;
        int newLine;
        while ((newLine = str.IndexOf(Environment.NewLine, start, StringComparison.Ordinal)) >= 0)
        {
            _line.Append(str, start, newLine - start);
            EndLine();
            start = newLine + Environment.NewLine.Length;
        }
        _line.Append(str, start, str.Length - start);
        if (endWithNewLine)
        {
            Append(Environment.NewLine, endWithNewLine: false);
        }
    }

    private void EndLine()
    {
        _lines.Add(_line.ToString());
        _line.Clear();
    }

    internal void AppendIndentation()
    {
        _line.Append(' ', Depth * _depthCount);
    }

    public void AppendLine()
    {
        Append(Environment.NewLine);
    }

    public void AppendLines(IEnumerable<string?> strs)
    {
        foreach (var str in strs)
        {
            AppendLine(str);
        }
    }

    public void AppendLines(IEnumerable<string?> strs, string delimeter)
    {
        foreach (var str in strs.IterateMarkLast())
        {
            if (str.Last)
            {
                AppendLine(str.Item);
            }
            else
            {
                Append(str.Item);
                Append(delimeter);
            }
        }
    }

    public void AppendLine(string? str, bool extraLine = false, bool appendNewLine = true)
    {
        AppendIndentation();
        Append(str);
        if (appendNewLine)
        {
            AppendLine();
        }

        if (extraLine)
        {
            AppendLine();
        }
    }

    public void AppendItem<T>([MaybeNull] T item, string? name = null)
    {
        if (item == null) return;
        if (name == null)
        {
            AppendLine(item.ToString());
        }
        else
        {
            AppendLine($"{name} => {item}");
        }
    }

    public void AppendItem(IPrintable? item, string? name = null)
    {
        if (item == null) return;
        item.Print(this, name);
    }

    public void RemoveAt(int index)
    {
        CheckIndex(index);
        _lines.RemoveAt(index);
    }

    public string GetString()
    {
        return string.Join(Environment.NewLine, _lines.Append(_line.ToString()));
    }

    public override string ToString()
    {
        return GetString();
    }

    public IEnumerator<string> GetEnumerator()
    {
        for (int i = 0; i < _lines.Count; i++)
        {
            yield return _lines[i];
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
    
    public Depth IncreaseDepth(bool doIt = true)
    {
        return new Depth(this, doIt);
    }
    
    public Line Line()
    {
        return new Line(this);
    }
    
    public Comment Comment()
    {
        return new Comment(this);
    }
    
    public CommaCollection CommaCollection(string delimiter = ",")
    {
        return new CommaCollection(this, delimiter: delimiter);
    }

    public Brace Brace(string start = "[", string end = "]", bool doIt = true)
    {
        return new Brace(this, start, end, doIt: doIt);
    }
}