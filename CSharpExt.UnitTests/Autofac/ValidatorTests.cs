using Noggog;
using Noggog.Autofac.Validation;
using Noggog.Testing.AutoFixture;
using NSubstitute;
using Xunit;

namespace CSharpExt.UnitTests.Autofac;

public class ValidatorTests
{
    [Theory]
    [TestData]
    public void ValidateEverything(Validator sut)
    {
        sut.Registrations.Items.Returns(new Dictionary<Type, IReadOnlyList<Registration>>()
        {
            { typeof(string), new List<Registration>() { new Registration(typeof(int), false) } },
        });
        sut.ShouldSkip.ShouldSkip(Arg.Any<Type>()).Returns(false);
            
        sut.ValidateEverything();
            
        sut.ValidateTypes.Received(1).Validate(Arg.Is<IEnumerable<Type>>(x => x.SequenceEqual(typeof(string))));
        sut.ReferenceChecker.Received(1).Check();
    }
        
    [Theory]
    [TestData]
    public void ValidateEverythingRespectsSkip(Validator sut)
    {
        sut.Registrations.Items.Returns(new Dictionary<Type, IReadOnlyList<Registration>>()
        {
            { typeof(string), new List<Registration>() { new Registration(typeof(int), false) } },
            { typeof(double), new List<Registration>() { new Registration(typeof(float), false) } },
        });
        sut.ShouldSkip.ShouldSkip(Arg.Any<Type>()).Returns(false);
        sut.ShouldSkip.ShouldSkip(typeof(double)).Returns(true);
            
        sut.ValidateEverything();
            
        sut.ValidateTypes.Received(1).Validate(Arg.Is<IEnumerable<Type>>(x => x.SequenceEqual(typeof(string))));
        sut.ReferenceChecker.Received(1).Check();
    }
        
    [Theory, TestData(ConfigureMembers: true)]
    public void Validate(Validator sut)
    {
        sut.Registrations.Items.Returns(new Dictionary<Type, IReadOnlyList<Registration>>()
        {
            { typeof(string), new List<Registration>() { new Registration(typeof(int), true) } },
        });
            
        sut.Validate(typeof(double), typeof(float));
            
        sut.ValidateTypes.Received(1).Validate(Arg.Is<IEnumerable<Type>>(x => x.SequenceEqual(typeof(double), typeof(float))));
        sut.ShouldSkip.DidNotReceiveWithAnyArgs().ShouldSkip(default!);
        sut.ReferenceChecker.Received(1).Check();
    }
}