using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;

namespace Noggog
{
    public static class IFileSystemExt
    {
        public static readonly IFileSystem DefaultFilesystem = new FileSystem();
        
        public static bool TryDeleteEntireFolder(this IDirectory system, DirectoryPath dir, bool disableReadonly = true, bool deleteFolderItself = true)
        {
            try
            {
                DeleteEntireFolder(system, dir, disableReadonly, deleteFolderItself);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static void DeleteEntireFolder(this IDirectory system, DirectoryPath path, bool disableReadonly = true, bool deleteFolderItself = true)
        {
            if (!system.Exists(path)) return;
            string[] files = system.GetFiles(path);
            List<Exception> exceptions = new List<Exception>();
            foreach (string f in files)
            {
                var fi = system.FileSystem.FileInfo.FromFileName(f);
                if (fi.IsReadOnly)
                {
                    if (!disableReadonly) continue;
                    fi.IsReadOnly = false;
                }
                try
                {
                    fi.Delete();
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            }
            foreach (string subDir in system.GetDirectories(path))
            {
                system.DeleteEntireFolder(subDir, disableReadonly);
            }
            if (deleteFolderItself)
            {
                if (system.GetFiles(path).Length == 0
                    && system.GetDirectories(path).Length == 0)
                {
                    try
                    {
                        system.Delete(path);
                    }
                    catch (Exception ex)
                    {
                        exceptions.Add(ex);
                    }
                }
            }
            if (exceptions.Count == 1) throw exceptions[0];
            if (exceptions.Count > 1)
            {
                throw new AggregateException(exceptions);
            }
        }

        public static IEnumerable<FilePath> EnumerateFilesRecursive(this IDirectory system, DirectoryPath path, string? searchPattern = null)
        {
            return (searchPattern == null ? system.EnumerateFiles(path) : system.EnumerateFiles(path, searchPattern))
                .Select(x => new FilePath(x))
                .Concat(system.EnumerateDirectories(path)
                    .SelectMany(d => system.EnumerateFilesRecursive(d, searchPattern: searchPattern)));
        }

        /// <summary>
        /// Ensures that the higher directories iterate first before lower directories
        /// </summary>
        /// <param name="system">Filesystem to be run on</param>
        /// <param name="path">Directory to start enumeration from</param>
        /// <param name="includeSelf">Whether to include the starting directory in the return enumeration</param>
        /// <param name="recursive">Whether to recursively enumerate subdirectories</param>
        /// <returns>Enumerable of contained directories</returns>
        public static IEnumerable<DirectoryPath> EnumerateDirectories(this IDirectory system, DirectoryPath path, bool includeSelf, bool recursive)
        {
            if (!system.Exists(path)) return EnumerableExt<DirectoryPath>.Empty;
            var ret = system.EnumerateDirectories(path)
                .Select(x => new DirectoryPath(x));
            if (recursive)
            {
                ret = ret
                    .Concat(system.EnumerateDirectories(path)
                    .SelectMany(d => system.EnumerateDirectories(d, includeSelf: false, recursive: true)));
            }
            if (includeSelf)
            {
                ret = path.AsEnumerable().Concat(ret);
            }
            return ret;
        }

        public static bool IsSubfolderOf(this IDirectory system, DirectoryPath path, DirectoryPath potentialParent)
        {
            IDirectoryInfo parent;
            while ((parent = system.GetParent(path)) != null)
            {
                if (parent.FullName.Equals(potentialParent.Path, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
                path = parent.FullName;
            }
            return false;
        }

        public static void DeepCopy(this IDirectory system, DirectoryPath from, DirectoryPath to)
        {
            if (!system.Exists(to.Path))
            {
                system.CreateDirectory(to.Path);
            }
            
            foreach (var file in system.GetFiles(from))
            {
                var fi = system.FileSystem.FileInfo.FromFileName(file);
                var rhs = Path.Combine(to.Path, Path.GetFileName(file));
                fi.CopyTo(rhs);
            }

            foreach (var dir in system.GetDirectories(from))
            {
                system.DeepCopy(dir, Path.Combine(to.Path, Path.GetFileName(dir)));
            }
        }
    }
}