using AutoFixture.Kernel;
using FluentAssertions;
using Noggog;
using Noggog.Testing.AutoFixture;
using Noggog.Testing.AutoFixture.Testing;

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
            .Should().BeOfType<NoSpecimen>();
    }
        
    [Theory, DefaultAutoData]
    public void ReturnsErrorResponse(
        string reason,
        ISpecimenContext context,
        ErrorResponseBuilder sut)
    {
        context.MockToReturn(reason);
        sut.Create(typeof(ErrorResponse), context)
            .Should().BeOfType<ErrorResponse>();
    }
        
    [Theory, DefaultAutoData]
    public void ReturnsSuccessful(
        string reason,
        ISpecimenContext context,
        ErrorResponseBuilder sut)
    {
        context.MockToReturn(reason);
        ((ErrorResponse)sut.Create(typeof(ErrorResponse), context))
            .Succeeded.Should().BeTrue();
    }
        
    [Theory, DefaultAutoData]
    public void QueriesContextForReason(
        string reason,
        ISpecimenContext context,
        ErrorResponseBuilder sut)
    {
        context.MockToReturn(reason);
        ((ErrorResponse)sut.Create(typeof(ErrorResponse), context))
            .Reason.Should().Be(reason);
    }
        
    [Theory, DefaultAutoData]
    public void ExceptionNull(
        string reason,
        ISpecimenContext context,
        ErrorResponseBuilder sut)
    {
        context.MockToReturn(reason);
        ((ErrorResponse)sut.Create(typeof(ErrorResponse), context))
            .Exception.Should().BeNull();
    }
}