using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noggog
{
    public class RangeCollection
    {
        List<RangeInt64> Ranges = new List<RangeInt64>();

        public void Add(RangeInt64 range)
        {
            this.Ranges.Add(range);
        }

        public void Add(RangeInt8 range)
        {
            this.Add(
                new RangeInt64(range.Min, range.Max));
        }

        public void Add(RangeInt16 range)
        {
            this.Add(
                new RangeInt64(range.Min, range.Max));
        }

        public void Add(RangeInt32 range)
        {
            this.Add(
                new RangeInt64(range.Min, range.Max));
        }

        public void Add(RangeUInt8 range)
        {
            this.Add(
                new RangeInt64(range.Min, range.Max));
        }

        public void Add(RangeUInt16 range)
        {
            this.Add(
                new RangeInt64(range.Min, range.Max));
        }

        public void Add(RangeUInt32 range)
        {
            this.Add(
                new RangeInt64(range.Min, range.Max));
        }

        public bool IsEncapsulated(long l)
        {
            foreach (var range in this.Ranges)
            {
                if (range.IsInRange(l)) return true;
            }
            return false;
        }

        public bool TryGetCurrentRange(long l, out RangeInt64 range)
        {
            RangeInt64? target = null;
            foreach (var r in this.Ranges)
            {
                if (r.IsInRange(l))
                {
                    if (target.HasValue)
                    {
                        throw new NotImplementedException("Needs to be upgraded to support overlapping ranges.");
                    }
                    target = r;
                }
            }
            if (target.HasValue)
            {
                range = target.Value;
                return true;
            }
            range = default(RangeInt64);
            return false;
        }
    }
}
