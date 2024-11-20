using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.RegularExpressions;

namespace Noggog;

public static class StringExt
{
    public static bool IsNumeric(this string s, bool floatingPt = true)
    {
        s = s.Trim();
        if (floatingPt)
        {
            return double.TryParse(s, out double d);
        }
        else
        {
            return Int32.TryParse(s, out int i);
        }
    }

    [Obsolete("Use alternative with comparison parameter")]
    public static IEnumerable<string> Split(this string line, string delim, char escapeChar)
    {
        return Split(line, delim, escapeChar, StringComparison.CurrentCulture);
    }

    public static IEnumerable<string> Split(this string line, string delim, char escapeChar, StringComparison comparison)
    {
        int index = -1;
        string replace = escapeChar + delim;
        // Parse into delimited elements
        while ((index = line.IndexOf(delim, index + 1, comparison)) != -1)
        {
            if (index > 0 && line[index - 1] == escapeChar)
            { // Was escaped
                continue;
            }
            yield return line.Substring(0, index).Replace(replace, delim);
            line = line.Substring(index + 1);
            index = -1;
        }
        yield return line.Replace(replace, delim);
    }

    [Obsolete("Use alternative with comparison parameter")]
    public static bool TrySubstringFromStart(this string src, string item, out string result)
    {
        return TrySubstringFromStart(src, item, StringComparison.CurrentCulture, out result);
    }

    public static bool TrySubstringFromStart(this string src, string item, StringComparison comparison, out string result)
    {
        int index = src.IndexOf(item, comparison);
        if (index == -1)
        {
            result = src;
            return false;
        }
        index += item.Length;
        if (index == src.Length)
        {
            result = string.Empty;
            return true;
        }
        result = src.Substring(index);
        return true;
    }

    [Obsolete("Use alternative with comparison parameter")]
    public static string SubstringFromStart(this string src, string item)
    {
        TrySubstringFromStart(src, item, out string result);
        return result;
    }

    public static string SubstringFromStart(this string src, string item, StringComparison comparison)
    {
        TrySubstringFromStart(src, item, comparison, out string result);
        return result;
    }

    [Obsolete("Use alternative with comparison parameter")]
    public static bool TrySubstringFromEnd(this string src, string item, out string result)
    {
        return TrySubstringFromEnd(src, item, StringComparison.CurrentCulture, out result);
    }

    public static bool TrySubstringFromEnd(this string src, string item, StringComparison comparison, out string result)
    {
        int index = src.LastIndexOf(item, comparison);
        if (index == -1)
        {
            result = src;
            return false;
        }
        if (index == 0)
        {
            result = string.Empty;
            return true;
        }
        result = src.Substring(0, index);
        return true;
    }

    [Obsolete("Use alternative with comparison parameter")]
    public static string SubstringFromEnd(this string src, string item)
    {
        TrySubstringFromEnd(src, item, out string result);
        return result;
    }

    public static string SubstringFromEnd(this string src, string item, StringComparison comparison)
    {
        TrySubstringFromEnd(src, item, comparison, out string result);
        return result;
    }

    [Obsolete("Use alternative with comparison parameter")]
    public static bool TryTrimStart(this string src, string item, out string result)
    {
        return TryTrimStart(src, item, StringComparison.CurrentCulture, out result);
    }
    
    public static bool TryTrimStart(this string src, string item, StringComparison comparison, out string result)
    {
        if (!src.StartsWith(item, comparison))
        {
            result = src;
            return false;
        }
        if (src.Length == item.Length)
        {
            result = string.Empty;
            return true;
        }
        result = src.Substring(item.Length);
        return true;
    }

    [Obsolete("Use alternative with comparison parameter")]
    public static string TrimStart(this string src, string item)
    {
        TryTrimStart(src, item, out string result);
        return result;
    }

    public static string TrimStart(this string src, string item, StringComparison comparison)
    {
        TryTrimStart(src, item, comparison, out string result);
        return result;
    }

    public static bool TryTrimEnd(this string src, string item, out string result)
    {
        if (!src.EndsWith(item))
        {
            result = src;
            return false;
        }
        if (src.Length == item.Length)
        {
            result = string.Empty;
            return true;
        }
        result = src.Substring(0, src.Length - item.Length);
        return true;
    }

    public static string RemoveDisallowedFilepathChars(string str)
    {
        Regex illegalInFileName = new Regex(@"[\\/:*?""<>|]");
        return illegalInFileName.Replace(str, "");
    }

    public static bool IsViableFilename(string str)
    {
        return str.IndexOfAny(Path.GetInvalidFileNameChars()) == -1;
    }

#if NETSTANDARD2_0
#else
    public static bool ContainsInsensitive(this string str, string rhs)
    {
        return str.Contains(rhs, StringComparison.OrdinalIgnoreCase);
    }
#endif

    /// <summary>
    /// Takes in a nullable string, and applies a string converter if it is not null or empty.
    /// </summary>
    /// <param name="src">String to process</param>
    /// <param name="decorator">String decorator if source not null or empty</param>
    /// <returns>Decorated string, or null/empty if source was null/empty</returns>
    [return: NotNullIfNotNull("src")]
    public static string? Decorate(this string? src, Func<string, string> decorator)
    {
        if (src == null) return null;
        if (src == string.Empty) return string.Empty;
        return decorator(src);
    }

    public static bool IsNullOrWhitespace([NotNullWhen(false)] this string? src)
    {
        return string.IsNullOrWhiteSpace(src);
    }

    public static bool IsNullOrEmpty([NotNullWhen(false)] this string? src)
    {
        return string.IsNullOrEmpty(src);
    }
}