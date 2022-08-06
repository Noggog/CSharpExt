using AutoFixture.Kernel;
using FluentAssertions;
using Noggog;
using Noggog.Testing.AutoFixture;
using Noggog.Testing.AutoFixture.Testing;
using NSubstitute;
using Xunit;

namespace CSharpExt.UnitTests.AutoFixture;

public class ErrorResponseParameterBuilderTests
{
    class NonInterestingClass
    {
        public void Test(ErrorResponse something)
        {
        }
    }
        
    [Theory, DefaultAutoData]
    public void UninterestingNameQueriesContext(
        ErrorResponse err,
        ISpecimenContext context,
        ErrorResponseParameterBuilder sut)
    {
        context.MockToReturn(err);
        var param = typeof(NonInterestingClass).Methods().First().GetParameters().First();
        ErrorResponse resp = (ErrorResponse)sut.Create(param, context);
        context.ShouldHaveCreated<ErrorResponse>();
        resp.Should().Be(err);
    }
        
    class Fails
    {
        public void Prefix(ErrorResponse failSomething)
        {
        }
            
        public void Suffix(ErrorResponse somethingFail)
        {
        }
            
        public void Sandwich(ErrorResponse somethingFailSomething)
        {
        }
    }
        
    [Theory, DefaultAutoData]
    public void FailReturnsFail(
        string errString,
        ISpecimenContext context,
        ErrorResponseParameterBuilder sut)
    {
        context.MockToReturn(errString);
        foreach (var method in typeof(Fails).Methods())
        {
            var param = method.GetParameters().First();
            context.ClearReceivedCalls();
            ErrorResponse resp = (ErrorResponse)sut.Create(param, context);
            resp.Succeeded.Should().BeFalse();
            resp.Reason.Should().Be(errString);
        }
    }
        
    [Theory, DefaultAutoData]
    public void FailDoesNotReturnException(
        string errString,
        ISpecimenContext context,
        ErrorResponseParameterBuilder sut)
    {
        context.MockToReturn(errString);
        foreach (var method in typeof(Fails).Methods())
        {
            var param = method.GetParameters().First();
            context.ClearReceivedCalls();
            ErrorResponse resp = (ErrorResponse)sut.Create(param, context);
            resp.Exception.Should().BeNull();
        }
    }
}