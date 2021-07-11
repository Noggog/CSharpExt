using System;
using System.Linq;
using System.Collections.Generic;

namespace Noggog.Autofac.Validation
{
    public interface IValidateType
    {
        void Validate(Type type, bool validateCtor = true);
    }

    public class ValidateType : IValidateType
    {
        private readonly IRegistrations _registrations;
        private readonly IValidateTracker _tracker;
        private readonly IIsAllowableFunc _allowableFunc;
        private readonly IIsAllowableLazy _allowableLazy;
        private readonly ICheckIsDelegateFactory _isDelegateFactory;
        private readonly IIsAllowableEnumerable _allowableEnumerable;
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
            _registrations = registrations;
            _tracker = tracker;
            _allowableFunc = allowableFunc;
            _allowableLazy = allowableLazy;
            _isDelegateFactory = isDelegateFactory;
            _allowableEnumerable = allowableEnumerable;
        }
        
        public void Validate(Type type, bool validateCtor = true)
        {
            if (!_checkedTypes.Add(type)) return;
            using var track = _tracker.Track(type);
            if (_isDelegateFactory.Check(type)) return;
            if (_allowableFunc.IsAllowed(type)) return;
            if (_allowableLazy.IsAllowed(type)) return;
            if (_allowableEnumerable.IsAllowed(type)) return;
            if (_registrations.Items.ContainsKey(type))
            {
                if (validateCtor)
                {
                    ValidateCtor.Validate(_registrations.Items[type].First());
                }
                return;
            }
            
            throw new AutofacValidationException(
                $"'{type.FullName}' Could not find registration for type `{type}`. {_tracker.State()}");
        }
    }
}