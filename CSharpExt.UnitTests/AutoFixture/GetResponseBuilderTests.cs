using AutoFixture.Kernel;
using Noggog;
using Noggog.Testing.AutoFixture;
using Noggog.Testing.AutoFixture.Testing;
using Shouldly;

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
            .ShouldBeOfType<NoSpecimen>();
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
            .ShouldBeOfType<GetResponse<int>>();
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
            .Succeeded.ShouldBeTrue();
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
            .Reason.ShouldBe(reason);
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
            .Exception.ShouldBeNull();
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
            .Value.ShouldBe(val);
    }
}