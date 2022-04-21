namespace Noggog.Autofac.Validation;

public interface ITypeToDependenciesProvider
{
    IReadOnlyDictionary<Type, IReadOnlySet<Type>> DirectTypeMapping { get; }
}

public class TypeToDependenciesProvider : ITypeToDependenciesProvider
{
    private readonly Lazy<IReadOnlyDictionary<Type, IReadOnlySet<Type>>> _directTypeMapping;
    public IReadOnlyDictionary<Type, IReadOnlySet<Type>> DirectTypeMapping => _directTypeMapping.Value;

    public TypeToDependenciesProvider(
        IRegistrations registrations)
    {
        _directTypeMapping = new Lazy<IReadOnlyDictionary<Type, IReadOnlySet<Type>>>(() =>
        {
            var dict = new Dictionary<Type, HashSet<Type>>();
            foreach (var concrete in registrations.Items)
            {
                var set = dict.GetOrAdd(concrete.Key);
                foreach (var constructor in concrete.Value.First().Type.GetConstructors())
                {
                    foreach (var param in constructor.GetParameters())
                    {
                        set.Add(param.ParameterType);
                    }
                }
            }

            return dict.ToDictionary<KeyValuePair<Type, HashSet<Type>>, Type, IReadOnlySet<Type>>(
                keySelector: x => x.Key,
                elementSelector: x => x.Value);
        });
    }
}