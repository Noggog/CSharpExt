namespace Noggog.UI;

/// <summary>
/// Platform-agnostic file dialog filter.  Mirrors the construction semantics of
/// WindowsAPICodePack's CommonFileDialogFilter so existing call sites only need a type rename.
/// Extensions are stored bare (no leading '*' or '.').
/// </summary>
public class DialogFileFilter
{
    public string DisplayName { get; set; }

    public List<string> Extensions { get; } = new();

    public DialogFileFilter(string rawDisplayName, string extensionList)
    {
        if (string.IsNullOrEmpty(extensionList))
        {
            throw new ArgumentNullException(nameof(extensionList));
        }

        DisplayName = rawDisplayName;

        // Parse the extension list, supporting comma/semicolon separation and
        // optional "*." or "." prefixes, matching CommonFileDialogFilter.
        foreach (var raw in extensionList.Split(',', ';'))
        {
            var clean = raw.Trim();
            if (clean.StartsWith("*."))
            {
                clean = clean.Substring(2);
            }
            else if (clean.StartsWith("."))
            {
                clean = clean.Substring(1);
            }
            else if (clean.StartsWith("*"))
            {
                clean = clean.Substring(1);
            }

            if (!string.IsNullOrWhiteSpace(clean))
            {
                Extensions.Add(clean);
            }
        }
    }
}
