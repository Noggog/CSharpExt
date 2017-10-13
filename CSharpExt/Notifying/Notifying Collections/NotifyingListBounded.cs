using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noggog.Notifying
{
    public class NotifyingListBounded<T> : NotifyingList<T>
    {
        private int _MaxValue;
        public int MaxValue
        {
            get => _MaxValue;
            set => SetMaxValue(value);
        }

        public NotifyingListBounded()
        {
        }

        public NotifyingListBounded(int max)
        {
            this._MaxValue = max;
        }

        private void SetMaxValue(int max)
        {
            if (max < this.list.Count)
            {
                throw new ArgumentException($"Max was set on a list that was bigger than the allowed value {this.list.Count} > {max}");
            }
            this._MaxValue = max;
        }

        public override void Set(int index, T item, NotifyingFireParameters? cmds = null)
        {
            if (index == _MaxValue)
            {
                throw new ArgumentException($"Executed a set on a list that would make it bigger than the allowed value {index + 1} > {_MaxValue}");
            }
            base.Set(index, item, cmds);
        }

        public override void Insert(int index, T item, NotifyingFireParameters? cmds = null)
        {
            if (this.list.Count == _MaxValue)
            {
                throw new ArgumentException($"Executed an insert on a list that would make it bigger than the allowed value {this.list.Count + 1} > {_MaxValue}");
            }
            base.Insert(index, item, cmds);
        }

        public override void Add(T item, NotifyingFireParameters? cmds = null)
        {
            if (this.list.Count == _MaxValue)
            {
                throw new ArgumentException($"Executed an add on a list that would make it bigger than the allowed value {this.list.Count + 1} > {_MaxValue}");
            }
            base.Add(item, cmds);
        }

        public override void Add(IEnumerable<T> items, NotifyingFireParameters? cmds = null)
        {
            int count;
            if (items is ICollection<T> coll)
            {
                count = coll.Count;
            }
            else
            {
                count = items.Count();
            }
            if (this.list.Count == _MaxValue - count + 1)
            {
                throw new ArgumentException($"Executed an add on a list that would make it bigger than the allowed value {this.list.Count + count} > {_MaxValue}");
            }
            base.Add(items, cmds);
        }

        public override void SetTo(IEnumerable<T> enumer, NotifyingFireParameters? cmds = null)
        {
            int count;
            if (enumer is ICollection<T> coll)
            {
                count = coll.Count;
            }
            else
            {
                count = enumer.Count();
            }
            if (count > this._MaxValue)
            {
                throw new ArgumentException($"Executed a set on a list that would make it bigger than the allowed value {count} > {_MaxValue}");
            }
            base.SetTo(enumer, cmds);
        }
    }
}
