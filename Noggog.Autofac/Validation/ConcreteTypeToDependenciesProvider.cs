using System;
using System.Collections.Generic;
using System.Linq;

namespace Noggog.Autofac.Validation
{
    public interface IConcreteTypeToDependenciesProvider
    {
        IReadOnlyDictionary<Type, IReadOnlySet<Type>> ConcreteTypeMapping { get; }
    }

    public class ConcreteTypeToDependenciesProvider : IConcreteTypeToDependenciesProvider
    {
        private readonly Lazy<IReadOnlyDictionary<Type, IReadOnlySet<Type>>> _concreteTypeMapping;
        public IReadOnlyDictionary<Type, IReadOnlySet<Type>> ConcreteTypeMapping => _concreteTypeMapping.Value;
        
        public ConcreteTypeToDependenciesProvider(
            IRegistrations registrations,
            ITypeToDependenciesProvider typeToDependenciesProvider)
        {
            _concreteTypeMapping = new Lazy<IReadOnlyDictionary<Type, IReadOnlySet<Type>>>(() =>
            {
                var dict = new Dictionary<Type, HashSet<Type>>();
                
                foreach (var mapping in typeToDependenciesProvider.DirectTypeMapping)
                {
                    if (!registrations.Items.TryGetValue(mapping.Key, out var keyRegis)) continue;
                    var set = dict.GetOrAdd(keyRegis.First().Type);
                    foreach (var dep in mapping.Value)
                    {
                        if (!registrations.Items.TryGetValue(dep, out var regis)) continue;
                        var first = regis.FirstOrDefault();
                        if (first == null) continue;
                        set.Add(first.Type);
                    }
                }

                return dict.ToDictionary<KeyValuePair<Type, HashSet<Type>>, Type, IReadOnlySet<Type>>(
                    keySelector: x => x.Key,
                    elementSelector: x => x.Value);
            });
        }
    }
}