using AutoFixture;
using AutoFixture.Kernel;

namespace Noggog.Testing.AutoFixture;

public class ErrorResponseBuilder : ISpecimenBuilder
{
    public object Create(object request, ISpecimenContext context)
    {
        if (request is Type t
            && t == typeof(ErrorResponse))
        {
            return ErrorResponse.Create(
                successful: true, 
                context.Create<string>());
        }
        return new NoSpecimen();
    }
}