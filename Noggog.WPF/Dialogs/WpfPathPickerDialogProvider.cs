using Microsoft.WindowsAPICodePack.Dialogs;
using Noggog.UI;

namespace Noggog.WPF;

/// <summary>
/// WPF / Windows implementation of <see cref="IPathPickerDialogProvider"/>, backed by
/// WindowsAPICodePack's CommonOpenFileDialog.
/// </summary>
public class WpfPathPickerDialogProvider : IPathPickerDialogProvider
{
    public Task<string?> ShowPickerAsync(PathPickerDialogRequest request)
    {
        var dlg = new CommonOpenFileDialog
        {
            Title = request.Title,
            IsFolderPicker = request.IsFolderPicker,
            InitialDirectory = request.InitialDirectory,
            AddToMostRecentlyUsedList = false,
            AllowNonFileSystemItems = false,
            DefaultDirectory = request.InitialDirectory,
            EnsureFileExists = request.EnsureFileExists,
            EnsurePathExists = request.EnsurePathExists,
            EnsureReadOnly = false,
            EnsureValidNames = true,
            Multiselect = false,
            ShowPlacesList = true,
        };
        foreach (var filter in request.Filters)
        {
            dlg.Filters.Add(new CommonFileDialogFilter(filter.DisplayName, string.Join(",", filter.Extensions)));
        }
        if (dlg.ShowDialog() != CommonFileDialogResult.Ok) return Task.FromResult<string?>(null);
        return Task.FromResult<string?>(dlg.FileName);
    }
}
