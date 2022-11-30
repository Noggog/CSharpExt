using System.Diagnostics.CodeAnalysis;

namespace Noggog;

public struct FileName : IEquatable<FileName>
{
    private readonly string? _name;

    public string Extension => Path.GetExtension(String);
    public string NameWithoutExtension => Path.GetFileNameWithoutExtension(String);
    public string String => _name ?? string.Empty;

    public FileName(string name, bool check = true)
    {
        if (check && Path.GetFileName(name) != name)
        {
            throw new ArgumentException($"Filename was constructed with a directory in its path: {name}");
        }
        _name = name;
    }

    public bool Equals(FileName other)
    {
        return String.Equals(other.String, StringComparison.OrdinalIgnoreCase);
    }

    public override bool Equals(object? obj)
    {
        if (obj is not FileName rhs) return false;
        return Equals(rhs);
    }

    public override int GetHashCode()
    {
#if NETSTANDARD2_0 
        return String.ToLower().GetHashCode();
#else 
            return String.GetHashCode(StringComparison.OrdinalIgnoreCase);
#endif
    }

    public override string ToString()
    {
        return String;
    }

    public static implicit operator FileName(string path)
    {
        return new FileName(path);
    }

    [return: NotNullIfNotNull("path")]
    public static implicit operator FileName?(string? path)
    {
        if (path == null) return null;
        return new FileName(path);
    }

    public static implicit operator string(FileName name)
    {
        return name.String;
    }

    [return: NotNullIfNotNull("name")]
    public static implicit operator string?(FileName? name)
    {
        if (name == null) return null;
        return (string)name.Value;
    }
        
#if NETSTANDARD2_0 
#else 
        public static implicit operator ReadOnlySpan<char>(FileName name)
        {
            return name.String;
        }
#endif
}