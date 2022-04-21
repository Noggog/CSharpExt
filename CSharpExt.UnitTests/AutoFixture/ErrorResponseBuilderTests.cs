using AutoFixture.Kernel;
using FluentAssertions;
using Noggog;
using Noggog.Testing.AutoFixture;
using Noggog.Testing.AutoFixture.Testing;
using Xunit;

namespace CSharpExt.UnitTests.AutoFixture;

public class ErrorResponseBuilderTests
{
    [Theory, BasicAutoData]
    public void UnrelatedTypeReturnsNoSpecimen(
        string reason,
        ISpecimenContext context,
        ErrorResponseBuilder sut)
    {
        context.MockToReturn(reason);
        sut.Create(typeof(string), context)
            .Should().BeOfType<NoSpecimen>();
    }
        
    [Theory, BasicAutoData]
    public void ReturnsErrorResponse(
        string reason,
        ISpecimenContext context,
        ErrorResponseBuilder sut)
    {
        context.MockToReturn(reason);
        sut.Create(typeof(ErrorResponse), context)
            .Should().BeOfType<ErrorResponse>();
    }
        
    [Theory, BasicAutoData]
    public void ReturnsSuccessful(
        string reason,
        ISpecimenContext context,
        ErrorResponseBuilder sut)
    {
        context.MockToReturn(reason);
        ((ErrorResponse)sut.Create(typeof(ErrorResponse), context))
            .Succeeded.Should().BeTrue();
    }
        
    [Theory, BasicAutoData]
    public void QueriesContextForReason(
        string reason,
        ISpecimenContext context,
        ErrorResponseBuilder sut)
    {
        context.MockToReturn(reason);
        ((ErrorResponse)sut.Create(typeof(ErrorResponse), context))
            .Reason.Should().Be(reason);
    }
        
    [Theory, BasicAutoData]
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