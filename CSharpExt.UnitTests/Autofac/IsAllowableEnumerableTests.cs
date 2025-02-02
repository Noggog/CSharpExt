using Noggog.Autofac.Validation.Rules;
using Noggog.Testing.AutoFixture;
using NSubstitute;
using Shouldly;

namespace CSharpExt.UnitTests.Autofac;

public class IsAllowableEnumerableTests
{
    [Theory, TestData]
    public void Typical(IsAllowableEnumerable sut)
    {
        sut.IsAllowed(typeof(IEnumerable<string>))
            .ShouldBeTrue();

        sut.ValidateTypeCtor.Received(1).Validate(typeof(string), null);
    }
    
    [Theory, TestData]
    public void Array(IsAllowableEnumerable sut)
    {
        sut.IsAllowed(typeof(string[]))
            .ShouldBeTrue();

        sut.ValidateTypeCtor.Received(1).Validate(typeof(string), null);
    }
        
    [Theory, TestData]
    public void NotEnumerable(IsAllowableEnumerable sut)
    {
        sut.IsAllowed(typeof(string))
            .ShouldBeFalse();
    }
}