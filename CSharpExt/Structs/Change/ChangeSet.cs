using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noggog.Notifying
{
    public class ChangeSet<T> : Change<T>, IEquatable<ChangeSet<T>>
    {
        public readonly bool OldSet;
        public readonly bool NewSet;

        public ChangeSet(T newVal, bool newSet = true)
            : base(newVal)
        {
            this.OldSet = false;
            this.NewSet = newSet;
        }

        public ChangeSet(T oldVal, T newVal, bool oldSet = true, bool newSet = true)
            : base(oldVal, newVal)
        {
            this.OldSet = oldSet;
            this.NewSet = newSet;
        }

        public ChangeSet(IHasBeenSetItem<T> newVal)
            : this(
                  newVal: newVal.Item, 
                  newSet: newVal.HasBeenSet)
        {
        }

        public ChangeSet(IHasBeenSetItem<T> oldVal, IHasBeenSetItem<T> newVal)
            : this(
                  oldVal: oldVal.Item, 
                  newVal: newVal.Item,
                  oldSet: oldVal.HasBeenSet,
                  newSet: newVal.HasBeenSet)
        {
        }

        public new ChangeSet<R> Convert<R>(Func<T, R> convert)
        {
            return new ChangeSet<R>(
                oldVal: convert(this.Old),
                oldSet: this.OldSet,
                newVal: convert(this.New),
                newSet: this.NewSet);
        }

        public bool Equals(ChangeSet<T> other)
        {
            return this.OldSet == other.OldSet
                && this.NewSet == other.NewSet
                && base.Equals(other);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ChangeSet<T> rhs)) return false;
            return Equals(rhs);
        }

        public override int GetHashCode()
        {
            return HashHelper.GetHashCode(this.OldSet, this.NewSet)
                .CombineHashCode(base.GetHashCode());
        }

        public override string ToString()
        {
            return $"(({(this.OldSet ? "X" : " ")}){this.Old} => ({(this.NewSet ? "X" : " ")}){this.New})";
        }
    }
}
