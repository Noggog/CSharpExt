namespace Noggog.Autofac.Validation;

public interface IValidateType
{
    void Validate(Type type, bool validateCtor = true);
}

public class ValidateType : IValidateType
{
    public IRegistrations Registrations { get; }
    private readonly IValidateTracker _tracker;
    public IIsAllowableFunc AllowableFunc { get; }
    public IIsAllowableLazy AllowableLazy { get; }
    public ICheckIsDelegateFactory IsDelegateFactory { get; }
    public IIsAllowableEnumerable AllowableEnumerable { get; }
        
    private readonly HashSet<Type> _checkedTypes = new();

    public IValidateTypeCtor ValidateCtor { get; set; } = null!;
        
    public ValidateType(
        IRegistrations registrations,
        IValidateTracker tracker,
        IIsAllowableFunc allowableFunc,
        IIsAllowableLazy allowableLazy,
        ICheckIsDelegateFactory isDelegateFactory,
        IIsAllowableEnumerable allowableEnumerable)
    {
        Registrations = registrations;
        _tracker = tracker;
        AllowableFunc = allowableFunc;
        AllowableLazy = allowableLazy;
        IsDelegateFactory = isDelegateFactory;
        AllowableEnumerable = allowableEnumerable;
    }
        
    public void Validate(Type type, bool validateCtor = true)
    {
        if (!_checkedTypes.Add(type)) return;
        using var track = _tracker.Track(type);
        if (IsDelegateFactory.Check(type)) return;
        if (AllowableFunc.IsAllowed(type)) return;
        if (AllowableLazy.IsAllowed(type)) return;
        if (AllowableEnumerable.IsAllowed(type)) return;
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