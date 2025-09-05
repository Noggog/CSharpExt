using AutoFixture;
using AutoFixture.Kernel;

namespace Noggog.Testing.AutoFixture;

public class PercentBuilder : ISpecimenBuilder
{
    private readonly Random _random = new();
    
    public object Create(object request, ISpecimenContext context)
    {
        if (request is Type t
            && t == typeof(Percent))
        {
            return new Percent(_random.NextDouble());
        }
        return new NoSpecimen();
    }
}