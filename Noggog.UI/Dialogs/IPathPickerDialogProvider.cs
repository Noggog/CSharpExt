namespace Noggog.UI;

/// <summary>
/// Platform-specific provider that shows a file/folder picker and returns the chosen path.
/// Implementations live in the UI framework assembly (e.g. Noggog.WPF, or an Avalonia app)
/// and are injected into the <see cref="PathPickerVM"/> that uses them.
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
