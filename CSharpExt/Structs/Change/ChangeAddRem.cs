using System;

namespace Noggog.Notifying
{
    public struct ChangeAddRem<T>
    {
        public readonly T Item;
        public readonly AddRemove AddRem;

        public ChangeAddRem(T item, AddRemove addRem)
        {
            this.Item = item;
            this.AddRem = addRem;
        }
    }
}
