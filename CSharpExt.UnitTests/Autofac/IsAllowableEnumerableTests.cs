using FluentAssertions;
using Noggog.Autofac.Validation;
using Noggog.Testing.AutoFixture;
using NSubstitute;
using Xunit;

namespace CSharpExt.UnitTests.Autofac;

public class IsAllowableEnumerableTests
{
    [Theory, TestData]
    public void Typical(IsAllowableEnumerable sut)
    {
        sut.IsAllowed(typeof(IEnumerable<string>))
            .Should().BeTrue();

        sut.ValidateTypeCtor.Received(1).Validate(typeof(string), null);
    }
    
    [Theory, TestData]
    public void Array(IsAllowableEnumerable sut)
    {
        sut.IsAllowed(typeof(string[]))
            .Should().BeTrue();

        sut.ValidateTypeCtor.Received(1).Validate(typeof(string), null);
    }
        
    [Theory, TestData]
    public void NotEnumerable(IsAllowableEnumerable sut)
    {
        sut.IsAllowed(typeof(string))
            .Should().BeFalse();
    }
}