namespace Noggog.Autofac.Validation.Rules;

public interface IValidationRule
{
    bool IsAllowed(Type type);
}