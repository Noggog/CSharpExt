using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noggog
{
    public class RangeCollection : IEnumerable<RangeInt64>
    {
        internal List<long> startingIndices = new List<long>();
        internal List<long> endingIndices = new List<long>();
        public bool Empty => startingIndices.Count == 0;

        public RangeCollection()
        {
        }

        public RangeCollection(IEnumerable<RangeInt64> e)
        {
            this.Add(e);
        }

        private void Add(long min, long max)
        {
            if (startingIndices.Count == 0)
            {
                startingIndices.Add(min);
                endingIndices.Add(max);
                return;
            }

            // ToDo
            // Can be improved by reusing binary search results that are being calculated twice
            // Or deriving later binary search results from previous similar ones

            bool shouldAddMin = !IsEncapsulated(min);
            bool shouldAddMax = !IsEncapsulated(max);

            // Remove indices inside our new added range
            if (min < max - 1
                && PreSortedListExt.TryGetEncapsulatedIndices(
                    startingIndices,
                    min + 1,
                    max - 1,
                    out var startingRange))
            {
                startingIndices.RemoveRange(startingRange.Min, startingRange.IntWidth);
            }

            if (min < max - 1
                && PreSortedListExt.TryGetEncapsulatedIndices(
                    endingIndices,
                    min + 1,
                    max - 1,
                    out var endingRange))
            {
                endingIndices.RemoveRange(endingRange.Min, endingRange.IntWidth);
            }

            if (shouldAddMin)
            {
                PreSortedListExt.Set(startingIndices, min);
            }
            if (shouldAddMax)
            {
                PreSortedListExt.Set(endingIndices, max);
            }
        }

        public void Add(RangeInt64 range)
        {
            this.Add(range.Min, range.Max);
        }

        public void Add(RangeInt8 range)
        {
            this.Add(range.Min, range.Max);
        }

        public void Add(RangeInt16 range)
        {
            this.Add(range.Min, range.Max);
        }

        public void Add(RangeInt32 range)
        {
            this.Add(range.Min, range.Max);
        }

        public void Add(RangeUInt8 range)
        {
            this.Add(range.Min, range.Max);
        }

        public void Add(RangeUInt16 range)
        {
            this.Add(range.Min, range.Max);
        }

        public void Add(RangeUInt32 range)
        {
            this.Add(range.Min, range.Max);
        }

        public void Add(IEnumerable<RangeInt64> ranges)
        {
            foreach (var range in ranges)
            {
                this.Add(range);
            }
        }

        public void Add(IEnumerable<RangeInt8> ranges)
        {
            this.Add(ranges.Select((r) => new RangeInt64(r.Min, r.Max)));
        }

        public void Add(IEnumerable<RangeInt16> ranges)
        {
            this.Add(ranges.Select((r) => new RangeInt64(r.Min, r.Max)));
        }

        public void Add(IEnumerable<RangeInt32> ranges)
        {
            this.Add(ranges.Select((r) => new RangeInt64(r.Min, r.Max)));
        }

        public void Add(IEnumerable<RangeUInt8> ranges)
        {
            this.Add(ranges.Select((r) => new RangeInt64(r.Min, r.Max)));
        }

        public void Add(IEnumerable<RangeUInt16> ranges)
        {
            this.Add(ranges.Select((r) => new RangeInt64(r.Min, r.Max)));
        }

        public void Add(IEnumerable<RangeUInt32> ranges)
        {
            this.Add(ranges.Select((r) => new RangeInt64(r.Min, r.Max)));
        }

        public bool IsEncapsulated(long l)
        {
            if (this.startingIndices.Count == 0) return false;
            // Find earlier start
            if (!PreSortedListExt.TryGetInDirection(
                this.startingIndices,
                l,
                higher: false,
                result: out var startResult))
            {
                return false;
            }
            // If start is our value, we are encapsulated
            if (startResult.Value == l) return true;

            // Find earlier end, if found is after our earlier start
            // we aren't encapsulated
            if (PreSortedListExt.TryGetInDirection(
                this.endingIndices,
                l,
                higher: false,
                result: out var endEarlierResult)
                && endEarlierResult.Value >= startResult.Value
                && endEarlierResult.Value != l)
            {
                return false;
            }
            return true;
        }

        public bool Collides(RangeInt64 range)
        {
            if (TryGetCurrentRange(range.Min, out var minRange))
            {
                return true;
            }
            else if (TryGetCurrentRange(range.Max, out var maxRange))
            {
                return true;
            }
            return false;
        }

        public bool TryGetCurrentRange(long l, out RangeInt64 range)
        {
            if (this.startingIndices.Count == 0)
            {
                range = default(RangeInt64);
                return false;
            }
            // Get start
            if (!PreSortedListExt.TryGetInDirection(
                this.startingIndices,
                item: l,
                higher: false,
                result: out var startingResult))
            {
                range = default(RangeInt64);
                return false;
            }

            // Get end
            if (!PreSortedListExt.TryGetInDirection(
                this.endingIndices,
                item: l,
                higher: true,
                result: out var endingResult))
            {
                range = default(RangeInt64);
                return false;
            }

            // Get start index after our located start index, confirm it's after our end, or we're not encapsulated
            if (startingIndices.Count > startingResult.Key + 1
                && startingIndices[startingResult.Key + 1] < endingResult.Value)
            {
                range = default(RangeInt64);
                return false;
            }

            range = new RangeInt64(startingResult.Value, endingResult.Value);
            return true;
        }

        public bool TryGetCurrentOrNextRange(long l, out RangeInt64 range)
        {
            if (this.startingIndices.Count == 0)
            {
                range = default(RangeInt64);
                return false;
            }

            // Get end
            if (!PreSortedListExt.TryGetInDirection(
                this.endingIndices,
                item: l,
                higher: true,
                result: out var endingResult))
            {
                range = default(RangeInt64);
                return false;
            }

            // Get earlier start, ensure earlier end comes before it
            if (PreSortedListExt.TryGetInDirection(
                this.startingIndices,
                item: l,
                higher: false,
                result: out var startingResult))
            {
                if (PreSortedListExt.TryGetInDirection(
                    this.endingIndices,
                    item: l,
                    higher: false,
                    result: out var earlierEndingResult))
                {
                    if (earlierEndingResult.Value < startingResult.Value)
                    {
                        range = new RangeInt64(startingResult.Value, endingResult.Value);
                        return true;
                    }
                }
                else
                {
                    range = new RangeInt64(startingResult.Value, endingResult.Value);
                    return true;
                }
            }

            // Get later start
            if (!PreSortedListExt.TryGetInDirection(
                this.startingIndices,
                item: l,
                higher: true,
                result: out startingResult))
            {
                range = default(RangeInt64);
                return false;
            }

            range = new RangeInt64(startingResult.Value, endingResult.Value);
            return true;
        }

        public IEnumerator<RangeInt64> GetEnumerator()
        {
            for (int i = 0; i < this.startingIndices.Count; i++)
            {
                yield return new RangeInt64(
                    this.startingIndices[i],
                    this.endingIndices[i]);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
