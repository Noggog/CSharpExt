using System;

namespace Noggog.Notifying
{
    public class ChangePoint<T> : IEquatable<ChangePoint<T>>
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

        public bool Equals(ChangePoint<T> other)
        {
            return this.AddRem == other.AddRem
                && this.Point.Equals(other.Point)
                && object.Equals(this.Old, other.Old)
                && object.Equals(this.New, other.New);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ChangePoint<T> rhs)) return false;
            return Equals(rhs);
        }

        public override int GetHashCode()
        {
            return this.AddRem.GetHashCode()
                .CombineHashCode(this.Point.GetHashCode())
                .CombineHashCode(HashHelper.GetHashCode(this.Old, this.New));
        }

        public override string ToString()
        {
            return $"({this.AddRem.ToStringFast_Enum_Only()}: {Point}, {this.Old} => {this.New})";
        }
    }
}
