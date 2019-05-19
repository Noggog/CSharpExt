using Noggog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CSharpExt.Tests
{
    public class SortedListExt_Tests
    {
        public const int TOO_LOW = -44;
        public const int LOW = -11;
        public const int MEDIUM = 22;
        public const int HIGH = 77;
        public const int TYPICAL_NOT_EXISTS = 55;
        public const int TOO_HIGH = 100;

        private SortedList<int, int> TypicalSortedList()
        {
            var list = new SortedList<int, int>();
            list[LOW] = 2;
            list[MEDIUM] = 6;
            list[HIGH] = 1;
            return list;
        }

        #region TryGetIndexInDirection
        [Fact]
        public void TryGetIndexInDirection_Higher_Typical()
        {
            var list = TypicalSortedList();
            var got = list.TryGetIndexInDirection(
                key: TYPICAL_NOT_EXISTS,
                higher: true,
                result: out var result);
            Assert.True(got);
            Assert.Equal(2, result);
        }

        [Fact]
        public void TryGetIndexInDirection_Higher_Equal()
        {
            var list = TypicalSortedList();
            var got = list.TryGetIndexInDirection(
                key: MEDIUM,
                higher: true,
                result: out var result);
            Assert.True(got);
            Assert.Equal(1, result);
        }

        [Fact]
        public void TryGetIndexInDirection_Higher_None()
        {
            var list = TypicalSortedList();
            var got = list.TryGetIndexInDirection(
                key: 88,
                higher: true,
                result: out var result);
            Assert.False(got);
            Assert.Equal(-1, result);
        }

        [Fact]
        public void TryGetIndexInDirection_Higher_FromLowest()
        {
            var list = TypicalSortedList();
            var got = list.TryGetIndexInDirection(
                key: TOO_LOW,
                higher: true,
                result: out var result);
            Assert.True(got);
            Assert.Equal(0, result);
        }

        [Fact]
        public void TryGetIndexInDirection_Lower_Typical()
        {
            var list = TypicalSortedList();
            var got = list.TryGetIndexInDirection(
                key: TYPICAL_NOT_EXISTS,
                higher: false,
                result: out var result);
            Assert.True(got);
            Assert.Equal(1, result);
        }

        [Fact]
        public void TryGetIndexInDirection_Lower_Equal()
        {
            var list = TypicalSortedList();
            var got = list.TryGetIndexInDirection(
                key: MEDIUM,
                higher: false,
                result: out var result);
            Assert.True(got);
            Assert.Equal(1, result);
        }

        [Fact]
        public void TryGetIndexInDirection_Lower_None()
        {
            var list = TypicalSortedList();
            var got = list.TryGetIndexInDirection(
                key: TOO_LOW,
                higher: false,
                result: out var result);
            Assert.False(got);
            Assert.Equal(-1, result);
        }

        [Fact]
        public void TryGetIndexInDirection_Lower_FromHighest()
        {
            var list = TypicalSortedList();
            var got = list.TryGetIndexInDirection(
                key: TOO_HIGH,
                higher: false,
                result: out var result);
            Assert.True(got);
            Assert.Equal(2, result);
        }
        #endregion
        #region TryGetEncapsulatedIndices
        [Fact]
        public void TryGetEncapsulatedIndices_Typical()
        {
            var list = TypicalSortedList();
            var got = list.TryGetEncapsulatedIndices(
                lowerKey: MEDIUM,
                higherKey: TOO_HIGH,
                result: out var range);
            Assert.True(got);
            Assert.Equal(new RangeInt32(1, 2), range);
        }

        [Fact]
        public void TryGetEncapsulatedIndices_Above()
        {
            var list = TypicalSortedList();
            var got = list.TryGetEncapsulatedIndices(
                lowerKey: TOO_HIGH,
                higherKey: TOO_HIGH + 5,
                result: out var range);
            Assert.False(got);
        }

        [Fact]
        public void TryGetEncapsulatedIndices_Below()
        {
            var list = TypicalSortedList();
            var got = list.TryGetEncapsulatedIndices(
                lowerKey: TOO_LOW - 5,
                higherKey: TOO_LOW,
                result: out var range);
            Assert.False(got);
        }

        [Fact]
        public void TryGetEncapsulatedIndices_Crossing()
        {
            var list = TypicalSortedList();
            var got = list.TryGetEncapsulatedIndices(
                lowerKey: LOW + 1,
                higherKey: MEDIUM - 1,
                result: out var range);
            Assert.False(got);
        }
        #endregion
    }
}
