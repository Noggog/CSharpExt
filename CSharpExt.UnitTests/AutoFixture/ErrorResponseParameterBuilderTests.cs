using AutoFixture.Kernel;
using Noggog;
using Noggog.Testing.AutoFixture;
using Noggog.Testing.AutoFixture.Testing;
using Noggog.Testing.Extensions;
using NSubstitute;
using Shouldly;

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
        var param = typeof(NonInterestingClass).GetMethods().First().GetParameters().First();
        ErrorResponse resp = (ErrorResponse)sut.Create(param, context);
        context.ShouldHaveCreated<ErrorResponse>();
        resp.ShouldBe(err);
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
            resp.Succeeded.ShouldBeFalse();
            resp.Reason.ShouldBe(errString);
        }
    }
        
    [Theory, DefaultAutoData]
    public void FailDoesNotReturnException(
        string errString,
        ISpecimenContext context,
        ErrorResponseParameterBuilder sut)
    {
        context.MockToReturn(errString);
        foreach (var method in typeof(Fails).GetMethods())
        {
            if (method.Name is not nameof(Fails.Prefix) and not nameof(Fails.Suffix) and not nameof(Fails.Sandwich))
            {
                continue;
            }
            var param = method.GetParameters().First();
            context.ClearReceivedCalls();
            ErrorResponse resp = (ErrorResponse)sut.Create(param, context);
            resp.Exception.ShouldBeNull();
        }
    }
}