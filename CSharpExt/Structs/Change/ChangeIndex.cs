using System;

namespace Noggog.Notifying
{
    public class ChangeIndex<T> : IEquatable<ChangeIndex<T>>
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

        public bool Equals(ChangeIndex<T> other)
        {
            return this.AddRem == other.AddRem
                && this.Index == other.Index
                && object.Equals(this.Old, other.Old)
                && object.Equals(this.New, other.New);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ChangeIndex<T> rhs)) return false;
            return Equals(rhs);
        }

        public override int GetHashCode()
        {
            return AddRem.GetHashCode()
                .CombineHashCode(Index.GetHashCode())
                .CombineHashCode(HashHelper.GetHashCode(Old, New));
        }

        public override string ToString()
        {
            return $"({this.AddRem.ToStringFast_Enum_Only()}: {this.Index}, {this.Old} => {this.New})";
        }
    }
}
