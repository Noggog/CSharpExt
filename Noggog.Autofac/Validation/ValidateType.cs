using Noggog.Autofac.Validation.Rules;

namespace Noggog.Autofac.Validation;

public interface IValidateType
{
    void Validate(Type type, bool validateCtor = true);
}

public class ValidateType : IValidateType
{
    public IRegistrations Registrations { get; }
    private readonly IValidateTracker _tracker;
    public IValidationRule[] Rules { get; }
        
    private readonly HashSet<Type> _checkedTypes = new();

    public IValidateTypeCtor ValidateCtor { get; set; } = null!;
        
    public ValidateType(
        IRegistrations registrations,
        IValidateTracker tracker,
        IValidationRule[] rules)
    {
        Registrations = registrations;
        _tracker = tracker;
        Rules = rules;
    }
        
    public void Validate(Type type, bool validateCtor = true)
    {
        if (!_checkedTypes.Add(type)) return;
        using var track = _tracker.Track(type);
        if (Rules.Any(r => r.IsAllowed(type))) return;
        if (Registrations.Items.ContainsKey(type))
        {
            var regis = Registrations.Items[type][^1];
            if (validateCtor && regis.NeedsValidation)
            {
                ValidateCtor.Validate(regis.Type);
            }
            return;
        }
            
        throw new AutofacValidationException(
            $"'{type.FullName}' Could not find registration for type `{type}`. {_tracker.State()}");
    }
}