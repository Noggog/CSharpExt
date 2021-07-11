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
        private readonly IValidateTracker _tracker;
        private readonly IValidateTypeCtor _validateTypeCtor;

        public ValidateTypes(
            IRegistrations registrations,
            IValidateTracker tracker,
            IValidateTypeCtor validateTypeCtor)
        {
            _registrations = registrations;
            _tracker = tracker;
            _validateTypeCtor = validateTypeCtor;
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
                    using var tracker = _tracker.Track(regis);
                    _validateTypeCtor.Validate(regis);
                }
            }
        }
    }
}