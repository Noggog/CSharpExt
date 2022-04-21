using Noggog.Autofac.Validation;
using Noggog.Testing.AutoFixture;
using NSubstitute;
using Xunit;

namespace CSharpExt.UnitTests.Autofac;

public class ValidateTypeCtorTests
{
    class NoCtor
    {
    }
        
    class ValidClass
    {
        public ValidClass(NoCtor cl)
        {
        }
    }
        
    class MultipleCtor
    {
        public MultipleCtor(NoCtor cl)
        {
        }
            
        public MultipleCtor()
        {
        }
    }
        
    class OptionalClass
    {
        public OptionalClass(string? str = null)
        {
        }
    }

    [Theory, TestData]
    public void NoShortCircuit(ValidateTypeCtor sut)
    {
        sut.ShouldSkip.ShouldSkip(Arg.Any<Type>()).Returns(true);
            
        sut.Validate(typeof(ValidClass));
        sut.Validate(typeof(ValidClass), new HashSet<string>() { "test" });
        sut.Validate(typeof(ValidClass), new HashSet<string>() { "test2" });

        sut.ShouldSkip.ReceivedWithAnyArgs(3).ShouldSkip(default!);
    }
        
    [Theory, TestData]
    public void RespectsShouldSkip(ValidateTypeCtor sut)
    {
        sut.ShouldSkip.ShouldSkip(Arg.Any<Type>()).Returns(true);
        sut.Validate(typeof(ValidClass));
    }
        
    [Theory, TestData]
    public void MultipleCtors(ValidateTypeCtor sut)
    {
        sut.ShouldSkip.ShouldSkip(Arg.Any<Type>()).Returns(false);
        Assert.Throws<AutofacValidationException>(() =>
        {
            sut.Validate(typeof(MultipleCtor));
        });
    }
        
    [Theory, TestData]
    public void NoCtors(ValidateTypeCtor sut)
    {
        sut.ShouldSkip.ShouldSkip(Arg.Any<Type>()).Returns(false);
        sut.Validate(typeof(NoCtor));
    }
        
    [Theory, TestData]
    public void Optional(ValidateTypeCtor sut)
    {
        sut.ShouldSkip.ShouldSkip(Arg.Any<Type>()).Returns(false);
        sut.Validate(typeof(OptionalClass));
    }
        
    [Theory, TestData]
    public void ParamSkipped(ValidateTypeCtor sut)
    {
        sut.ShouldSkip.ShouldSkip(Arg.Any<Type>()).Returns(false);
        sut.Validate(typeof(ValidClass), new HashSet<string>()
        {
            "cl"
        });
    }
}