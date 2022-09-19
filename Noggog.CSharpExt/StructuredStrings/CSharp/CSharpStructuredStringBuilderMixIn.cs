namespace Noggog.StructuredStrings.CSharp;

public static class CSharpStructuredStringBuilderMixIn
{
    public static Call Call(this StructuredStringBuilder sb,
        string? initialLine = null,
        string? suffixLine = null,
        bool semiColon = true,
        bool linePerArgument = true)
    {
        return new Call(sb,
            initialLine: initialLine,
            suffixLine: suffixLine,
            semiColon: semiColon,
            linePerArgument: linePerArgument);
    }

    public static Function Function(this StructuredStringBuilder sb, string initialLine, bool semiColon = false)
    {
        return new Function(sb,
            initialLine: initialLine)
        {
            SemiColon = semiColon
        };
    }

    public static Class Class(this StructuredStringBuilder sb, string name)
    {
        return new Class(sb, name: name);
    }

    public static Namespace Namespace(this StructuredStringBuilder sb, string str, bool fileScoped = true)
    {
        return new Namespace(sb, str: str, fileScoped: fileScoped);
    }

    public static If If(this StructuredStringBuilder sb, bool ands, bool first = true)
    {
        return new If(sb, ANDs: ands, first: first);
    }

    public static Region Region(this StructuredStringBuilder sb, string str, bool appendExtraLine = true, bool skipIfOnlyOneLine = false)
    {
        return new Region(sb, str: str, appendExtraLine: appendExtraLine, skipIfOnlyOneLine: skipIfOnlyOneLine);
    }

    public static PropertyCtor PropertyCtor(this StructuredStringBuilder sb)
    {
        return new PropertyCtor(sb);
    }
    
    public static CurlyBrace CurlyBrace(
        this StructuredStringBuilder sb,
        bool extraLine = true, 
        bool doIt = true,
        bool appendParenthesis = false,
        bool appendSemiColon = false,
        bool appendComma = false)
    {
        return new CurlyBrace(sb, doIt)
        {
            AppendSemicolon = appendSemiColon,
            AppendParenthesis = appendParenthesis,
            AppendComma = appendComma
        };
    }

    public static string ToCodeString(this AccessModifier accessModifier)
    {
        return accessModifier switch
        {
            AccessModifier.Public => "public",
            AccessModifier.Private => "private",
            AccessModifier.Protected => "protected",
            AccessModifier.Internal => "internal",
            _ => throw new ArgumentOutOfRangeException(nameof(accessModifier), accessModifier, null)
        };
    }

    public static string ToCodeString(this ObjectType type)
    {
        return type switch
        {
            ObjectType.Class => "class",
            ObjectType.Struct => "struct",
            ObjectType.Interface => "interface",
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }
}