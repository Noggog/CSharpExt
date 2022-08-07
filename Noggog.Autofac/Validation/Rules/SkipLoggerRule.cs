namespace Noggog.Autofac.Validation.Rules;

public class SkipLoggerRule : IValidationRule
{
    // To Do
    // Find a more robust way to validate logging systems
    public bool IsAllowed(Type type)
    {
        if (type.Name.StartsWith("ILogger")) return true;
        return false;
    }
}