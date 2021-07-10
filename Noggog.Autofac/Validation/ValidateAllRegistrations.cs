using System;
using System.Collections.Generic;
using System.Linq;

namespace Noggog.Autofac.Validation
{
    public interface IValidateAllRegistrations
    {
        void Check(HashSet<Type>? usages);
    }

    public class ValidateAllRegistrations : IValidateAllRegistrations
    {
        private readonly IRegistrations _registrations;
        private readonly IValidateType _validateType;
        private readonly IShouldSkipType _shouldSkipType;

        public ValidateAllRegistrations(
            IRegistrations registrations,
            IValidateType validateType,
            IShouldSkipType shouldSkipType)
        {
            _registrations = registrations;
            _validateType = validateType;
            _shouldSkipType = shouldSkipType;
        }
        
        public void Check(HashSet<Type>? usages)
        {
            foreach (var registration in _registrations.Items)
            {
                if (_shouldSkipType.ShouldSkip(registration.Key)) continue;
                if (!usages?.Contains(registration.Key) ?? false) continue;
                
                if (registration.Value.Count == 0)
                {
                    throw new InvalidOperationException(
                        $"'{registration.Key.FullName}' does not have an implementation");
                }

                foreach (var regis in registration.Value)
                {
                    _validateType.Check(regis);
                }
            }
        }
    }
}