using System;
using System.Xml;

namespace Noggog.Xml
{
    public struct ElementWrapper : IDisposable
    {
        XmlWriter writer;

        public ElementWrapper(XmlWriter writer, string elem)
        {
            this.writer = writer;
            this.writer.WriteStartElement(elem);
        }

        public void Dispose()
        {
            this.writer.WriteEndElement();
        }
    }
}
