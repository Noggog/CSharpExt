﻿using AutoFixture.Kernel;
using Noggog;
using Noggog.Testing.AutoFixture;
using Noggog.Testing.AutoFixture.Testing;
using Noggog.Testing.Extensions;
using NSubstitute;
using Shouldly;

namespace CSharpExt.UnitTests.AutoFixture;

public class GetResponseParameterBuilderTests
{
    class NonInterestingClass
    {
        public void Test(GetResponse<int> something)
        {
        }
    }
        
    [Theory, DefaultAutoData]
    public void UninterestingNameQueriesContext(
        GetResponse<int> ret,
        ISpecimenContext context,
        GetResponseParameterBuilder sut)
    {
        context.MockToReturn(ret);
        var param = typeof(NonInterestingClass).GetMethods().First().GetParameters().First();
        GetResponse<int> resp = (GetResponse<int>)sut.Create(param, context);
        context.ShouldHaveCreated<GetResponse<int>>();
        resp.ShouldBe(ret);
    }
        
    class Fails
    {
        public void Prefix(GetResponse<int> failSomething)
        {
        }
            
        public void Suffix(GetResponse<int> somethingFail)
        {
        }
            
        public void Sandwich(GetResponse<int> somethingFailSomething)
        {
        }
    }
        
    [Theory, DefaultAutoData]
    public void FailReturnsFail(
        string errString,
        ISpecimenContext context,
        GetResponseParameterBuilder sut)
    {
        context.MockToReturn(errString);
        foreach (var method in typeof(Fails).Methods())
        {
            var param = method.GetParameters().First();
            context.ClearReceivedCalls();
            GetResponse<int> resp = (GetResponse<int>)sut.Create(param, context);
            resp.Succeeded.ShouldBeFalse();
            resp.Reason.ShouldBe(errString);
        }
    }
        
    [Theory, DefaultAutoData]
    public void FailDoesNotReturnException(
        string errString,
        ISpecimenContext context,
        GetResponseParameterBuilder sut)
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
            GetResponse<int> resp = (GetResponse<int>)sut.Create(param, context);
            resp.Exception.ShouldBeNull();
        }
    }
}