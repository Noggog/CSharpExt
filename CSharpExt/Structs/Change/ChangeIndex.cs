using System;

namespace Noggog
{
    public class ChangeIndex<T> : IEquatable<ChangeIndex<T>>
    {
        public readonly T Old;
        public readonly T New;
        public readonly AddRemoveModify AddRem;
        public readonly int Index;

        public ChangeIndex(T oldItem, T newItem, AddRemoveModify addRem, int index)
        {
            this.Old = oldItem;
            this.New = newItem;
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

        public ChangeIndex<R> Convert<R>(Func<T, R> convert)
        {
            return new ChangeIndex<R>(
                oldItem: convert(this.Old),
                newItem: convert(this.New),
                addRem: this.AddRem,
                index: this.Index);
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
