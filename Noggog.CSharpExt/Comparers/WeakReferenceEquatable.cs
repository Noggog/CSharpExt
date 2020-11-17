using System;

namespace Noggog
{
    public class WeakReferenceEquatable : IEquatable<WeakReferenceEquatable>, IEquatable<WeakReference>
    {
        private readonly int _targetHashCode;
        private readonly WeakReference _weakReferenceToTarget;

        public WeakReferenceEquatable(object target)
        {
            _targetHashCode = target.GetHashCode();
            _weakReferenceToTarget = new WeakReference(target);
        }

        public object? Target => _weakReferenceToTarget.Target;

        public bool IsAlive => _weakReferenceToTarget.IsAlive;

        public override int GetHashCode() => _targetHashCode;

        public override bool Equals(object? obj)
        {
            if (obj is WeakReferenceEquatable rhs)
            {
                return this.Equals(rhs);
            }
            else if (obj is WeakReference weakRef)
            {
                return this.Equals(weakRef);
            }
            else
            {
                if (!this.IsAlive) return false;
                return object.Equals(this.Target, obj);
            }
        }

        public bool Equals(WeakReferenceEquatable? other)
        {
            if (other == null) return false;
            if (this.IsAlive != other.IsAlive) return false;
            if (!this.IsAlive) return true;
            return object.Equals(this.Target, other.Target);
        }

        public bool Equals(WeakReference? other)
        {
            if (other == null) return false;
            if (this.IsAlive != other.IsAlive) return false;
            if (!this.IsAlive) return true;
            return object.Equals(this.Target, other.Target);
        }
    }
}
