using AutoFixture.Kernel;

namespace Noggog.Testing.AutoFixture;

public abstract class SpecimenBuilderFor<T> : ISpecimenBuilder
    where T : notnull
{
    public object Create(object request, ISpecimenContext context)
    {
        if (request is Type t && t == typeof(T))
        {
            return Create(context);
        }
    
        return new NoSpecimen();
    }

    public abstract T Create(ISpecimenContext context);
}