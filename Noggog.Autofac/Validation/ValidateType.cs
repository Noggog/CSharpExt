using System;
using System.Collections.Generic;

namespace Noggog.Autofac.Validation
{
    public interface IValidateType
    {
        void Check(Type type, HashSet<string>? paramSkip = null);
    }

    public class ValidateType : IValidateType
    {
        private readonly IRegistrations _registrations;
        private readonly IIsAllowableFunc _allowableFunc;
        private readonly IIsAllowableLazy _allowableLazy;
        private readonly IShouldSkipType _shouldSkipType;
        private readonly ICheckIsDelegateFactory _isDelegateFactory;
        private readonly IIsAllowableEnumerable _allowableEnumerable;

        public ValidateType(
            IRegistrations registrations,
            IIsAllowableFunc allowableFunc,
            IIsAllowableLazy allowableLazy,
            IShouldSkipType shouldSkipType,
            ICheckIsDelegateFactory isDelegateFactory,
            IIsAllowableEnumerable allowableEnumerable)
        {
            _registrations = registrations;
            _allowableFunc = allowableFunc;
            _allowableLazy = allowableLazy;
            _shouldSkipType = shouldSkipType;
            _isDelegateFactory = isDelegateFactory;
            _allowableEnumerable = allowableEnumerable;
        }
        
        public void Check(Type type, HashSet<string>? paramSkip = null)
        {
            if (_shouldSkipType.ShouldSkip(type)) return;
            var constr = type.GetConstructors();
            if (constr.Length > 1)
            {
                throw new AutofacValidationException(
                    $"'{type.FullName}' has more than one constructor");
            }

            if (constr.Length == 0) return;

            foreach (var param in constr[0].GetParameters())
            {
                if (param.IsOptional) continue;
                if (param.Name != null && (paramSkip?.Contains(param.Name) ?? false)) continue;
                if (_registrations.Items.ContainsKey(param.ParameterType)) continue;
                if (_allowableFunc.IsAllowed(param.ParameterType)) continue;
                if (_allowableLazy.IsAllowed(param.ParameterType)) continue;
                if (_allowableEnumerable.IsAllowed(param.ParameterType)) continue;
                if (_isDelegateFactory.Check(param.ParameterType)) continue;
                throw new AutofacValidationException(
                    $"'{type.FullName}' Could not find registration for type `{param.ParameterType}`");
            }
        }
    }
}