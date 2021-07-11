using System;
using System.Collections.Generic;

namespace Noggog.Autofac.Validation
{
    public interface IValidateTypeCtor
    {
        void Validate(Type type, HashSet<string>? paramSkip = null);
    }

    public class ValidateTypeCtor : IValidateTypeCtor
    {
        private readonly IShouldSkipType _shouldSkipType;
        private readonly IValidateType _validateType;
        private readonly HashSet<Type> _checkedTypes = new();
        
        public ValidateTypeCtor(
            IShouldSkipType shouldSkipType,
            IValidateType validateType)
        {
            _shouldSkipType = shouldSkipType;
            _validateType = validateType;
        }
        
        public void Validate(Type type, HashSet<string>? paramSkip = null)
        {
            if (!_checkedTypes.Add(type)) return;
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
                _validateType.Validate(param.ParameterType);
            }
        }
    }
}