using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Xunit;

namespace CSharpExt.Tests
{
    public class XmlExt_Tests
    {
        enum TestEnum
        {
            Value1,
            Value2
        }

        [Fact]
        public void TryGetAttribute()
        {
            XElement elem = new XElement("Test", new XAttribute("value", "true"));
            elem.TryGetAttribute<bool>("value", out var val);
            Assert.True(val);
        }

        [Fact]
        public void TryGetAttribute_Enum()
        {
            XElement elem = new XElement("Test", new XAttribute("value", "Value2"));
            elem.TryGetAttribute<TestEnum>("value", out var val);
            Assert.Equal(TestEnum.Value2, val);
        }
    }
}
