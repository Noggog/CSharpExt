namespace Noggog.UI;

/// <summary>
/// Platform-agnostic description of a file/folder picker request.
/// Consumed by <see cref="IPathPickerDialogProvider"/> implementations.
/// </summary>
public class PathPickerDialogRequest
{
    public string Title { get; set; } = string.Empty;

    public bool IsFolderPicker { get; set; }

    public string InitialDirectory { get; set; } = string.Empty;

    public bool EnsureFileExists { get; set; }

    public bool EnsurePathExists { get; set; }

    public IReadOnlyList<DialogFileFilter> Filters { get; set; } = Array.Empty<DialogFileFilter>();
}
