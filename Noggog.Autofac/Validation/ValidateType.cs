using System;
using System.Collections.Generic;

namespace Noggog.Autofac.Validation
{
    public interface IValidateType
    {
        void Validate(Type type);
    }

    public class ValidateType : IValidateType
    {
        private readonly IRegistrations _registrations;
        private readonly IIsAllowableFunc _allowableFunc;
        private readonly IIsAllowableLazy _allowableLazy;
        private readonly ICheckIsDelegateFactory _isDelegateFactory;
        private readonly IIsAllowableEnumerable _allowableEnumerable;
        private readonly HashSet<Type> _checkedTypes = new();

        public ValidateType(
            IRegistrations registrations,
            IIsAllowableFunc allowableFunc,
            IIsAllowableLazy allowableLazy,
            ICheckIsDelegateFactory isDelegateFactory,
            IIsAllowableEnumerable allowableEnumerable)
        {
            _registrations = registrations;
            _allowableFunc = allowableFunc;
            _allowableLazy = allowableLazy;
            _isDelegateFactory = isDelegateFactory;
            _allowableEnumerable = allowableEnumerable;
        }
        
        public void Validate(Type type)
        {
            if (!_checkedTypes.Add(type)) return;
            if (_registrations.Items.ContainsKey(type)) return;
            if (_allowableFunc.IsAllowed(type)) return;
            if (_allowableLazy.IsAllowed(type)) return;
            if (_allowableEnumerable.IsAllowed(type)) return;
            if (_isDelegateFactory.Check(type)) return;
            throw new AutofacValidationException(
                $"'{type.FullName}' Could not find registration for type `{type}`");
        }
    }
}