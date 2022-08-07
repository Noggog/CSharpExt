namespace Noggog.Autofac.Validation;

public class IsAllowableLazy : IValidationRule
{
    public IValidateTypeCtor ValidateTypeCtor { get; set; } = null!;

    public bool IsAllowed(Type type)
    {
        if (type.Name.StartsWith("Lazy")
            && type.IsGenericType
            && type.GenericTypeArguments.Length == 1)
        {
            ValidateTypeCtor.Validate(type.GenericTypeArguments[0]);
            return true;
        }
        return false;
    }
}