using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Core;

namespace Noggog.Autofac
{
    public static class Validation
    {
        public static void ValidateRegistrations(this IContainer container, params Type[] extraUsages)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            var registrations = GetRegistrations(container);
            var usages = new HashSet<Type>(extraUsages);
            FillUsages(registrations.Keys, usages);
            if (registrations.Count == 0)
            {
                throw new InvalidOperationException("No registrations to validate");
            }
            Validate(registrations, usages);
        }

        public static void ValidateRegistrations(this IContainer container, bool evaluateUsages, params Type[] extraUsages)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            var registrations = GetRegistrations(container);
            HashSet<Type>? usages = default;
            if (evaluateUsages)
            {
                usages = new HashSet<Type>();
                FillUsages(registrations.Keys, usages);
            }
            if (registrations.Count == 0)
            {
                throw new InvalidOperationException("No registrations to validate");
            }
            Validate(registrations, usages);
        }

        static void Validate(IDictionary<Type, IList<Type>> registrations, HashSet<Type>? usages)
        {
            foreach (var registration in registrations.OrderBy(x => x.Key.FullName))
            {
                if (IsAutofacType(registration.Key)) continue;
                if (!usages?.Contains(registration.Key) ?? false) continue;
                
                if (registration.Value.Count == 0)
                {
                    throw new InvalidOperationException(
                        $"'{registration.Key.FullName}' does not have an implementation");
                }

                foreach (var regis in registration.Value)
                {
                    Validate(regis, registrations);
                }
            }
        }

        private static bool IsAutofacType(Type type)
        {
            if (type.Namespace?.StartsWith("Castle") ?? false) return true;
            if (type.Namespace?.StartsWith("Autofac") ?? false) return true;
            return false;
        }

        static void Validate(Type type, IDictionary<Type, IList<Type>> registrations, HashSet<string>? paramSkip = null)
        {
            if (IsAutofacType(type)) return;
            var constr = type.GetConstructors();
            if (constr.Length > 1)
            {
                throw new InvalidOperationException(
                    $"'{type.FullName}' has more than one constructor");
            }

            if (constr.Length == 0) return;

            foreach (var param in constr[0].GetParameters())
            {
                if (param.IsOptional) continue;
                if (param.Name != null && (paramSkip?.Contains(param.Name) ?? false)) continue;
                if (registrations.ContainsKey(param.ParameterType)) continue;
                if (IsAllowableFunc(param, registrations)) continue;
                if (IsAllowableLazy(param, registrations)) continue;
                if (IsAllowableEnumerable(param, registrations)) continue;
                if (CheckIsDelegateFactory(param, registrations)) continue;
                throw new InvalidOperationException(
                    $"'{type.FullName}' Could not find registration for type `{param.ParameterType}`");
            }
        }

        static bool IsAllowableFunc(ParameterInfo param, IDictionary<Type, IList<Type>> registrations)
        {
            if (param.ParameterType.Name.StartsWith("Func")
                        && param.ParameterType.IsGenericType
                        && param.ParameterType.GenericTypeArguments.Length == 1)
            {
                Validate(param.ParameterType.GenericTypeArguments[0], registrations);
                return true;
            }
            return false;
        }

        static bool IsAllowableLazy(ParameterInfo param, IDictionary<Type, IList<Type>> registrations)
        {
            if (param.ParameterType.Name.StartsWith("Lazy")
                && param.ParameterType.IsGenericType
                && param.ParameterType.GenericTypeArguments.Length == 1)
            {
                Validate(param.ParameterType.GenericTypeArguments[0], registrations);
                return true;
            }
            return false;
        }

        static bool IsAllowableEnumerable(ParameterInfo param, IDictionary<Type, IList<Type>> registrations)
        {
            if (param.ParameterType.Name.StartsWith("IEnumerable")
                && param.ParameterType.IsGenericType
                && param.ParameterType.GenericTypeArguments.Length == 1)
            {
                Validate(param.ParameterType.GenericTypeArguments[0], registrations);
                return true;
            }
            return false;
        }

        static bool CheckIsDelegateFactory(ParameterInfo param, IDictionary<Type, IList<Type>> registrations)
        {
            if (param.ParameterType.BaseType?.FullName != "System.MulticastDelegate") return false;
            var invoke = param.ParameterType.GetMethod("Invoke");
            if (invoke == null) return false;
            if (invoke.ReturnType == typeof(void)) return false;
            var parameterNames = new HashSet<string>(invoke.GetParameters().Select(p => p.Name).NotNull());
            Validate(invoke.ReturnType, registrations, parameterNames);
            return true;
        }

        static IDictionary<Type, IList<Type>> GetRegistrations(IContainer container)
        {
            var registrations = new Dictionary<Type, IList<Type>>();

            foreach (var registration in container.ComponentRegistry.Registrations)
            {
                foreach (var service in registration.Services.OfType<IServiceWithType>())
                {
                    if (!registrations.TryGetValue(service.ServiceType, out var implementations))
                    {
                        implementations = new List<Type>();
                        registrations[service.ServiceType] = implementations;
                    }

                    implementations.Add(registration.Activator.LimitType);
                }
            }

            return registrations;
        }

        static void FillUsages(IEnumerable<Type> concreteTypes, HashSet<Type> ctorUsages)
        {
            foreach (var type in concreteTypes)
            {
                foreach (var ctor in type.GetConstructors())
                {
                    foreach (var param in ctor.GetParameters())
                    {
                        ctorUsages.Add(param.ParameterType);
                    }
                }
            }
        }
    }
}