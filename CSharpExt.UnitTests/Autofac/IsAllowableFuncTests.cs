using FluentAssertions;
using Noggog.Autofac.Validation;
using Noggog.Autofac.Validation.Rules;
using Noggog.Testing.AutoFixture;
using NSubstitute;
using Xunit;

namespace CSharpExt.UnitTests.Autofac;

public class IsAllowableFuncTests
{
    [Theory, TestData]
    public void Typical(IsAllowableFunc sut)
    {
        sut.IsAllowed(typeof(Func<string>))
            .Should().BeTrue();
        sut.ValidateTypeCtor.Received(1).Validate(typeof(string), Arg.Any<HashSet<string>?>());
    }
        
    [Theory, TestData]
    public void TooManyArgs(IsAllowableFunc sut)
    {
        sut.IsAllowed(typeof(Func<string, string>))
            .Should().BeFalse();
    }
        
    [Theory, TestData]
    public void NotEnumerable(IsAllowableFunc sut)
    {
        sut.IsAllowed(typeof(string))
            .Should().BeFalse();
    }
}