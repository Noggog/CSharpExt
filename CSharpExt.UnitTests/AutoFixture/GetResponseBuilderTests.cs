using AutoFixture.Kernel;
using FluentAssertions;
using Noggog;
using Noggog.Testing.AutoFixture;
using Noggog.Testing.AutoFixture.Testing;

namespace CSharpExt.UnitTests.AutoFixture;

public class GetResponseBuilderTests
{
    [Theory, DefaultAutoData]
    public void UnrelatedTypeReturnsNoSpecimen(
        string reason,
        int val,
        ISpecimenContext context,
        GetResponseBuilder sut)
    {
        context.MockToReturn(reason);
        context.MockToReturn(val);
        sut.Create(typeof(string), context)
            .Should().BeOfType<NoSpecimen>();
    }
        
    [Theory, DefaultAutoData]
    public void ReturnsGetResponse(
        string reason,
        int val,
        ISpecimenContext context,
        GetResponseBuilder sut)
    {
        context.MockToReturn(reason);
        context.MockToReturn(val);
        sut.Create(typeof(GetResponse<int>), context)
            .Should().BeOfType<GetResponse<int>>();
    }
        
    [Theory, DefaultAutoData]
    public void ReturnsSuccessful(
        string reason,
        int val,
        ISpecimenContext context,
        GetResponseBuilder sut)
    {
        context.MockToReturn(reason);
        context.MockToReturn(val);
        ((GetResponse<int>)sut.Create(typeof(GetResponse<int>), context))
            .Succeeded.Should().BeTrue();
    }
        
    [Theory, DefaultAutoData]
    public void QueriesContextForReason(
        string reason,
        int val,
        ISpecimenContext context,
        GetResponseBuilder sut)
    {
        context.MockToReturn(reason);
        context.MockToReturn(val);
        ((GetResponse<int>)sut.Create(typeof(GetResponse<int>), context))
            .Reason.Should().Be(reason);
    }
        
    [Theory, DefaultAutoData]
    public void ExceptionNull(
        string reason,
        int val,
        ISpecimenContext context,
        GetResponseBuilder sut)
    {
        context.MockToReturn(reason);
        context.MockToReturn(val);
        ((GetResponse<int>)sut.Create(typeof(GetResponse<int>), context))
            .Exception.Should().BeNull();
    }
        
    [Theory, DefaultAutoData]
    public void QueriesContextForItem(
        string reason,
        int val,
        ISpecimenContext context,
        GetResponseBuilder sut)
    {
        context.MockToReturn(reason);
        context.MockToReturn(val);
        ((GetResponse<int>)sut.Create(typeof(GetResponse<int>), context))
            .Value.Should().Be(val);
    }
}