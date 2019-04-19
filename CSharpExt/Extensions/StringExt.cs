using System;
using System.Collections.Generic;
using System.Text;

namespace System
{
    public static class StringExt
    {
        public static bool IsNumeric(this string s, bool floatingPt = true)
        {
            s = s.Trim();
            if (floatingPt)
            {
                return double.TryParse(s, out double d);
            }
            else
            {
                return Int32.TryParse(s, out int i);
            }
        }

        public static bool Extract(this string s, string target, out string result)
        {
            if (s == null)
            {
                result = s;
                return false;
            }
            int index = s.IndexOf(target);
            if (index == -1)
            {
                result = s;
                return false;
            }
            result = s.Remove(index, target.Length);
            return true;
        }

        public static IEnumerable<string> Split(this string line, string delim, char escapeChar)
        {
            int index = -1;
            string replace = escapeChar + delim;
            // Parse into delimited elements
            while ((index = line.IndexOf(delim, index + 1)) != -1)
            {
                if (index > 0 && line[index - 1] == escapeChar)
                { // Was escaped
                    continue;
                }
                yield return line.Substring(0, index).Replace(replace, delim);
                line = line.Substring(index + 1);
                index = -1;
            }
            yield return line.Replace(replace, delim);
        }

        public static bool TrySubstringFromStart(this string src, string item, out string result)
        {
            int index = src.IndexOf(item);
            if (index == -1)
            {
                result = src;
                return false;
            }
            index += item.Length;
            if (index == src.Length)
            {
                result = string.Empty;
                return true;
            }
            result = src.Substring(index);
            return true;
        }

        public static string SubstringFromStart(this string src, string item)
        {
            TrySubstringFromStart(src, item, out string result);
            return result;
        }

        public static bool TrySubstringFromEnd(this string src, string item, out string result)
        {
            int index = src.LastIndexOf(item);
            if (index == -1)
            {
                result = src;
                return false;
            }
            if (index == 0)
            {
                result = string.Empty;
                return true;
            }
            result = src.Substring(0, index);
            return true;
        }

        public static string SubstringFromEnd(this string src, string item)
        {
            TrySubstringFromEnd(src, item, out string result);
            return result;
        }

        public static bool TryTrimStart(this string src, string item, out string result)
        {
            if (!src.StartsWith(item))
            {
                result = src;
                return false;
            }
            if (src.Length == item.Length)
            {
                result = string.Empty;
                return true;
            }
            result = src.Substring(item.Length);
            return true;
        }

        public static string TrimStart(this string src, string item)
        {
            TryTrimStart(src, item, out string result);
            return result;
        }

        public static bool TryTrimEnd(this string src, string item, out string result)
        {
            if (!src.EndsWith(item))
            {
                result = src;
                return false;
            }
            if (src.Length == item.Length)
            {
                result = string.Empty;
                return true;
            }
            result = src.Substring(0, src.Length - item.Length);
            return true;
        }

        public static string TrimEnd(this string src, string item)
        {
            TryTrimEnd(src, item, out string result);
            return result;
        }

        public static byte[] ToBytes(this string str)
        {
            return Encoding.ASCII.GetBytes(str);
        }
    }
}
