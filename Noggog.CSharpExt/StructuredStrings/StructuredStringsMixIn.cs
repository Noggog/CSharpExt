using System.IO.Abstractions;

namespace Noggog.StructuredStrings;

public static class StructuredStringsMixIn
{
    public static string Print(this IPrintable printable, string? name = null)
    {
        StructuredStringBuilder sb = new();
        printable.Print(sb, name);
        return sb.ToString();
    }
    
}