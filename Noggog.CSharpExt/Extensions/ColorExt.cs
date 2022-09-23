using System.Drawing;

namespace Noggog;

public static class ColorExt
{
    public static bool ColorOnlyEquals(this Color color, Color rhs)
    {
        return color.A == rhs.A
               && color.R == rhs.R
               && color.G == rhs.G
               && color.B == rhs.B;
    }

    public static bool ColorOnlyEquals(this Color? color, Color? rhs)
    {
        if (color.HasValue && rhs.HasValue)
        {
            return ColorOnlyEquals(color.Value, rhs.Value);
        }
        else
        {
            return !color.HasValue && !rhs.HasValue;
        }
    }

#if NETSTANDARD2_0 
#else
    private static ReadOnlySpan<char> TryGetSection(ReadOnlySpan<char> str, out ReadOnlySpan<char> section,
        bool throwIfMissing)
    {
        var index = str.IndexOf(",");
        if (index != -1)
        {
            section = str.Slice(0, index).Trim();
            return str.Slice(index + 1).Trim();
        }

        if (str.IsEmpty || str.IsWhiteSpace())
        {
            if (throwIfMissing)
            {
                throw new ArgumentException("Did not have expected number of splits", nameof(str));
            }
            section = string.Empty;
            return str;
        }

        section = str.Trim();
        return string.Empty;
    }

    public static ErrorResponse TryConvertFromCommaString(ReadOnlySpan<char> span, out Color color)
    {
        ReadOnlySpan<char> first, second, third, fourth;

        span = TryGetSection(span, out first, true);
        span = TryGetSection(span, out second, true);
        span = TryGetSection(span, out third, true);
        span = TryGetSection(span, out fourth, false);

        if (!span.IsEmpty)
        {
            color = default;
            return ErrorResponse.Fail($"Had unexpected suffix content: {span}");
        }

        if (fourth.IsEmpty)
        {
            if (!Byte.TryParse(first, out var r))
            {
                color = default;
                return ErrorResponse.Fail($"Inconvertible section: {first}");
            }
            if (!Byte.TryParse(second, out var g))
            {
                color = default;
                return ErrorResponse.Fail($"Inconvertible section: {second}");
            }
            if (!Byte.TryParse(third, out var b))
            {
                color = default;
                return ErrorResponse.Fail($"Inconvertible section: {third}");
            }

            color = Color.FromArgb(r, g, b);
        }
        else
        {
            if (!Byte.TryParse(first, out var a))
            {
                color = default;
                return ErrorResponse.Fail($"Inconvertible section: {first}");
            }
            if (!Byte.TryParse(second, out var r))
            {
                color = default;
                return ErrorResponse.Fail($"Inconvertible section: {second}");
            }
            if (!Byte.TryParse(third, out var g))
            {
                color = default;
                return ErrorResponse.Fail($"Inconvertible section: {third}");
            }
            if (!Byte.TryParse(fourth, out var b))
            {
                color = default;
                return ErrorResponse.Fail($"Inconvertible section: {fourth}");
            }

            color = Color.FromArgb(a, r, g, b);
        }
        return ErrorResponse.Success;
    }

    public static Color ConvertFromCommaString(ReadOnlySpan<char> span)
    {
        var err = TryConvertFromCommaString(span, out var color);
        if (err.Succeeded)
        {
            return color;
        }

        throw new ArgumentException($"Cannot convert to Color: {span}. {err.Reason}", nameof(span));
    }
#endif

    public enum IncludeAlpha
    {
        WhenApplicable,
        Always,
        Never,
    }
    
    public static string CommaString(this Color color, IncludeAlpha alpha = IncludeAlpha.WhenApplicable)
    {
        switch (alpha)
        {
            case IncludeAlpha.Always:
            case IncludeAlpha.WhenApplicable when color.A != 255: 
                return $"{color.A}, {color.R}, {color.G}, {color.B}";
            case IncludeAlpha.WhenApplicable:
            case IncludeAlpha.Never:
                return $"{color.R}, {color.G}, {color.B}";
            default:
                throw new ArgumentOutOfRangeException(nameof(alpha), alpha, null);
        }
    }
}