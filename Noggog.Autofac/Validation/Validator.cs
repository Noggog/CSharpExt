using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;

namespace Noggog.Autofac.Validation
{
    public interface IValidator
    {
        void ValidateEverything();
        void Validate(Type drivingType, params Type[] otherdrivingTypes);
    }

    public class Validator : IValidator
    {
        private readonly IRegistrations _registrations;
        private readonly IValidateTypes _validateTypes;
        private readonly IShouldSkipType _shouldSkipType;
        private readonly ICircularReferenceChecker _circularReferenceChecker;

        public Validator(
            IRegistrations registrations,
            IValidateTypes validateTypes,
            IShouldSkipType shouldSkipType,
            ICircularReferenceChecker circularReferenceChecker)
        {
            _registrations = registrations;
            _validateTypes = validateTypes;
            _shouldSkipType = shouldSkipType;
            _circularReferenceChecker = circularReferenceChecker;
        }
        
        private void InternalValidate(IEnumerable<Type> types)
        {
            _circularReferenceChecker.Check();
            _validateTypes.Validate(types);
        }

        public void ValidateEverything()
        {
            InternalValidate(_registrations.Items.Keys
                .Where(type => !_shouldSkipType.ShouldSkip(type)));
        }

        public void Validate(Type drivingType, params Type[] otherdrivingTypes)
        {
            InternalValidate(drivingType.AsEnumerable().Concat(otherdrivingTypes));
        }
    }
}