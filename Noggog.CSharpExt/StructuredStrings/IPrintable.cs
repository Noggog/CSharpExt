namespace Noggog.StructuredStrings;

public interface IPrintable
{
    void ToString(StructuredStringBuilder sb, string? name = null);
}