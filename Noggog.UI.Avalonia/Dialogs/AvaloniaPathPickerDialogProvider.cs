using Avalonia.Controls;
using Avalonia.Platform.Storage;

namespace Noggog.UI;

public class AvaloniaPathPickerDialogProvider : IPathPickerDialogProvider
{
    private readonly Func<Window?> _getWindow;

    public AvaloniaPathPickerDialogProvider(Func<Window?> getWindow)
    {
        _getWindow = getWindow;
    }

    public async Task<string?> ShowPickerAsync(PathPickerDialogRequest request)
    {
        var window = _getWindow();
        if (window?.StorageProvider is not { } storage) return null;

        IStorageFolder? startFolder = null;
        if (!string.IsNullOrWhiteSpace(request.InitialDirectory))
        {
            try
            {
                startFolder = await storage.TryGetFolderFromPathAsync(request.InitialDirectory);
            }
            catch
            {
                // Ignore an invalid starting directory; the picker just opens at its default.
            }
        }

        if (request.IsFolderPicker)
        {
            var folders = await storage.OpenFolderPickerAsync(new FolderPickerOpenOptions
            {
                Title = request.Title,
                AllowMultiple = false,
                SuggestedStartLocation = startFolder,
            });
            return folders.Count > 0 ? folders[0].TryGetLocalPath() : null;
        }

        var fileTypes = request.Filters.Count > 0
            ? request.Filters.Select(f => new FilePickerFileType(f.DisplayName)
            {
                Patterns = f.Extensions.Select(e => "*." + e).ToList(),
            }).ToList()
            : null;

        var files = await storage.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = request.Title,
            AllowMultiple = false,
            SuggestedStartLocation = startFolder,
            FileTypeFilter = fileTypes,
        });
        return files.Count > 0 ? files[0].TryGetLocalPath() : null;
    }
}
