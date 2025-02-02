using AutoFixture.Kernel;
using Noggog;
using Noggog.Testing.AutoFixture;
using Noggog.Testing.AutoFixture.Testing;
using Shouldly;

namespace CSharpExt.UnitTests.AutoFixture;

public class ErrorResponseBuilderTests
{
    [Theory, DefaultAutoData]
    public void UnrelatedTypeReturnsNoSpecimen(
        string reason,
        ISpecimenContext context,
        ErrorResponseBuilder sut)
    {
        context.MockToReturn(reason);
        sut.Create(typeof(string), context)
            .ShouldBeOfType<NoSpecimen>();
    }
        
    [Theory, DefaultAutoData]
    public void ReturnsErrorResponse(
        string reason,
        ISpecimenContext context,
        ErrorResponseBuilder sut)
    {
        context.MockToReturn(reason);
        sut.Create(typeof(ErrorResponse), context)
            .ShouldBeOfType<ErrorResponse>();
    }
        
    [Theory, DefaultAutoData]
    public void ReturnsSuccessful(
        string reason,
        ISpecimenContext context,
        ErrorResponseBuilder sut)
    {
        context.MockToReturn(reason);
        ((ErrorResponse)sut.Create(typeof(ErrorResponse), context))
            .Succeeded.ShouldBeTrue();
    }
        
    [Theory, DefaultAutoData]
    public void QueriesContextForReason(
        string reason,
        ISpecimenContext context,
        ErrorResponseBuilder sut)
    {
        context.MockToReturn(reason);
        ((ErrorResponse)sut.Create(typeof(ErrorResponse), context))
            .Reason.ShouldBe(reason);
    }
        
    [Theory, DefaultAutoData]
    public void ExceptionNull(
        string reason,
        ISpecimenContext context,
        ErrorResponseBuilder sut)
    {
        context.MockToReturn(reason);
        ((ErrorResponse)sut.Create(typeof(ErrorResponse), context))
            .Exception.ShouldBeNull();
    }
}