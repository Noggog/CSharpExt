namespace Noggog.Autofac.Validation;

public class IsAllowableEnumerable : IValidationRule
{
    public IValidateTypeCtor ValidateTypeCtor { get; set; } = null!;

    public bool IsAllowed(Type type)
    {
        if (type.Name.StartsWith("IEnumerable")
            && type.IsGenericType
            && type.GenericTypeArguments.Length == 1)
        {
            ValidateTypeCtor.Validate(type.GenericTypeArguments[0]);
            return true;
        }

        if (type.Name.EndsWith("[]"))
        {
            var elemType = type.GetElementType();
            if (elemType != null)
            {
                ValidateTypeCtor.Validate(elemType);
                return true;
            }
        }
        return false;
    }
}