using FluentAssertions;
using Noggog.Autofac.Validation.Rules;
using Noggog.Testing.AutoFixture;
using NSubstitute;

namespace CSharpExt.UnitTests.Autofac;

public class IsAllowableLazyTests
{
    [Theory, TestData]
    public void Typical(IsAllowableLazy sut)
    {
        sut.IsAllowed(typeof(Lazy<string>))
            .Should().BeTrue();
        sut.ValidateTypeCtor.Received(1).Validate(typeof(string), Arg.Any<HashSet<string>>());
    }
        
    [Theory, TestData]
    public void NotLazy(IsAllowableLazy sut)
    {
        sut.IsAllowed(typeof(string))
            .Should().BeFalse();
    }
}