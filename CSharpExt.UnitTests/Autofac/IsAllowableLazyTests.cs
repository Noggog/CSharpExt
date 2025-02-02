using Noggog.Autofac.Validation.Rules;
using Noggog.Testing.AutoFixture;
using NSubstitute;
using Shouldly;

namespace CSharpExt.UnitTests.Autofac;

public class IsAllowableLazyTests
{
    [Theory, TestData]
    public void Typical(IsAllowableLazy sut)
    {
        sut.IsAllowed(typeof(Lazy<string>))
            .ShouldBeTrue();
        sut.ValidateTypeCtor.Received(1).Validate(typeof(string), Arg.Any<HashSet<string>>());
    }
        
    [Theory, TestData]
    public void NotLazy(IsAllowableLazy sut)
    {
        sut.IsAllowed(typeof(string))
            .ShouldBeFalse();
    }
}