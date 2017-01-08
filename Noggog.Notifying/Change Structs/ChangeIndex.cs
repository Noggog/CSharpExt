using System;

namespace Noggog.Notifying
{
    public struct ChangeIndex<T>
    {
        public readonly T Old;
        public readonly T New;
        public readonly AddRemoveModify AddRem;
        public readonly int Index;

        public ChangeIndex(T oldItem, T item, AddRemoveModify addRem, int index)
        {
            this.Old = oldItem;
            this.New = item;
            this.AddRem = addRem;
            this.Index = index;
        }
    }
}
