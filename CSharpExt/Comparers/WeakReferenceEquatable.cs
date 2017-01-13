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

        public object Target
        {
            get { return _weakReferenceToTarget.Target; }
        }

        public bool IsAlive
        {
            get
            {
                return _weakReferenceToTarget.IsAlive;
            }
        }

        public override int GetHashCode()
        {
            return _targetHashCode;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is WeakReferenceEquatable)) return false;
            WeakReferenceEquatable rhs = (WeakReferenceEquatable)obj;
            if (_targetHashCode != rhs.GetHashCode()) return false;
            if (this.IsAlive != rhs.IsAlive) return false;
            if (!this.IsAlive) return true;
            return object.Equals(Target, rhs.Target);
        }
    }
}
