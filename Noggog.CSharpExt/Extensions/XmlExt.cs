using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Xml.Linq;

namespace Noggog;

public static class XmlExt
{
    public static IEnumerable<XElement> GetChildCaseInsensitive(this XElement node, string named)
    {
        named = named.ToUpper();
        foreach (XElement child in node.Elements())
        {
            if (child.Name.LocalName.ToUpper().Equals(named))
            {
                yield return child;
            }
        }
    }

    public static string PrintPretty(this XDocument doc)
    {
        MemoryStream mStream = new MemoryStream();
        var writer = new System.Xml.XmlTextWriter(mStream, Encoding.Unicode);
        writer.Formatting = System.Xml.Formatting.Indented;
        doc.WriteTo(writer);
        writer.Flush();
        mStream.Flush();
        mStream.Position = 0;
        StreamReader sReader = new StreamReader(mStream);
        return sReader.ReadToEnd();
    }

    public static bool TryGetAttribute(this XElement node, string str, [MaybeNullWhen(false)] out XAttribute val)
    {
        if (node != null)
        {
            var n = node.Attribute(str);
            if (n != null)
            {
                val = n;
                return true;
            }
        }
        val = null;
        return false;
    }

    public static bool TryGetAttributeString(this XElement node, string str, out string val)
    {
        if (TryGetAttribute(node, str, out var attr))
        {
            val = attr.Value;
            return true;
        }
        val = string.Empty;
        return false;
    }

    public static bool TryGetAttribute<P>(this XElement node, string str, [MaybeNullWhen(false)] out P val, Func<string, P> converter)
    {
        if (!TryGetAttributeString(node, str, out string strVal))
        {
            val = default;
            return false;
        }
        val = converter(strVal);
        return true;
    }

    public static bool TryGetAttribute<P>(this XElement node, string str, [MaybeNullWhen(false)] out P val, bool throwException = false, CultureInfo? culture = null)
    {
        var ret = TryGetAttribute<P>(node, str, out val, (strVal) =>
        {
            if (strVal == null)
            {
                return default!;
            }
            var t = typeof(P);
            if (TypeExt.IsNullable<P>(out var underlyingType))
            {
                t = underlyingType;
            }
            try
            {
                if (t.IsEnum)
                {
                    return (P)Enum.Parse(t, strVal);
                }
                else
                {
                    return (P)Convert.ChangeType(strVal, t, culture);
                }
            }
            catch (Exception)
            {
                if (throwException)
                {
                    throw;
                }
                else
                {
                    return default!;
                }
            }
        });
        if (!ret) return false;
        return val != null;
    }

    public static P? GetAttribute<P>(this XElement node, string str, P? defaultVal = default, bool throwException = false)
    {
        if (!TryGetAttribute<P>(node, str, out var val, throwException))
        {
            val = defaultVal;
        }
        return val;
    }

    public static void TransferAttribute<P>(this XElement node, string str, Action<P> acti, bool throwException = false)
    {
        if (TryGetAttribute<P>(node, str, out var val, throwException))
        {
            acti(val);
        }
    }

    public static string? GetAttribute(this XElement node, string str, string? defaultVal = null, bool throwException = false)
    {
        if (!TryGetAttribute(node, str, out string? val, throwException))
        {
            return defaultVal;
        }
        return val;
    }

    public static bool ContentEqual(this XName name, XName rhs)
    {
        if (!name.LocalName.Equals(rhs.LocalName)) return false;
        if (!name.NamespaceName.Equals(rhs.NamespaceName)) return false;
        return true;
    }

    public static bool ContentEqual(this XAttribute attr, XAttribute rhs)
    {
        if (!attr.Name.ContentEqual(rhs.Name)) return false;
        if (!attr.Value.Equals(rhs.Value)) return false;
        return true;
    }

    public static bool ContentEqual(this XElement node, XElement rhs)
    {
        if (node.HasElements != rhs.HasElements) return false;
        if (!node.Name.ContentEqual(rhs.Name)) return false;
        var lhsAttrEnumer = node.Attributes().GetEnumerator();
        var rhsAttrEnumer = rhs.Attributes().GetEnumerator();
        while (true)
        {
            var lhsHas = lhsAttrEnumer.MoveNext();
            var rhsHas = rhsAttrEnumer.MoveNext();
            if (lhsHas != rhsHas) return false;
            if (!lhsHas) break;
            var lhsAttr = lhsAttrEnumer.Current;
            var rhsAttr = rhsAttrEnumer.Current;
            if (!lhsAttr.ContentEqual(rhsAttr)) return false;
        }
        foreach (var elem in node.Elements())
        {
            throw new NotImplementedException();
        }
        return true;
    }

    /*
     * Adapted from: http://www.undermyhat.org/blog/2009/08/tip-force-utf8-or-other-encoding-for-xmlwriter-with-stringbuilder/
     */
    public static void OverrideEncoding(this System.Xml.XmlWriter writer, Encoding encoding)
    {
        /*
            get the writer implementation property InnerWriter, which is
            of internal scope
        */
        PropertyInfo innerWriterPropInfo = writer.GetType().GetProperty(
            "InnerWriter",
            BindingFlags.NonPublic | BindingFlags.Instance)!;

        /*
            get the XmlEncodedRawTextWriter or the XmlEncodedRawTextWriterIndent
            which are the actual underlying XmlWriter objects that do the meat of the work
            when writing to a StringBuilder or StringWriter
        */
        System.Xml.XmlWriter innerWriter = (System.Xml.XmlWriter)innerWriterPropInfo.GetValue(writer, null)!;

        /*
            get the FieldInfo for the "encoding" field, which is a Protected field
            this is the actual field we need for our "overriding" technique
        */
        FieldInfo encodingFieldInfo = innerWriter.GetType().GetField(
            "encoding",
            BindingFlags.NonPublic | BindingFlags.Instance)!;

        /*
            assign the encoding we want, any encoding can be used here
        */
        encodingFieldInfo.SetValue(innerWriter, Encoding.UTF8);
    }
    /*
     * Attempt to make saving masses of XML faster by checking the disk if they changed before writing
     */
    public static void SaveIfChanged(this XElement elem, string path)
    {
        if (File.Exists(path))
        {
            MemoryStream data = new MemoryStream();
            elem.Save(data);
            data.Position = 0;

            File.SetLastAccessTime(path, DateTime.Now);

            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                if (fs.ContentsEqual(data)) return;
            }

            data.Position = 0;
            using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None, 4096, FileOptions.None))
            {
                data.CopyTo(fs);
            }
        }
        else
        {
            using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None, 4096, FileOptions.None))
            {
                elem.Save(fs);
            }

            File.SetLastAccessTime(path, DateTime.Now);
        }
    }
}