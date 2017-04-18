using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml.Linq;

namespace System
{
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

        public static bool TryGetAttribute(this XElement node, string str, out XAttribute val)
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
            if (TryGetAttribute(node, str, out XAttribute attr))
            {
                val = attr.Value;
                return true;
            }
            val = null;
            return false;
        }

        public static bool TryGetAttribute<P>(this XElement node, string str, out P val, Func<string, P> converter)
        {
            bool ret = TryGetAttributeString(node, str, out string strVal);
            val = converter(strVal);
            return ret;
        }

        public static bool TryGetAttribute<P>(this XElement node, string str, out P val, bool throwException = false)
        {
            return TryGetAttribute<P>(node, str, out val, (strVal) =>
            {
                if (strVal == null)
                {
                    return default(P);
                }
                try
                {
                    return (P)Convert.ChangeType(strVal, typeof(P));
                }
                catch (Exception ex)
                {
                    try
                    {
                        return (P)Enum.Parse(typeof(P), strVal);
                    }
                    catch (Exception)
                    {
                        if (throwException)
                        {
                            throw ex;
                        }
                        else
                        {
                            return default(P);
                        }
                    }
                }
            });
        }

        public static P GetAttributeCustom<P>(this XElement node, string str, Func<string, P> converter)
        {
            TryGetAttribute(node, str, out P val, converter);
            return val;
        }

        public static P GetAttribute<P>(this XElement node, string str, P defaultVal = default(P), bool throwException = false)
        {
            if (!TryGetAttribute(node, str, out P val, throwException))
            {
                val = defaultVal;
            }
            return val;
        }

        public static string GetAttribute(this XElement node, string str, string defaultVal = null, bool throwException = false)
        {
            if (!TryGetAttribute(node, str, out string val, throwException))
            {
                val = defaultVal;
            }
            return val;
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
                BindingFlags.NonPublic | BindingFlags.Instance);

            /*
                get the XmlEncodedRawTextWriter or the XmlEncodedRawTextWriterIndent
                which are the actual underlying XmlWriter objects that do the meat of the work
                when writing to a StringBuilder or StringWriter
            */
            System.Xml.XmlWriter innerWriter = (System.Xml.XmlWriter)innerWriterPropInfo.GetValue(writer, null);

            /*
                get the FieldInfo for the "encoding" field, which is a Protected field
                this is the actual field we need for our "overriding" technique
            */
            FieldInfo encodingFieldInfo = innerWriter.GetType().GetField(
                "encoding",
                BindingFlags.NonPublic | BindingFlags.Instance);

            /*
                assign the encoding we want, any encoding can be used here
            */
            encodingFieldInfo.SetValue(innerWriter, Encoding.UTF8);
        }
    }
}
