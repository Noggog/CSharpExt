namespace Noggog.UI;

/// <summary>
/// Platform-specific provider that shows a file/folder picker and returns the chosen path.
/// Implementations live in the UI framework assembly (e.g. Noggog.WPF, or an Avalonia app).
/// <para>
/// Register an implementation via <see cref="PathPickerDialogProvider.Instance"/>.  Noggog.WPF
/// auto-registers a WPF implementation on assembly load; Avalonia apps register theirs at startup.
/// </para>
/// </summary>
public interface IPathPickerDialogProvider
{
    /// <summary>
    /// Shows the picker and returns the selected path, or null if cancelled.
    /// Async so that frameworks with async-only dialogs (e.g. Avalonia's StorageProvider) work
    /// without blocking the UI thread; synchronous/modal implementations return a completed task.
    /// </summary>
    Task<string?> ShowPickerAsync(PathPickerDialogRequest request);
}

/// <summary>
/// Ambient registration point for the active <see cref="IPathPickerDialogProvider"/>.
/// Kept as a simple static so the parameterless <see cref="PathPickerVM"/> JSON constructor
/// and existing call sites need no dependency-injection wiring.
/// </summary>
public static class PathPickerDialogProvider
{
    public static IPathPickerDialogProvider? Instance { get; set; }
}
