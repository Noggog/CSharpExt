using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noggog
{
    // Poor internal implementation.  Functional API shell to be improved later.
    public class RangeCollection
    {
        List<RangeInt64> Ranges = new List<RangeInt64>();

        public RangeCollection()
        {
        }

        public RangeCollection(IEnumerable<RangeInt64> e)
        {
            this.Add(e);
        }

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

        public void Add(IEnumerable<RangeInt64> ranges)
        {
            foreach (var range in ranges)
            {
                this.Ranges.Add(range);
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
            foreach (var range in this.Ranges)
            {
                if (range.IsInRange(l)) return true;
            }
            return false;
        }

        public bool Collides(RangeInt64 range)
        {
            foreach (var curRange in this.Ranges)
            {
                if (range.Collides(curRange)) return true;
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
