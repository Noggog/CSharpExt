using System;
using System.IO;

namespace System
{
    public static class DirectoryInfoExt
    {
        public static bool DeleteEntireFolder(this DirectoryInfo dir, bool disableReadonly = true)
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