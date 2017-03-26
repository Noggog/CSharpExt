using System;

namespace Noggog.Notifying
{
    public struct ChangePoint<T>
    {
        public readonly T Old;
        public readonly T New;
        public readonly AddRemoveModify AddRem;
        public readonly P2Int Point;

        public ChangePoint(T old, T newVal, AddRemoveModify addRem, P2Int p)
        {
            this.Old = old;
            this.New = newVal;
            this.AddRem = addRem;
            this.Point = p;
        }
    }
}
