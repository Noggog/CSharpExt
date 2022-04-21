namespace Noggog.Autofac.Validation;

public interface IIsAllowableFunc
{
    bool IsAllowed(Type type);
}

public class IsAllowableFunc : IIsAllowableFunc
{
    public IValidateTypeCtor ValidateTypeCtor { get; set; } = null!;

    public bool IsAllowed(Type type)
    {
        if (type.Name.StartsWith("Func")
            && type.IsGenericType
            && type.GenericTypeArguments.Length == 1)
        {
            ValidateTypeCtor.Validate(type.GenericTypeArguments[0]);
            return true;
        }
        return false;
    }
}