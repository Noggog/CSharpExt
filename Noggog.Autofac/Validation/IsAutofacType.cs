namespace Noggog.Autofac.Validation;

public interface IShouldSkipType
{
    bool ShouldSkip(Type type);
}

public class ShouldSkipType : IShouldSkipType
{
    public bool ShouldSkip(Type type)
    {
        if (type.Namespace?.StartsWith("Castle") ?? false) return true;
        if (type.Namespace?.StartsWith("Autofac") ?? false) return true;
        return false;
    }
}