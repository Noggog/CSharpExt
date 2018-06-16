using Noggog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CSharpExt.Tests
{
    public class SortingList_Tests
    {
        public const int TypicalCount = 3;
        public const int LowIndex = 0;
        public const int MiddleIndex = 1;
        public const int HighIndex = 2;
        public static WrappedInt LowItem => new WrappedInt(-3);
        public static WrappedInt MiddleItem => new WrappedInt(6);
        public static WrappedInt HighItem => new WrappedInt(155);
        public static WrappedInt MissingMiddleItem => new WrappedInt(8);
        public static WrappedInt MissingLowItem => new WrappedInt(-1000);
        public static WrappedInt MissingHighItem => new WrappedInt(1000);

        #region Helper Class
        public class WrappedInt : IComparable, IComparable<WrappedInt>, IEquatable<WrappedInt>
        {
            public readonly int Int;

            public WrappedInt(int i)
            {
                this.Int = i;
            }

            public int CompareTo(WrappedInt other)
            {
                return Int.CompareTo(other.Int);
            }

            public int CompareTo(object obj)
            {
                if (!(obj is WrappedInt rhs)) return 0;
                return CompareTo(rhs);
            }

            public override bool Equals(object obj)
            {
                if (!(obj is WrappedInt rhs)) return false;
                return Equals(rhs);
            }

            public override int GetHashCode()
            {
                return Int.GetHashCode();
            }

            public bool Equals(WrappedInt other)
            {
                return Int == other.Int;
            }
        }
        #endregion

        public List<WrappedInt> TypicalBareList()
        {
            return new List<WrappedInt>()
            {
                LowItem,
                MiddleItem,
                HighItem
            };
        }

        public SortingList<WrappedInt> Typical()
        {
            return SortingList<WrappedInt>.Factory_Wrap_AssumeSorted(TypicalBareList());
        }

        #region Factories
        [Fact]
        public void Factory_Wrap_AssumeSorted()
        {
            var list = SortingList<WrappedInt>.Factory_Wrap_AssumeSorted(TypicalBareList());
            Assert.Equal(TypicalCount, list.Count);
            Assert.True(TypicalBareList().SequenceEqual(list));
        }

        [Fact]
        public void Factory_Wrap_AssumeSorted_But_Not()
        {
            var badInts = new int[] { 4, 2, 5 };
            var list = SortingList<int>.Factory_Wrap_AssumeSorted(
                new List<int>(badInts));
            Assert.Equal(badInts.Length, list.Count);
            Assert.True(badInts.SequenceEqual(list));
        }

        [Fact]
        public void Factory_Wrap_Sort()
        {
            var list = SortingList<WrappedInt>.Factory_Wrap_Sort(TypicalBareList());
            Assert.Equal(TypicalCount, list.Count);
            Assert.True(TypicalBareList().SequenceEqual(list));
        }

        [Fact]
        public void Factory_Wrap_Sort_WasNeeded()
        {
            var badInts = new int[] { 4, 2, 5 };
            var sortedInts = new int[] { 2, 4, 5 };
            var list = SortingList<int>.Factory_Wrap_Sort(
                new List<int>(badInts));
            Assert.Equal(badInts.Length, list.Count);
            Assert.True(sortedInts.SequenceEqual(list));
        }
        #endregion

        #region Adds
        #region Add ReplaceIfMatch
        #region True
        #region Missing
        [Fact]
        public void Add_ReplaceIfMatch_Missing()
        {
            var list = Typical();
            Assert.False(list.Add(MissingMiddleItem, replaceIfMatch: true));
            Assert.Equal(TypicalCount + 1, list.Count);
            Assert.True(
                TypicalBareList().And(MissingMiddleItem).OrderBy((i => i))
                .SequenceEqual(list));
        }

        [Fact]
        public void Add_ReplaceIfMatch_MissingLow()
        {
            var list = Typical();
            Assert.False(list.Add(MissingLowItem, replaceIfMatch: true));
            Assert.Equal(TypicalCount + 1, list.Count);
            Assert.True(
                TypicalBareList().And(MissingLowItem).OrderBy((i => i))
                .SequenceEqual(list));
        }

        [Fact]
        public void Add_ReplaceIfMatch_MissingHigh()
        {
            var list = Typical();
            Assert.False(list.Add(MissingHighItem, replaceIfMatch: true));
            Assert.Equal(TypicalCount + 1, list.Count);
            Assert.True(
                TypicalBareList().And(MissingHighItem).OrderBy((i => i))
                .SequenceEqual(list));
        }
        #endregion
        #region Collide
        [Fact]
        public void Add_ReplaceIfMatch_Collide()
        {
            var list = Typical();
            Assert.True(list.Add(MiddleItem, replaceIfMatch: true));
            Assert.Equal(TypicalCount, list.Count);
            Assert.True(TypicalBareList().SequenceEqual(list));
        }

        [Fact]
        public void Add_ReplaceIfMatch_CollideLow()
        {
            var list = Typical();
            Assert.True(list.Add(LowItem, replaceIfMatch: true));
            Assert.Equal(TypicalCount, list.Count);
            Assert.True(TypicalBareList().SequenceEqual(list));
        }

        [Fact]
        public void Add_ReplaceIfMatch_CollideHigh()
        {
            var list = Typical();
            Assert.True(list.Add(HighItem, replaceIfMatch: true));
            Assert.Equal(TypicalCount, list.Count);
            Assert.True(TypicalBareList().SequenceEqual(list));
        }
        #endregion
        #endregion
        #region False
        #region Missing
        [Fact]
        public void Add_DontReplaceIfMatch_Missing()
        {
            var list = Typical();
            Assert.False(list.Add(MissingMiddleItem, replaceIfMatch: false));
            Assert.Equal(TypicalCount + 1, list.Count);
            Assert.True(
                TypicalBareList().And(MissingMiddleItem).OrderBy((i => i))
                .SequenceEqual(list));
        }

        [Fact]
        public void Add_DontReplaceIfMatch_MissingLow()
        {
            var list = Typical();
            Assert.False(list.Add(MissingLowItem, replaceIfMatch: false));
            Assert.Equal(TypicalCount + 1, list.Count);
            Assert.True(
                TypicalBareList().And(MissingLowItem).OrderBy((i => i))
                .SequenceEqual(list));
        }

        [Fact]
        public void Add_DontReplaceIfMatch_MissingHigh()
        {
            var list = Typical();
            Assert.False(list.Add(MissingHighItem, replaceIfMatch: false));
            Assert.Equal(TypicalCount + 1, list.Count);
            Assert.True(
                TypicalBareList().And(MissingHighItem).OrderBy((i => i))
                .SequenceEqual(list));
        }
        #endregion
        #region Collide
        [Fact]
        public void Add_DontReplaceIfMatch_Collide()
        {
            var list = Typical();
            var item = MiddleItem;
            Assert.True(list.Add(item, replaceIfMatch: false));
            Assert.Equal(TypicalCount, list.Count);
            Assert.True(TypicalBareList().SequenceEqual(list));
            Assert.NotSame(item, list[MiddleIndex]);
        }

        [Fact]
        public void Add_DontReplaceIfMatch_CollideLow()
        {
            var list = Typical();
            var item = LowItem;
            Assert.True(list.Add(item, replaceIfMatch: false));
            Assert.Equal(TypicalCount, list.Count);
            Assert.True(TypicalBareList().SequenceEqual(list));
            Assert.NotSame(item, list[LowIndex]);
        }

        [Fact]
        public void Add_DontReplaceIfMatch_CollideHigh()
        {
            var list = Typical();
            var item = HighItem;
            Assert.True(list.Add(item, replaceIfMatch: false));
            Assert.Equal(TypicalCount, list.Count);
            Assert.True(TypicalBareList().SequenceEqual(list));
            Assert.NotSame(item, list[HighIndex]);
        }
        #endregion
        #endregion
        #endregion
        #region Add
        #region Missing
        [Fact]
        public void Add_Missing()
        {
            var list = Typical();
            list.Add(MissingMiddleItem);
            Assert.Equal(TypicalCount + 1, list.Count);
            Assert.True(
                TypicalBareList().And(MissingMiddleItem).OrderBy((i => i))
                .SequenceEqual(list));
        }

        [Fact]
        public void Add_MissingLow()
        {
            var list = Typical();
            list.Add(MissingLowItem);
            Assert.Equal(TypicalCount + 1, list.Count);
            Assert.True(
                TypicalBareList().And(MissingLowItem).OrderBy((i => i))
                .SequenceEqual(list));
        }

        [Fact]
        public void Add_MissingHigh()
        {
            var list = Typical();
            list.Add(MissingHighItem);
            Assert.Equal(TypicalCount + 1, list.Count);
            Assert.True(
                TypicalBareList().And(MissingHighItem).OrderBy((i => i))
                .SequenceEqual(list));
        }
        #endregion
        #region Collide
        [Fact]
        public void Add_Collide()
        {
            var list = Typical();
            list.Add(MiddleItem);
            Assert.Equal(TypicalCount, list.Count);
            Assert.True(TypicalBareList().SequenceEqual(list));
        }

        [Fact]
        public void Add_CollideLow()
        {
            var list = Typical();
            list.Add(LowItem);
            Assert.Equal(TypicalCount, list.Count);
            Assert.True(TypicalBareList().SequenceEqual(list));
        }

        [Fact]
        public void Add_CollideHigh()
        {
            var list = Typical();
            list.Add(HighItem);
            Assert.Equal(TypicalCount, list.Count);
            Assert.True(TypicalBareList().SequenceEqual(list));
        }
        #endregion
        #endregion
        #endregion

        #region Misc
        [Fact]
        public void Empty()
        {
            var list = new SortingList<WrappedInt>();
            Assert.Empty(list);
        }

        [Fact]
        public void Count()
        {
            var list = Typical();
            Assert.Equal(TypicalCount, list.Count);
        }

        [Fact]
        public void Clear_Empty()
        {
            var list = new SortingList<WrappedInt>();
            list.Clear();
            Assert.Empty(list);
        }

        [Fact]
        public void Clear()
        {
            var list = Typical();
            list.Clear();
            Assert.Empty(list);
        }
        #endregion

        #region IndexOf
        [Fact]
        public void IndexOf_Empty()
        {
            var list = new SortingList<int>();
            Assert.Equal(-1, list.IndexOf(4));
        }

        [Fact]
        public void IndexOf()
        {
            var list = Typical();
            Assert.Equal(MiddleIndex, list.IndexOf(MiddleItem));
        }

        [Fact]
        public void IndexOf_Missing()
        {
            var list = Typical();
            Assert.Equal(-1, list.IndexOf(MissingMiddleItem));
        }
        #endregion

        #region Contains
        [Fact]
        public void Contains_Empty()
        {
            var list = new SortingList<int>();
            Assert.DoesNotContain(4, list);
        }

        [Fact]
        public void Contains()
        {
            var list = Typical();
            Assert.Contains(MiddleItem, list);
        }

        [Fact]
        public void Contains_Missing()
        {
            var list = Typical();
            Assert.Equal(-1, list.IndexOf(MissingMiddleItem));
        }
        #endregion

        #region Removes
        #region RemoveAt
        [Fact]
        public void RemoveAt()
        {
            var list = Typical();
            list.RemoveAt(MiddleIndex);
            Assert.Equal(TypicalCount - 1, list.Count);
            Assert.Equal(LowItem, list[0]);
            Assert.Equal(HighItem, list[1]);
        }

        [Fact]
        public void Remove()
        {
            var list = Typical();
            Assert.True(list.Remove(MiddleItem));
            Assert.Equal(TypicalCount - 1, list.Count);
            Assert.Equal(LowItem, list[0]);
            Assert.Equal(HighItem, list[1]);
        }

        [Fact]
        public void Remove_Missing()
        {
            var list = Typical();
            Assert.False(list.Remove(MissingMiddleItem));
        }
        #endregion
        #endregion

        #region Insert
        [Fact]
        public void Insert()
        {
            var list = Typical();
            Assert.Throws<NotImplementedException>(() => ((IList<WrappedInt>)list).Insert(1, MiddleItem));
        }
        #endregion
    }
}
