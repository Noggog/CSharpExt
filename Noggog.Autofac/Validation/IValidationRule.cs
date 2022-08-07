namespace Noggog.Autofac.Validation;

public interface IValidationRule
{
    bool IsAllowed(Type type);
}