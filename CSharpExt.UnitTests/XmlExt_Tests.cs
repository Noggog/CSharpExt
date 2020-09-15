using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Xunit;
using Noggog;

namespace CSharpExt.UnitTests
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

        [Fact]
        public void GetAttribute_Nullable()
        {
            XElement elem = new XElement("Test");
            var ret = elem.GetAttribute<int?>("value", default(int?));
            Assert.Null(ret);
            elem = new XElement("Test", new XAttribute("value", "123"));
            ret = elem.GetAttribute<int?>("value", default(int?));
            Assert.Equal(123, ret);
        }
    }
}
