﻿namespace Noggog.Autofac.Validation.Rules;

public class CheckIsDelegateFactory : IValidationRule
{
    public IRegistrations Registrations { get; }
    public IValidateTypeCtor ValidateTypeCtor { get; set; } = null!;
    public IValidateType ValidateType { get; set; } = null!;

    public CheckIsDelegateFactory(IRegistrations registrations)
    {
        Registrations = registrations;
    }

    public bool IsAllowed(Type type)
    {
        if (type.BaseType?.FullName != "System.MulticastDelegate") return false;
        var invoke = type.GetMethod("Invoke");
        if (invoke == null) return false;
        if (invoke.ReturnType == typeof(void)) return false;

        var typeToCheck = invoke.ReturnType;
        ValidateType.Validate(typeToCheck, validateCtor: false);
            
        if (Registrations.Items.TryGetValue(typeToCheck, out var registrations))
        {
            var register = registrations.FirstOrDefault();
            if (register != null)
            {
                if (!register.NeedsValidation) return true;
                typeToCheck = register.Type;
            }
        }
            
        var parameterNames = new HashSet<string>(invoke.GetParameters().Select(p => p.Name).WhereNotNull());
        ValidateTypeCtor.Validate(typeToCheck, parameterNames);
        return true;
    }
}