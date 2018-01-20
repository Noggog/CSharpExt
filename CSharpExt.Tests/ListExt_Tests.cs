using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CSharpExt.Tests
{
    public class ListExt_Tests
    {
        public const int TOO_LOW = -44;
        public const int LOW = -11;
        public const int MEDIUM = 22;
        public const int HIGH = 77;
        public const int TYPICAL_NOT_EXISTS = 55;
        public const int TOO_HIGH = 100;

        private IList<int> TypicalSortedList()
        {
            var list = new List<int>();
            list.Add(LOW);
            list.Add(MEDIUM);
            list.Add(HIGH);
            return list;
        }

        #region BinarySearch
        [Fact]
        public void BinarySearch_Typical()
        {
            var list = TypicalSortedList();
            var result = list.BinarySearch(TYPICAL_NOT_EXISTS);
            Assert.Equal(~2, result);
        }

        [Fact]
        public void BinarySearch_Equal()
        {
            var list = TypicalSortedList();
            var result = list.BinarySearch(MEDIUM);
            Assert.Equal(1, result);
        }

        [Fact]
        public void BinarySearch_None()
        {
            var list = TypicalSortedList();
            var result = list.BinarySearch(TOO_HIGH);
            Assert.Equal(~3, result);
        }

        [Fact]
        public void BinarySearch_FromLowest()
        {
            var list = TypicalSortedList();
            var result = list.BinarySearch(TOO_LOW);
            Assert.Equal(~0, result);
        }
        #endregion
    }
}
