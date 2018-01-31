using System;

namespace Noggog.Notifying
{
    public class Change<T> : IEquatable<Change<T>>
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

        public bool Equals(Change<T> other)
        {
            return object.Equals(this.Old, other.Old)
                && object.Equals(this.New, other.New);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Change<T> rhs)) return false;
            return Equals(rhs);
        }

        public override int GetHashCode()
        {
            return HashHelper.GetHashCode(this.Old, this.New);
        }

        public override string ToString()
        {
            return $"({this.Old} => {this.New})";
        }
    }
}
