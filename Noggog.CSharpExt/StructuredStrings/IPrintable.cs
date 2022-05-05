namespace Noggog.StructuredStrings;

public interface IPrintable
{
    void Print(StructuredStringBuilder sb, string? name = null);
}