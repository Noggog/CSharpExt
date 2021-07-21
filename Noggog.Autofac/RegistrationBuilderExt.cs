using System;
using System.Linq;
using Autofac;
using Builder = Autofac.Builder.IRegistrationBuilder<object, Autofac.Features.Scanning.ScanningActivatorData, Autofac.Builder.DynamicRegistrationStyle>;

namespace Noggog.Autofac
{
    public static class RegistrationBuilderExt
    {
        public static Builder InNamespacesOf(
            this Builder registration,
            params Type[] types)
        {
            var ns = types.Select(x => x.Namespace!).ToArray();
            return registration.Where(t => ns.Any(t.IsInNamespace));
        }
        
        public static Builder NotInNamespacesOf(
            this Builder registration,
            params Type[] types)
        {
            var ns = types.Select(x => x.Namespace!).ToArray();
            return registration.Where(t => !ns.Any(t.IsInNamespace));
        }
        
        public static Builder NotInjection(this Builder registration)
        {
            return registration
                .Where(t => !t.Name.EndsWith("Injection", StringComparison.OrdinalIgnoreCase));
        }
        
        public static Builder AsMatchingInterface(this Builder registration)
        {
            return registration
                .As(t =>
                {
                    return t.GetInterfaces().Where(x => x.Name == $"I{t.Name}");
                });
        }
    }
}