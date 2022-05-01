namespace Noggog.StructuredStrings.CSharp;

public class Class : IDisposable
{
    private readonly StructuredStringBuilder _sb;
    
    public string Name { get; }
    public AccessModifier AccessModifier = AccessModifier.Public;
    public bool Partial;
    public bool Abstract;
    public bool Static;
    public string? BaseClass;
    public bool New;
    public ObjectType Type = ObjectType.Class;
    public HashSet<string> Interfaces = new();
    public List<string> Wheres = new();
    public List<string> Attributes = new();

    public Class(StructuredStringBuilder sb, string name)
    {
        this._sb = sb;
        Name = name;
    }

    public void Dispose()
    {
        foreach (var attr in Attributes)
        {
            _sb.AppendLine(attr);
        }
        var classLine = $"{AccessModifier.ToCodeString()} {(Static ? "static " : null)}{(New ? "new " : null)}{(Abstract ? "abstract " : null)}{(Partial ? "partial " : null)}{Type.ToCodeString()} {Name}";
        var toAdd = Interfaces.OrderBy(x => x).ToList();
        if (BaseClass != null &&!string.IsNullOrWhiteSpace(BaseClass))
        {
            toAdd.Insert(0, BaseClass);
        }
        if (toAdd.Count > 1)
        {
            _sb.AppendLine($"{classLine} :");
            _sb.Depth++;
            toAdd.Last(
                each: (item, last) =>
                {
                    _sb.AppendLine($"{item}{(last ? string.Empty : ",")}");
                });
            _sb.Depth--;
        }
        else if (toAdd.Count == 1)
        {
            _sb.AppendLine($"{classLine} : {toAdd.First()}");
        }
        else
        {
            _sb.AppendLine(classLine);
        }
        if (Wheres.Count > 0)
        {
            using (_sb.IncreaseDepth())
            {
                foreach (var where in Wheres)
                {
                    _sb.AppendLine(where);
                }
            }
        }
    }
}