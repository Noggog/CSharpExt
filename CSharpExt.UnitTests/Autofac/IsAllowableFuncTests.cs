using Noggog.Autofac.Validation.Rules;
using Noggog.Testing.AutoFixture;
using NSubstitute;
using Shouldly;

namespace CSharpExt.UnitTests.Autofac;

public class IsAllowableFuncTests
{
    [Theory, TestData]
    public void Typical(IsAllowableFunc sut)
    {
        sut.IsAllowed(typeof(Func<string>))
            .ShouldBeTrue();
        sut.ValidateTypeCtor.Received(1).Validate(typeof(string), Arg.Any<HashSet<string>?>());
    }
        
    [Theory, TestData]
    public void TooManyArgs(IsAllowableFunc sut)
    {
        sut.IsAllowed(typeof(Func<string, string>))
            .ShouldBeFalse();
    }
        
    [Theory, TestData]
    public void NotEnumerable(IsAllowableFunc sut)
    {
        sut.IsAllowed(typeof(string))
            .ShouldBeFalse();
    }
}