using System.Collections.Immutable;

namespace Noggog.Autofac.Validation;

public interface ICircularReferenceChecker
{
    void Check();
}

public class CircularReferenceChecker : ICircularReferenceChecker
{
    private readonly IConcreteTypeToDependenciesProvider _concreteTypeToDependenciesProvider;

    public CircularReferenceChecker(
        IConcreteTypeToDependenciesProvider concreteTypeToDependenciesProvider)
    {
        _concreteTypeToDependenciesProvider = concreteTypeToDependenciesProvider;
    }

    public void Check()
    {
        foreach (var item in _concreteTypeToDependenciesProvider.ConcreteTypeMapping)
        {
            Check(item.Key, ImmutableList<Type>.Empty);
        }
    }

    private void Check(Type type, ImmutableList<Type> passed)
    {
        if (passed.Contains(type))
        {
            throw new AutofacValidationException(
                $"Circular dependency detected.  {string.Join(" --> ", passed.SkipWhile(x => x != type).And(type).Select(x => x.Name))}");
        }
            
        if (!_concreteTypeToDependenciesProvider.ConcreteTypeMapping.TryGetValue(type, out var deps)) return;

        passed = passed.Add(type);
        foreach (var dep in deps)
        {
            Check(dep, passed);
        }
    }
}