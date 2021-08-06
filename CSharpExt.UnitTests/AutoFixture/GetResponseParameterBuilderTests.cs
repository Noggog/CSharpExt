using System.Linq;
using AutoFixture.Kernel;
using FluentAssertions;
using Noggog;
using Noggog.Testing.AutoFixture;
using Noggog.Testing.AutoFixture.Testing;
using NSubstitute;
using Xunit;

namespace CSharpExt.UnitTests.AutoFixture
{
    public class GetResponseParameterBuilderTests
    {
        class NonInterestingClass
        {
            public void Test(GetResponse<int> something)
            {
            }
        }
        
        [Theory, BasicAutoData]
        public void UninterestingNameQueriesContext(
            GetResponse<int> ret,
            ISpecimenContext context,
            GetResponseParameterBuilder sut)
        {
            context.MockToReturn(ret);
            var param = typeof(NonInterestingClass).Methods().First().GetParameters().First();
            GetResponse<int> resp = (GetResponse<int>)sut.Create(param, context);
            context.ShouldHaveCreated<GetResponse<int>>();
            resp.Should().Be(ret);
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
        
        [Theory, BasicAutoData]
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
                resp.Succeeded.Should().BeFalse();
                resp.Reason.Should().Be(errString);
            }
        }
        
        [Theory, BasicAutoData]
        public void FailDoesNotReturnException(
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
                resp.Exception.Should().BeNull();
            }
        }
    }
}