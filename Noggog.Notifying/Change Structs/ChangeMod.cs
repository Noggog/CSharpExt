using System;

namespace Noggog.Notifying
{
    public struct ChangeMod<T>
    {
        public readonly T Old;
        public readonly T New;
        public readonly AddRemoveModify AddRem;

        public ChangeMod(T old, T item, AddRemoveModify addRem)
        {
            this.Old = old;
            this.New = item;
            this.AddRem = addRem;
        }
    }
}
