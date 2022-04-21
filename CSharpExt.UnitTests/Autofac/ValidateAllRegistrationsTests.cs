using Noggog;
using Noggog.Autofac.Validation;
using Noggog.Testing.AutoFixture;
using NSubstitute;
using Xunit;

namespace CSharpExt.UnitTests.Autofac;

public class ValidateAllRegistrationsTests
{
    [Theory, TestData]
    public void Empty(ValidateTypes sut)
    {
        sut.Registrations.Items.Returns(new Dictionary<Type, IReadOnlyList<Registration>>());
        sut.Validate(Enumerable.Empty<Type>());
    }
        
    [Theory, TestData(ConfigureMembers: true)]
    public void NoImplementation(ValidateTypes sut)
    {
        sut.Registrations.Items.Returns(new Dictionary<Type, IReadOnlyList<Registration>>()
        {
            { typeof(string), new List<Registration>() }
        });
        Assert.Throws<AutofacValidationException>(() =>
        {
            sut.Validate(typeof(string).AsEnumerable());
        });
    }
        
    [Theory, TestData(ConfigureMembers: true)]
    public void TypicalValidate(ValidateTypes sut)
    {
        sut.Registrations.Items.Returns(new Dictionary<Type, IReadOnlyList<Registration>>()
        {
            { typeof(string), new List<Registration>() { new Registration(typeof(int), false) } }
        });
        sut.Validate(typeof(string).AsEnumerable());
        sut.TypeCtor.Received(1).Validate(typeof(int), default(HashSet<string>?));
    }
}