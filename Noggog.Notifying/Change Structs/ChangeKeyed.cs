using System;

namespace Noggog.Notifying
{
    public struct ChangeKeyed<K, V>
    {
        public readonly V Old;
        public readonly V New;
        public readonly AddRemoveModify AddRem;
        public readonly K Key;

        public ChangeKeyed(K k, V old, V newVal, AddRemoveModify addRem)
        {
            this.Old = old;
            this.New = newVal;
            this.AddRem = addRem;
            this.Key = k;
        }
    }
}
