using System;

namespace Noggog.Notifying
{
    public class ChangeMod<T> : IEquatable<ChangeMod<T>>
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

        public bool Equals(ChangeMod<T> other)
        {
            return this.AddRem == other.AddRem
                && object.Equals(this.Old, other.Old)
                && object.Equals(this.New, other.New);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ChangeMod<T> rhs)) return false;
            return Equals(rhs);
        }

        public override int GetHashCode()
        {
            return AddRem.GetHashCode()
                .CombineHashCode(
                    HashHelper.GetHashCode(Old, New));
        }

        public override string ToString()
        {
            return $"({AddRem.ToStringFast_Enum_Only()}: {this.Old} => {this.New})";
        }
    }
}
