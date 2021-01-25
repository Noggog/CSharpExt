using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noggog.Extensions
{
    public static class PathExt
    {
        public static String MakeRelativePath(String fromPath, String toPath)
        {
            if (String.IsNullOrEmpty(fromPath)) throw new ArgumentNullException("fromPath");
            if (String.IsNullOrEmpty(toPath)) throw new ArgumentNullException("toPath");

            Uri fromUri = new Uri(fromPath);
            Uri toUri = new Uri(toPath);

            if (fromUri.Scheme != toUri.Scheme) { return toPath; } // path can't be made relative.

            Uri relativeUri = fromUri.MakeRelativeUri(toUri);
            String relativePath = Uri.UnescapeDataString(relativeUri.ToString());

            if (toUri.Scheme.Equals("file", StringComparison.InvariantCultureIgnoreCase))
            {
                relativePath = relativePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            }

            return relativePath;
        }

        public static string AddSuffix(string filePath, string suffix)
        {
            if (string.IsNullOrWhiteSpace(suffix)) return filePath;
            return Path.Combine(Path.GetDirectoryName(filePath)!, $"{Path.GetFileNameWithoutExtension(filePath)}{suffix}{Path.GetExtension(filePath)}");
        }

        #region Invalid Path Chars
        private static readonly char[] _invalidPathChars = Path.GetInvalidPathChars();

        public static bool HasInvalidPathChars(ReadOnlySpan<char> str, [MaybeNullWhen(false)] out char invalidChar)
        {
            foreach (var c in str)
            {
                int index = _invalidPathChars.IndexOf(c);
                if (index != -1)
                {
                    invalidChar = _invalidPathChars[index];
                    return true;
                }
            }
            invalidChar = default;
            return false;
        }

        public static bool HasInvalidPathChars(ReadOnlySpan<char> str)
        {
            foreach (var c in str)
            {
                if (IsInvalidPathChar(c)) return true;
            }
            return false;
        }

        public static bool IsInvalidPathChar(char c)
        {
            return _invalidPathChars.IndexOf(c) != -1;
        }
        #endregion


        #region Invalid Filename Chars
        private static readonly char[] _invalidFileNameChars = Path.GetInvalidFileNameChars();

        public static bool HasInvalidFileNameChars(ReadOnlySpan<char> str, [MaybeNullWhen(false)] out char invalidChar)
        {
            foreach (var c in str)
            {
                int index = _invalidFileNameChars.IndexOf(c);
                if (index != -1)
                {
                    invalidChar = _invalidFileNameChars[index];
                    return true;
                }
            }
            invalidChar = default;
            return false;
        }

        public static bool HasInvalidFileNameChars(ReadOnlySpan<char> str)
        {
            foreach (var c in str)
            {
                if (IsInvalidFileNameChar(c)) return true;
            }
            return false;
        }

        public static bool IsInvalidFileNameChar(char c)
        {
            return _invalidFileNameChars.IndexOf(c) != -1;
        }
        #endregion
    }
}
