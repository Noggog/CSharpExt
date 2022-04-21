namespace Noggog.Autofac.Validation;

public interface IValidateTypeCtor
{
    void Validate(Type type, HashSet<string>? paramSkip = null);
}

public class ValidateTypeCtor : IValidateTypeCtor
{
    public IShouldSkipType ShouldSkip { get; }
    public IValidateType ValidateType { get; }

    public ValidateTypeCtor(
        IShouldSkipType shouldShouldSkip,
        IValidateType validateType)
    {
        ShouldSkip = shouldShouldSkip;
        ValidateType = validateType;
    }
        
    public void Validate(Type type, HashSet<string>? paramSkip = null)
    {
        if (ShouldSkip.ShouldSkip(type)) return;
        var constr = type.GetConstructors();
        if (constr.Length > 1)
        {
            throw new AutofacValidationException(
                $"'{type.FullName}' has more than one constructor");
        }

        if (constr.Length == 0) return;

        foreach (var param in constr[0].GetParameters())
        {
            if (param.IsOptional) continue;
            if (param.Name != null && (paramSkip?.Contains(param.Name) ?? false)) continue;
            ValidateType.Validate(param.ParameterType);
        }
    }
}