using Noggog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CSharpExt.Tests
{
    public class RangeCollection_Tests
    {
        public RangeCollection Typical_Sparse()
        {
            var ret = new RangeCollection();
            ret.Add(new RangeInt64(-13, -7));
            ret.Add(new RangeInt64(-3, 3));
            ret.Add(new RangeInt64(7, 13));
            return ret;
        }

        public RangeCollection Typical_Overlap()
        {
            var ret = new RangeCollection();
            ret.Add(new RangeInt64(3, 7));
            ret.Add(new RangeInt64(5, 10));
            return ret;
        }

        #region Add
        [Fact]
        public void AddSparse()
        {
            var coll = Typical_Sparse();
            Assert.Equal(3, coll.startingIndices.Count);
            Assert.Equal(3, coll.endingIndices.Count);
            Assert.Equal(-13, coll.startingIndices[0]);
            Assert.Equal(-3, coll.startingIndices[1]);
            Assert.Equal(7, coll.startingIndices[2]);
            Assert.Equal(-7, coll.endingIndices[0]);
            Assert.Equal(3, coll.endingIndices[1]);
            Assert.Equal(13, coll.endingIndices[2]);
        }

        [Fact]
        public void AddOverlap()
        {
            var coll = Typical_Overlap();
            Assert.Single(coll.startingIndices);
            Assert.Single(coll.endingIndices);
            Assert.Equal(3, coll.startingIndices[0]);
            Assert.Equal(10, coll.endingIndices[0]);
        }

        [Fact]
        public void AddOverlapWrappingSparses()
        {
            var coll = Typical_Sparse();
            coll.Add(new RangeInt64(2, 10));
            Assert.Equal(2, coll.startingIndices.Count);
            Assert.Equal(2, coll.endingIndices.Count);
            Assert.Equal(-13, coll.startingIndices[0]);
            Assert.Equal(-3, coll.startingIndices[1]);
            Assert.Equal(-7, coll.endingIndices[0]);
            Assert.Equal(13, coll.endingIndices[1]);
        }

        [Fact]
        public void AddOverlapWrappingSparses_PastEnd()
        {
            var coll = Typical_Sparse();
            coll.Add(new RangeInt64(2, 16));
            Assert.Equal(2, coll.startingIndices.Count);
            Assert.Equal(2, coll.endingIndices.Count);
            Assert.Equal(-13, coll.startingIndices[0]);
            Assert.Equal(-3, coll.startingIndices[1]);
            Assert.Equal(-7, coll.endingIndices[0]);
            Assert.Equal(16, coll.endingIndices[1]);
        }

        [Fact]
        public void AddOverlapWrappingSparses_ToEnd()
        {
            var coll = Typical_Sparse();
            coll.Add(new RangeInt64(2, 13));
            Assert.Equal(2, coll.startingIndices.Count);
            Assert.Equal(2, coll.endingIndices.Count);
            Assert.Equal(-13, coll.startingIndices[0]);
            Assert.Equal(-3, coll.startingIndices[1]);
            Assert.Equal(-7, coll.endingIndices[0]);
            Assert.Equal(13, coll.endingIndices[1]);
        }

        [Fact]
        public void AddOverlapWrappingSparses_PastStart()
        {
            var coll = Typical_Sparse();
            coll.Add(new RangeInt64(-15, 2));
            Assert.Equal(2, coll.startingIndices.Count);
            Assert.Equal(2, coll.endingIndices.Count);
            Assert.Equal(-15, coll.startingIndices[0]);
            Assert.Equal(7, coll.startingIndices[1]);
            Assert.Equal(3, coll.endingIndices[0]);
            Assert.Equal(13, coll.endingIndices[1]);
        }

        [Fact]
        public void AddOverlapWrappingSparses_ToStart()
        {
            var coll = Typical_Sparse();
            coll.Add(new RangeInt64(-13, 2));
            Assert.Equal(2, coll.startingIndices.Count);
            Assert.Equal(2, coll.endingIndices.Count);
            Assert.Equal(-13, coll.startingIndices[0]);
            Assert.Equal(7, coll.startingIndices[1]);
            Assert.Equal(3, coll.endingIndices[0]);
            Assert.Equal(13, coll.endingIndices[1]);
        }
        #endregion

        #region IsEncapsulated
        #region Empty
        [Fact]
        public void IsEncapsulated_Empty()
        {
            var coll = new RangeCollection();
            Assert.False(coll.IsEncapsulated(5));
        }

        [Fact]
        public void IsEncapsulated_EmptyZero()
        {
            var coll = new RangeCollection();
            Assert.False(coll.IsEncapsulated(0));
        }

        [Fact]
        public void IsEncapsulated_EmptyNegative()
        {
            var coll = new RangeCollection();
            Assert.False(coll.IsEncapsulated(-5));
        }
        #endregion

        #region Single
        [Fact]
        public void IsEncapsulated_Single_Pass()
        {
            var coll = Typical_Sparse();
            Assert.True(coll.IsEncapsulated(10));
        }

        [Fact]
        public void IsEncapsulated_SingleZero_Pass()
        {
            var coll = Typical_Sparse();
            Assert.True(coll.IsEncapsulated(0));
        }

        [Fact]
        public void IsEncapsulated_SingleNegative_Pass()
        {
            var coll = Typical_Sparse();
            Assert.True(coll.IsEncapsulated(-10));
        }

        [Fact]
        public void IsEncapsulated_Fail()
        {
            var coll = Typical_Sparse();
            Assert.False(coll.IsEncapsulated(5));
        }

        [Fact]
        public void IsEncapsulated_Negative_Fail()
        {
            var coll = Typical_Sparse();
            Assert.False(coll.IsEncapsulated(-5));
        }
        #endregion

        #region Overlap
        [Fact]
        public void IsEncapsulated_Overlapped_Overlap_Pass()
        {
            var coll = Typical_Overlap();
            Assert.True(coll.IsEncapsulated(6));
        }

        [Fact]
        public void IsEncapsulated_Overlapped_Overlap_Early_Edge_Pass()
        {
            var coll = Typical_Overlap();
            Assert.True(coll.IsEncapsulated(5));
        }

        [Fact]
        public void IsEncapsulated_Overlapped_Overlap_Later_Edge_Pass()
        {
            var coll = Typical_Overlap();
            Assert.True(coll.IsEncapsulated(7));
        }

        [Fact]
        public void IsEncapsulated_Overlapped_Early_Pass()
        {
            var coll = Typical_Overlap();
            Assert.True(coll.IsEncapsulated(4));
        }

        [Fact]
        public void IsEncapsulated_Overlapped_Early_Edge_Pass()
        {
            var coll = Typical_Overlap();
            Assert.True(coll.IsEncapsulated(3));
        }

        [Fact]
        public void IsEncapsulated_Overlapped_Early_Edge_Fail()
        {
            var coll = Typical_Overlap();
            Assert.False(coll.IsEncapsulated(2));
        }

        [Fact]
        public void IsEncapsulated_Overlapped_Later_Pass()
        {
            var coll = Typical_Overlap();
            Assert.True(coll.IsEncapsulated(9));
        }

        [Fact]
        public void IsEncapsulated_Overlapped_Later_Edge_Pass()
        {
            var coll = Typical_Overlap();
            Assert.True(coll.IsEncapsulated(10));
        }

        [Fact]
        public void IsEncapsulated_Overlapped_Later_Edge_Fail()
        {
            var coll = Typical_Overlap();
            Assert.False(coll.IsEncapsulated(11));
        }

        [Fact]
        public void IsEncapsulated_Overlapped_Fail()
        {
            var coll = Typical_Overlap();
            Assert.False(coll.IsEncapsulated(-5));
        }
        #endregion
        #endregion

        #region Collides
        #region Empty
        [Fact]
        public void Collides_Empty()
        {
            var coll = new RangeCollection();
            Assert.False(coll.Collides(new RangeInt64(3, 6)));
        }
        #endregion

        #region Single
        [Fact]
        public void Collides_Encompassed()
        {
            var coll = Typical_Sparse();
            Assert.True(coll.Collides(new RangeInt64(-2, 2)));
        }

        [Fact]
        public void Collides_RightEdge()
        {
            var coll = Typical_Sparse();
            Assert.True(coll.Collides(new RangeInt64(3, 6)));
        }

        [Fact]
        public void Collides_LeftEdge()
        {
            var coll = Typical_Sparse();
            Assert.True(coll.Collides(new RangeInt64(-6, -3)));
        }
        #endregion

        #region Overlap
        [Fact]
        public void Collides_Overlap_Encompased_Both()
        {
            var coll = Typical_Overlap();
            Assert.True(coll.Collides(new RangeInt64(6, 6)));
        }

        [Fact]
        public void Collides_Overlap_Encompassed_LeftOverlap()
        {
            var coll = Typical_Overlap();
            Assert.True(coll.Collides(new RangeInt64(4, 6)));
        }

        [Fact]
        public void Collides_Overlap_Encompassed_RightOverlap()
        {
            var coll = Typical_Overlap();
            Assert.True(coll.Collides(new RangeInt64(6, 8)));
        }

        [Fact]
        public void Collides_Overlap_LeftEdge()
        {
            var coll = Typical_Overlap();
            Assert.True(coll.Collides(new RangeInt64(1, 3)));
        }

        [Fact]
        public void Collides_Overlap_RightEdge()
        {
            var coll = Typical_Overlap();
            Assert.True(coll.Collides(new RangeInt64(10, 12)));
        }

        [Fact]
        public void Collides_Overlap_Fail()
        {
            var coll = Typical_Overlap();
            Assert.False(coll.Collides(new RangeInt64(1, 2)));
        }
        #endregion
        #endregion

        #region TryGetCurrentRange
        [Fact]
        public void TryGetCurrentRange_Empty()
        {
            var coll = new RangeCollection();
            Assert.False(coll.TryGetCurrentRange(3, out var range));
        }

        [Fact]
        public void TryGetCurrentRange()
        {
            var coll = Typical_Sparse();
            Assert.True(coll.TryGetCurrentRange(1, out var range));
            Assert.Equal(-3, range.Min);
            Assert.Equal(3, range.Max);
        }

        [Fact]
        public void TryGetCurrentRange_Fail()
        {
            var coll = Typical_Sparse();
            Assert.False(coll.TryGetCurrentRange(4, out var range));
        }

        [Fact]
        public void TryGetCurrentRange_End()
        {
            var coll = Typical_Sparse();
            Assert.False(coll.TryGetCurrentRange(114, out var range));
        }

        [Fact]
        public void TryGetCurrentRange_Start()
        {
            var coll = Typical_Sparse();
            Assert.False(coll.TryGetCurrentRange(-114, out var range));
        }

        [Fact]
        public void TryGetCurrentOverlap()
        {
            var coll = Typical_Overlap();
            Assert.True(coll.TryGetCurrentRange(8, out var range));
            Assert.Equal(3, range.Min);
            Assert.Equal(10, range.Max);
        }
        #endregion

        #region TryGetCurrentOrNextRange
        [Fact]
        public void TryGetCurrentOrNextRange_Empty()
        {
            var coll = new RangeCollection();
            Assert.False(coll.TryGetCurrentOrNextRange(3, out var range));
        }

        [Fact]
        public void TryGetCurrentOrNextRange_Inside()
        {
            var coll = Typical_Sparse();
            Assert.True(coll.TryGetCurrentOrNextRange(1, out var range));
            Assert.Equal(-3, range.Min);
            Assert.Equal(3, range.Max);
        }

        [Fact]
        public void TryGetCurrentOrNextRange_BetweenRanges()
        {
            var coll = Typical_Sparse();
            Assert.True(coll.TryGetCurrentOrNextRange(4, out var range));
            Assert.Equal(7, range.Min);
            Assert.Equal(13, range.Max);
        }

        [Fact]
        public void TryGetCurrentOrNextRange_End()
        {
            var coll = Typical_Sparse();
            Assert.False(coll.TryGetCurrentOrNextRange(114, out var range));
        }

        [Fact]
        public void TryGetCurrentOrNextRange_Start()
        {
            var coll = Typical_Sparse();
            Assert.True(coll.TryGetCurrentOrNextRange(-114, out var range));
            Assert.Equal(-13, range.Min);
            Assert.Equal(-7, range.Max);
        }

        [Fact]
        public void TryGetCurrentOrNextRange()
        {
            var coll = Typical_Overlap();
            Assert.True(coll.TryGetCurrentOrNextRange(8, out var range));
            Assert.Equal(3, range.Min);
            Assert.Equal(10, range.Max);
        }
        #endregion
    }
}
