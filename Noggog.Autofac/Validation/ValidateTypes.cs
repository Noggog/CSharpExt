namespace Noggog.Autofac.Validation;

public interface IValidateTypes
{
    void Validate(IEnumerable<Type> types);
}

public class ValidateTypes : IValidateTypes
{
    public IRegistrations Registrations { get; }
    public IValidateTracker Tracker { get; }
    public IValidateTypeCtor TypeCtor { get; }

    public ValidateTypes(
        IRegistrations registrations,
        IValidateTracker tracker,
        IValidateTypeCtor validateTypeCtor)
    {
        Registrations = registrations;
        Tracker = tracker;
        TypeCtor = validateTypeCtor;
    }
        
    public void Validate(IEnumerable<Type> types)
    {
        foreach (var type in types)
        {
            try
            {
                var registrations = Registrations.Items[type];
                if (registrations.Count == 0)
                {
                    throw new AutofacValidationException(
                        $"'{type.FullName}' does not have an implementation");
                }

                foreach (var regis in registrations)
                {
                    using var tracker = Tracker.Track(regis.Type);
                    TypeCtor.Validate(regis.Type);
                }
            }
            catch (Exception e)
            {
                throw new AutofacValidationException(
                    $"'{type.FullName}' had a validation problem.", e);
            }
        }
    }
}