using System;

namespace Noggog.Notifying
{
    public class ChangeAddRem<T> : IEquatable<ChangeAddRem<T>>
    {
        public readonly T Item;
        public readonly AddRemove AddRem;

        public ChangeAddRem(T item, AddRemove addRem)
        {
            this.Item = item;
            this.AddRem = addRem;
        }

        public bool Equals(ChangeAddRem<T> other)
        {
            return this.AddRem == other.AddRem
                && object.Equals(this.Item, other.Item);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ChangeAddRem<T> rhs)) return false;
            return Equals(rhs);
        }

        public override int GetHashCode()
        {
            return this.AddRem.GetHashCode()
                .CombineHashCode(HashHelper.GetHashCode(this.Item));
        }

        public override string ToString()
        {
            return $"({this.AddRem.ToStringFast_Enum_Only()}: {this.Item})";
        }
    }
}
