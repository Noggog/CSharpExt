using System;
using System.Reflection;

namespace Noggog.Autofac.Validation
{
    public interface IIsAllowableLazy
    {
        bool IsAllowed(Type type);
    }

    public class IsAllowableLazy : IIsAllowableLazy
    {
        public IValidateType ValidateType { get; set; } = null!;

        public bool IsAllowed(Type type)
        {
            if (type.Name.StartsWith("Lazy")
                && type.IsGenericType
                && type.GenericTypeArguments.Length == 1)
            {
                ValidateType.Check(type.GenericTypeArguments[0]);
                return true;
            }
            return false;
        }
    }
}