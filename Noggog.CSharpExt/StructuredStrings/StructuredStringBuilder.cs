using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;
#if NETSTANDARD2_0
#else
using System.Reactive.Subjects;
#endif
using System.Text;

namespace Noggog.StructuredStrings;

public class StructuredStringBuilder : IEnumerable<string>
{
    private static readonly string[] NewLineArr = new[] { Environment.NewLine };
        
    public int Depth;
    private readonly List<string> _strings = new();
    public string DepthStr
    {
        get
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(' ', Depth * _depthCount);
            return sb.ToString();
        }
    }
    public bool Empty
    {
        get
        {
            if (_strings.Count > 1) return false;
            if (_strings.Count == 0) return true;
            return string.IsNullOrWhiteSpace(_strings[0]);
        }
    }

#if NETSTANDARD2_0
#else
        // Debug inspection members
        private static readonly Subject<string> _lineAppended = new();
        public static IObservable<string> LineAppended => _lineAppended;
#endif
    private readonly int _depthCount;

    public int Count => _strings.Count - 1;

    public string this[int index]
    {
        get
        {
            CheckIndex(index);
            return _strings[index];
        }
        set
        {
            CheckIndex(index);
            if (index == _strings.Count - 1)
            {
                AppendLine(value);
            }
            else
            {
                _strings[index] = value;
            }
        }
    }

    public StructuredStringBuilder()
        : this(depthCount: 4)
    {
    }

    public StructuredStringBuilder(int depthCount)
    {
        _depthCount = depthCount;
        AppendLine();
    }

    public void Insert(int index, string str)
    {
        CheckIndex(index);
        _strings.Insert(index, str);
    }

    private void CheckIndex(int index)
    {
        if (index >= _strings.Count - 1 || index < 0)
        {
            throw new IndexOutOfRangeException();
        }
    }

    public void Append(string? str)
    {
        if (str == null) str = string.Empty;
#if NETSTANDARD2_0
#else
            _lineAppended.OnNext(str);
#endif
        if (str.StartsWith(Environment.NewLine))
        {
            _strings.Add("");
            return;
        }
        string[] split = str.Split(NewLineArr, StringSplitOptions.None);
        split.First(
            (s, first) =>
            {
                if (_strings.Count == 0)
                {
                    _strings.Add(s);
                }
                else
                {
                    if (first)
                    {
                        _strings[_strings.Count - 1] = _strings[_strings.Count - 1] + s;
                    }
                    else
                    {
                        _strings.Add(s);
                    }
                }
            });
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

    public void AppendLine(string? str, bool extraLine = false)
    {
        using (new Line(this))
        {
            Append(str);
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
        _strings.RemoveAt(index);
    }

    public string GetString()
    {
        return string.Join(Environment.NewLine, _strings);
    }

    public override string ToString()
    {
        return GetString();
    }

    public IEnumerator<string> GetEnumerator()
    {
        for (int i = 0; i < _strings.Count - 1; i++)
        {
            yield return _strings[i];
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
}