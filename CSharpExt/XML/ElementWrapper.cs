using System;
using System.Xml;

namespace Noggog.Xml
{
    public struct ElementWrapper : IDisposable
    {
        XmlWriter writer;

        public ElementWrapper(XmlWriter writer, string elem, string nameSpace = null)
        {
            this.writer = writer;
            this.writer.WriteStartElement(elem, ns: nameSpace);
        }

        public void Dispose()
        {
            this.writer.WriteEndElement();
        }
    }
}
