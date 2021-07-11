using System;
using System.Reflection;

namespace Noggog.Autofac.Validation
{
    public interface IIsAllowableEnumerable
    {
        bool IsAllowed(Type type);
    }

    public class IsAllowableEnumerable : IIsAllowableEnumerable
    {
        public IValidateTypeCtor ValidateTypeCtor { get; set; } = null!;

        public bool IsAllowed(Type type)
        {
            if (type.Name.StartsWith("IEnumerable")
                && type.IsGenericType
                && type.GenericTypeArguments.Length == 1)
            {
                ValidateTypeCtor.Validate(type.GenericTypeArguments[0]);
                return true;
            }
            return false;
        }
    }
}