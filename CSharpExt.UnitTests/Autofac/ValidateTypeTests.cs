using Noggog.Autofac.Validation;
using Noggog.Testing.AutoFixture;
using NSubstitute;
using Xunit;

namespace CSharpExt.UnitTests.Autofac;

public class ValidateTypeTests
{
    class Class
    {
    }
        
    [Theory, TestData]
    public void CheckIfRegistered(ValidateType sut)
    {
        sut.Registrations.Items.Returns(new Dictionary<Type, IReadOnlyList<Registration>>()
        {
            { typeof (Class), new []{ new Registration(typeof(string), true) } },
        });
            
        sut.Validate(typeof(Class));

        sut.ValidateCtor.Received(1).Validate(typeof(string));
    }
        
    [Theory, TestData]
    public void ChecksLastRegistration(ValidateType sut)
    {
        sut.Registrations.Items.Returns(new Dictionary<Type, IReadOnlyList<Registration>>()
        {
            { typeof (Class), new []
            {
                new Registration(typeof(string), true),
                new Registration(typeof(int), true),
            } },
        });
            
        sut.Validate(typeof(Class));

        sut.ValidateCtor.Received(1).Validate(typeof(int));
    }
}