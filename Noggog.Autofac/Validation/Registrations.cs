using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Autofac.Core;
using Autofac.Core.Activators.Reflection;

namespace Noggog.Autofac.Validation
{
    public record Registration(Type Type, bool NeedsValidation);
    
    public interface IRegistrations
    {
        IReadOnlyDictionary<Type, IReadOnlyList<Registration>> Items { get; }
    }

    public class Registrations : IRegistrations
    {
        private Lazy<IReadOnlyDictionary<Type, IReadOnlyList<Registration>>> _items;
        public IReadOnlyDictionary<Type, IReadOnlyList<Registration>> Items => _items.Value;

        public Registrations(IContainer container)
        {
            _items = new Lazy<IReadOnlyDictionary<Type, IReadOnlyList<Registration>>>(() =>
            {
                var registrations = new Dictionary<Type, List<Registration>>();

                foreach (var registration in container.ComponentRegistry.Registrations)
                {
                    foreach (var service in registration.Services.OfType<IServiceWithType>())
                    {
                        if (!registrations.TryGetValue(service.ServiceType, out var implementations))
                        {
                            implementations = new List<Registration>();
                            registrations[service.ServiceType] = implementations;
                        }

                        implementations.Add(new Registration(
                            registration.Activator.LimitType,
                            registration.Activator is ReflectionActivator));
                    }
                }

                if (registrations.Count == 0)
                {
                    throw new AutofacValidationException("No registrations to validate");
                }

                return registrations.ToDictionary<KeyValuePair<Type, List<Registration>>, Type, IReadOnlyList<Registration>>(
                    x => x.Key,
                    x => x.Value);
            });
        }
    }
}