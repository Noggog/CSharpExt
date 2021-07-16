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
        public IRegistrations Registrations { get; }
        public IValidateTypes ValidateTypes { get; }
        public IShouldSkipType ShouldSkip { get; }
        public ICircularReferenceChecker ReferenceChecker { get; }

        public Validator(
            IRegistrations registrations,
            IValidateTypes validateTypes,
            IShouldSkipType shouldShouldSkip,
            ICircularReferenceChecker circularReferenceChecker)
        {
            Registrations = registrations;
            ValidateTypes = validateTypes;
            ShouldSkip = shouldShouldSkip;
            ReferenceChecker = circularReferenceChecker;
        }
        
        private void InternalValidate(IEnumerable<Type> types)
        {
            ReferenceChecker.Check();
            ValidateTypes.Validate(types);
        }

        public void ValidateEverything()
        {
            InternalValidate(Registrations.Items.Keys
                .Where(type => !ShouldSkip.ShouldSkip(type)));
        }

        public void Validate(Type drivingType, params Type[] otherdrivingTypes)
        {
            InternalValidate(drivingType.AsEnumerable().Concat(otherdrivingTypes));
        }
    }
}