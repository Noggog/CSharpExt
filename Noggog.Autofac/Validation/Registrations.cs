using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Autofac.Core;

namespace Noggog.Autofac.Validation
{
    public interface IRegistrations
    {
        IReadOnlyDictionary<Type, IReadOnlyList<Type>> Items { get; }
    }

    public class Registrations : IRegistrations
    {
        private Lazy<IReadOnlyDictionary<Type, IReadOnlyList<Type>>> _items;
        public IReadOnlyDictionary<Type, IReadOnlyList<Type>> Items => _items.Value;

        public Registrations(IContainer container)
        {
            _items = new Lazy<IReadOnlyDictionary<Type, IReadOnlyList<Type>>>(() =>
            {
                var registrations = new Dictionary<Type, List<Type>>();

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

                if (registrations.Count == 0)
                {
                    throw new AutofacValidationException("No registrations to validate");
                }

                return registrations.ToDictionary<KeyValuePair<Type, List<Type>>, Type, IReadOnlyList<Type>>(
                    x => x.Key,
                    x => x.Value);
            });
        }
    }
}