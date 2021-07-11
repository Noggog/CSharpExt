using AutoFixture.Xunit2;
using FakeItEasy;
using Noggog.Autofac.Validation;
using Noggog.Testing.AutoFixture;
using Xunit;

namespace CSharpExt.UnitTests.Autofac
{
    public class ValidateTypeTests
    {
        class Class
        {
        }

        [Theory, AutoFakeItEasyData(false)]
        public void OnlyProcessesSameTypeOnce(
            [Frozen]IRegistrations registrations,
            ValidateType sut)
        {
            A.CallTo(() => registrations.Items.ContainsKey(typeof(Class))).Returns(true);
            sut.Validate(typeof(Class));
            sut.Validate(typeof(Class));
            A.CallTo(() => registrations.Items.ContainsKey(typeof(Class)))
                .MustHaveHappenedOnceExactly();
        }
        
        [Theory, AutoFakeItEasyData(false)]
        public void CheckIfFuncAllowed([Frozen]IIsAllowableFunc allowable, ValidateType sut)
        {
            A.CallTo(() => allowable.IsAllowed(typeof(Class))).Returns(true);
            sut.Validate(typeof(Class));
        }
        
        [Theory, AutoFakeItEasyData(false)]
        public void CheckIfLazyAllowed([Frozen]IIsAllowableLazy allowable, ValidateType sut)
        {
            A.CallTo(() => allowable.IsAllowed(typeof(Class))).Returns(true);
            sut.Validate(typeof(Class));
        }
        
        [Theory, AutoFakeItEasyData(false)]
        public void CheckIfEnumerableAllowed([Frozen]IIsAllowableEnumerable allowable, ValidateType sut)
        {
            A.CallTo(() => allowable.IsAllowed(typeof(Class))).Returns(true);
            sut.Validate(typeof(Class));
        }
        
        [Theory, AutoFakeItEasyData(false)]
        public void CheckIfRegistered([Frozen]IRegistrations registrations, ValidateType sut)
        {
            A.CallTo(() => registrations.Items.ContainsKey(typeof(Class))).Returns(true);
            sut.Validate(typeof(Class));
        }
        
        [Theory, AutoFakeItEasyData(false)]
        public void CheckIfDelegateFactory([Frozen]ICheckIsDelegateFactory registrations, ValidateType sut)
        {
            A.CallTo(() => registrations.Check(typeof(Class))).Returns(true);
            sut.Validate(typeof(Class));
        }
    }
}