namespace Noggog;

public static class DirectoryInfoExt
{
    public static bool TryDeleteEntireFolder(this DirectoryInfo dir, bool disableReadOnly = true, bool deleteFolderItself = true)
    {
        try
        {
            DeleteEntireFolder(dir, disableReadOnly, deleteFolderItself);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public static void DeleteEntireFolder(
        this DirectoryInfo dir, 
        bool disableReadOnly = true,
        bool deleteFolderItself = true)
    {
        var dirPath = new DirectoryPath(dir.FullName);
        dirPath.DeleteEntireFolder(
            disableReadOnly: disableReadOnly,
            deleteFolderItself: deleteFolderItself);
    }

    public static bool Exists(this DirectoryInfo source)
    {
        source.Refresh();
        return source.Exists;
    }

    [Obsolete("Use DeleteEntireFolder instead")]
    public static void DeleteContainedFiles(this DirectoryInfo dir, bool recursive)
    {
        if (dir.Exists)
        {
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo fi in files)
            {
                fi.Delete();
            }
            if (recursive)
            {
                foreach (DirectoryInfo subDir in dir.GetDirectories())
                {
                    DeleteContainedFiles(subDir, recursive);
                }
            }
        }
    }

    public static IEnumerable<FileInfo> EnumerateFilesRecursive(this DirectoryInfo dir, string? searchPattern = null)
    {
        return (searchPattern == null ? dir.EnumerateFiles() : dir.EnumerateFiles(searchPattern))
            .Concat(dir.EnumerateDirectories()
                .SelectMany(d => d.EnumerateFilesRecursive(searchPattern: searchPattern)));
    }

    // Ensures that the higher directories iterate first before lower directories
    public static IEnumerable<DirectoryInfo> EnumerateDirectories(this DirectoryInfo dir, bool includeSelf, bool recursive)
    {
        if (!dir.Exists) return EnumerableExt<DirectoryInfo>.Empty;
        var ret = dir.EnumerateDirectories();
        if (recursive)
        {
            ret = ret
                .Concat(dir.EnumerateDirectories()
                    .SelectMany(d => d.EnumerateDirectories(includeSelf: false, recursive: true)));
        }
        if (includeSelf)
        {
            ret = dir.AsEnumerable().Concat(ret);
        }
        return ret;
    }

    public static bool IsSubfolderOf(this DirectoryInfo dir, DirectoryInfo potentialParent)
    {
        while (dir.Parent != null)
        {
            if (dir.Parent.FullName.Equals(potentialParent.FullName, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            dir = dir.Parent;
        }
        return false;
    }

    public static void DeepCopy(this DirectoryInfo from, DirectoryInfo to)
    {
        if (!to.Exists)
        {
            to.Create();
        }
            
        foreach (var file in from.GetFiles())
        {
            file.CopyTo(Path.Combine(to.FullName, file.Name));
        }

        foreach (var dir in from.GetDirectories())
        {
            dir.DeepCopy(new DirectoryInfo(Path.Combine(to.FullName, dir.Name)));
        }
    }
}