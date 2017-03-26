using System;

namespace Noggog.Notifying
{
    public struct Change<T>
    {
        public readonly T Old;
        public readonly T New;

        public Change(T newVal)
        {
            Old = default(T);
            New = newVal;
        }

        public Change(T oldVal, T newVal)
        {
            Old = oldVal;
            New = newVal;
        }
    }
}
