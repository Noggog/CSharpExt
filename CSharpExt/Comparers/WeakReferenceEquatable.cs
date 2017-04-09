using System;

namespace Noggog
{
    public class WeakReferenceEquatable
    {
        private readonly int _targetHashCode;
        private readonly WeakReference _weakReferenceToTarget;

        public WeakReferenceEquatable(object target)
        {
            _targetHashCode = target.GetHashCode();
            _weakReferenceToTarget = new WeakReference(target);
        }

        public object Target =>_weakReferenceToTarget.Target;

        public bool IsAlive => _weakReferenceToTarget.IsAlive;

        public override int GetHashCode() => _targetHashCode;

        public override bool Equals(object obj)
        {
            if (!(obj is WeakReferenceEquatable rhs)) return false;
            if (_targetHashCode != rhs.GetHashCode()) return false;
            if (this.IsAlive != rhs.IsAlive) return false;
            if (!this.IsAlive) return true;
            return object.Equals(Target, rhs.Target);
        }
    }
}
