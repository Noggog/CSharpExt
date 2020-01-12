using System;

namespace Noggog
{
    public class ChangeKeyed<K, V> : IEquatable<ChangeKeyed<K, V>>
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

        public bool Equals(ChangeKeyed<K, V> other)
        {
            return this.AddRem == other.AddRem
                && object.Equals(this.Key, other.Key);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ChangeKeyed<K, V> rhs)) return false;
            return Equals(rhs);
        }

        public override int GetHashCode()
        {
            return this.AddRem.GetHashCode()
                .CombineHashCode(HashHelper.GetHashCode(this.Key));
        }

        public override string ToString()
        {
            return $"({this.AddRem.ToStringFast_Enum_Only()}: {this.Key}, {this.Old} => {this.New})";
        }
    }
}
