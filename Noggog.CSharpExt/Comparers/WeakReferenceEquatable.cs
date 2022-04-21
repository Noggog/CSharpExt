namespace Noggog;

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
            return Equals(rhs);
        }
        else if (obj is WeakReference weakRef)
        {
            return Equals(weakRef);
        }
        else
        {
            if (!IsAlive) return false;
            return Equals(Target, obj);
        }
    }

    public bool Equals(WeakReferenceEquatable? other)
    {
        if (other == null) return false;
        if (IsAlive != other.IsAlive) return false;
        if (!IsAlive) return true;
        return Equals(Target, other.Target);
    }

    public bool Equals(WeakReference? other)
    {
        if (other == null) return false;
        if (IsAlive != other.IsAlive) return false;
        if (!IsAlive) return true;
        return Equals(Target, other.Target);
    }
}