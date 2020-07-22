using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Noggog
{
    public static class DirectoryInfoExt
    {
        public static bool DeleteEntireFolder(this DirectoryInfo dir, bool disableReadonly = true, bool deleteFolderItself = true)
        {
            if (!dir.Exists()) return true;
            bool pass = true;
            FileInfo[] files = dir.GetFiles("*.*");
            foreach (FileInfo fi in files)
            {
                if (fi.IsReadOnly)
                {
                    if (!disableReadonly) continue;
                    fi.IsReadOnly = false;
                }
                try
                {
                    fi.Delete();
                }
                catch (IOException)
                {
                    pass = false;
                }
            }
            foreach (DirectoryInfo subDir in dir.GetDirectories())
            {
                subDir.DeleteEntireFolder(disableReadonly);
            }
            if (!deleteFolderItself) return true;
            dir.Refresh();
            if (dir.GetFiles().Length == 0)
            {
                try
                {
                    dir.Delete();
                }
                catch (IOException)
                {
                    pass = false;
                }
            }
            return pass;
        }

        public static bool Exists(this DirectoryInfo source)
        {
            source.Refresh();
            return source.Exists;
        }

        public static void DeleteContainedFiles(this DirectoryInfo dir, bool recursive)
        {
            if (dir.Exists)
            {
                FileInfo[] files = dir.GetFiles("*.*");
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

        public static IEnumerable<FileInfo> EnumerateFilesRecursive(this DirectoryInfo dir)
        {
            return dir.EnumerateFiles()
                .Concat(dir.EnumerateDirectories()
                    .SelectMany(d => d.EnumerateFilesRecursive()));
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
                if (dir.Parent.FullName.Equals(potentialParent.FullName))
                {
                    return true;
                }
                dir = dir.Parent;
            }
            return false;
        }
    }
}
