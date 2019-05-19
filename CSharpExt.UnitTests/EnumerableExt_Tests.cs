using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CSharpExt.Tests
{
    public class EnumerableExt_Tests
    {
        public IEnumerable<int> TypicalEnumer()
        {
            yield return 5;
            yield return -4;
            yield return 94;
            yield return 8;
        }

        #region CountGreaterThan
        [Fact]
        public void CountGreaterThan_TypicalPass()
        {
            Assert.True(TypicalEnumer().CountGreaterThan(2));
        }

        [Fact]
        public void CountGreaterThan_TypicalFail()
        {
            Assert.False(TypicalEnumer().CountGreaterThan(6));
        }

        [Fact]
        public void CountGreaterThan_EdgeFail()
        {
            Assert.False(TypicalEnumer().CountGreaterThan(4));
        }

        [Fact]
        public void CountGreaterThan_Zero()
        {
            Assert.True(TypicalEnumer().CountGreaterThan(0));
        }

        [Fact]
        public void CountGreaterThan_EmptyTypical()
        {
            Assert.False(EnumerableExt<int>.Empty.CountGreaterThan(2));
        }

        [Fact]
        public void CountGreaterThan_EmptyZero()
        {
            Assert.False(EnumerableExt<int>.Empty.CountGreaterThan(0));
        }
        #endregion
    }
}
