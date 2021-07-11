using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Noggog.Autofac.Validation
{
    public interface ICheckIsDelegateFactory
    {
        bool Check(Type type);
    }

    public class CheckIsDelegateFactory : ICheckIsDelegateFactory
    {
        public IValidateTypeCtor ValidateTypeCtor { get; set; } = null!;
        public IValidateType ValidateType { get; set; } = null!;

        public bool Check(Type type)
        {
            if (type.BaseType?.FullName != "System.MulticastDelegate") return false;
            var invoke = type.GetMethod("Invoke");
            if (invoke == null) return false;
            if (invoke.ReturnType == typeof(void)) return false;
            ValidateType.Validate(invoke.ReturnType, validateCtor: false);
            var parameterNames = new HashSet<string>(invoke.GetParameters().Select(p => p.Name).NotNull());
            ValidateTypeCtor.Validate(invoke.ReturnType, parameterNames);
            return true;
        }
    }
}