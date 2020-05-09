using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Noggog
{
    public class ExtendedList<T> : List<T>, IExtendedList<T>
    {
        public ExtendedList()
            : base()
        {
        }

        public ExtendedList(IEnumerable<T> collection)
            : base(collection)
        {
        }

        public void InsertRange(IEnumerable<T> collection, int index)
        {
            foreach (var item in collection.Reverse())
            {
                this.Insert(index, item);
            }
        }

        public void Move(int original, int destination)
        {
            var item = this[original];
            this.RemoveAt(original);
            this.Insert(destination, item);
        }
    }
}
