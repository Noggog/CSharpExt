using DynamicData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Noggog
{
    public class ExtendedList<T> : List<T>, IExtendedList<T>
    {
        public void InsertRange(IEnumerable<T> collection, int index)
        {
            foreach (var item in collection.Reverse())
            {
                this.Insert(index, item);
            }
        }

        public void Move(int original, int destination)
        {
            this[destination] = this[original];
            this[original] = default;
        }
    }
}
