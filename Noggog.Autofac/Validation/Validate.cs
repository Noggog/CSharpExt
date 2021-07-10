using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;

namespace Noggog.Autofac.Validation
{
    public interface IValidate
    {
        void ValidateRegistrations(bool evaluateUsages, params Type[] extraUsages);
    }

    public class Validate : IValidate
    {
        private readonly IGetUsages _getUsages;
        private readonly IRegistrations _registrations;
        private readonly IValidateAllRegistrations _validateAllRegistrations;

        public Validate(
            IGetUsages getUsages,
            IRegistrations registrations,
            IValidateAllRegistrations validateAllRegistrations)
        {
            _getUsages = getUsages;
            _registrations = registrations;
            _validateAllRegistrations = validateAllRegistrations;
        }
        
        public void ValidateRegistrations(bool evaluateUsages, params Type[] extraUsages)
        {
            HashSet<Type>? usages = default;
            if (evaluateUsages)
            {
                usages = _getUsages.Get(_registrations.Items.Keys.ToArray());
                usages.Add(extraUsages);
            }

            _validateAllRegistrations.Check(usages);
        }
    }
}