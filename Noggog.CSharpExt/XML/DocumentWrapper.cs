using System;
using System.Xml;

namespace Noggog.Xml
{
    public struct DocumentWrapper : IDisposable
    {
        XmlWriter writer;

        public DocumentWrapper(XmlWriter writer)
        {
            this.writer = writer;
            writer.WriteStartDocument();
        }

        public void Dispose()
        {
            writer.WriteEndDocument();
        }
    }
}
