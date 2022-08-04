using System.Diagnostics.CodeAnalysis;

namespace Noggog.DotNetCli.DI;

public interface INugetListingParser
{
    bool TryParse(
        string line, 
        [MaybeNullWhen(false)] out string package,
        [MaybeNullWhen(false)] out string requested,
        [MaybeNullWhen(false)] out string resolved,
        out string? latest);
}

public class NugetListingParser : INugetListingParser
{
    private readonly char[] _split = new[] { ' ' };
    
    public bool TryParse(
        string line, 
        [MaybeNullWhen(false)] out string package,
        [MaybeNullWhen(false)] out string requested,
        [MaybeNullWhen(false)] out string resolved,
        out string? latest)
    {
        var startIndex = line.IndexOf("> ");
        if (startIndex == -1)
        {
            package = default;
            requested = default;
            resolved = default;
            latest = default;
            return false;
        }
        var split = line
            .Substring(startIndex + 2)
            .Split(_split, StringSplitOptions.RemoveEmptyEntries)
            .WithIndex()
            .Where(x => x.Index == 0 || x.Item != "(D)")
            .Select(x => x.Item)
            .ToArray();
        package = split[0];
        requested = split[1];
        resolved = split[2];
        if (split.Length > 3)
        {
            latest = split[3];
        }
        else
        {
            latest = null;
        }
        return true;
    }
}
