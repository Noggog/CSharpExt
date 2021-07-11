using System;
using System.Collections.Generic;
using System.Linq;

namespace Noggog.Autofac.Validation
{
    public interface IValidateTypes
    {
        void Validate(IEnumerable<Type> types);
    }

    public class ValidateTypes : IValidateTypes
    {
        private readonly IRegistrations _registrations;
        private readonly IValidateType _validateType;

        public ValidateTypes(
            IRegistrations registrations,
            IValidateType validateType)
        {
            _registrations = registrations;
            _validateType = validateType;
        }
        
        public void Validate(IEnumerable<Type> types)
        {
            foreach (var type in types)
            {
                var registrations = _registrations.Items[type];
                if (registrations.Count == 0)
                {
                    throw new AutofacValidationException(
                        $"'{type.FullName}' does not have an implementation");
                }

                foreach (var regis in registrations)
                {
                    _validateType.Check(regis);
                }
            }
        }
    }
}