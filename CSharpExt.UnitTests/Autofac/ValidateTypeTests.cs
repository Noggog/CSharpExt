using System;
using System.Collections.Generic;
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
            [Frozen]IIsAllowableEnumerable allowable,
            ValidateType sut)
        {
            A.CallTo(() => allowable.IsAllowed(typeof(Class))).Returns(true);
            sut.Validate(typeof(Class));
            sut.Validate(typeof(Class));
            A.CallTo(() => allowable.IsAllowed(typeof(Class)))
                .MustHaveHappenedOnceExactly();
        }
        
        [Theory, AutoFakeItEasyData(false)]
        public void CheckIfFuncAllowed(
            [Frozen]IValidateTypeCtor validateCtor,
            [Frozen]IRegistrations registrations,
            [Frozen]IIsAllowableFunc allowable,
            ValidateType sut)
        {
            A.CallTo(() => allowable.IsAllowed(typeof(Class))).Returns(true);
            sut.Validate(typeof(Class));
            A.CallTo(() => registrations.Items.ContainsKey(A<Type>._))
                .MustNotHaveHappened();
            A.CallTo(() => validateCtor.Validate(A<Type>._, A<HashSet<string>?>._))
                .MustNotHaveHappened();
        }
        
        [Theory, AutoFakeItEasyData(false)]
        public void CheckIfLazyAllowed(
            [Frozen]IValidateTypeCtor validateCtor,
            [Frozen]IRegistrations registrations,
            [Frozen]IIsAllowableLazy allowable,
            ValidateType sut)
        {
            A.CallTo(() => allowable.IsAllowed(typeof(Class))).Returns(true);
            sut.Validate(typeof(Class));
            A.CallTo(() => registrations.Items.ContainsKey(A<Type>._))
                .MustNotHaveHappened();
            A.CallTo(() => validateCtor.Validate(A<Type>._, A<HashSet<string>?>._))
                .MustNotHaveHappened();
        }
        
        [Theory, AutoFakeItEasyData(false)]
        public void CheckIfEnumerableAllowed(
            [Frozen]IValidateTypeCtor validateCtor,
            [Frozen]IRegistrations registrations,
            [Frozen]IIsAllowableEnumerable allowable,
            ValidateType sut)
        {
            A.CallTo(() => allowable.IsAllowed(typeof(Class))).Returns(true);
            sut.Validate(typeof(Class));
            A.CallTo(() => registrations.Items.ContainsKey(A<Type>._))
                .MustNotHaveHappened();
            A.CallTo(() => validateCtor.Validate(A<Type>._, A<HashSet<string>?>._))
                .MustNotHaveHappened();
        }
        
        [Theory, AutoFakeItEasyData(false)]
        public void CheckIfRegistered(
            [Frozen]IValidateTypeCtor validateCtor,
            [Frozen]IRegistrations registrations,
            ValidateType sut)
        {
            A.CallTo(() => registrations.Items).Returns(new Dictionary<Type, IReadOnlyList<Type>>()
            {
                { typeof (Class), new []{ typeof(string) } },
            });
            sut.Validate(typeof(Class));
            A.CallTo(() => validateCtor.Validate(typeof(string), null))
                .MustHaveHappenedOnceExactly();
        }
        
        [Theory, AutoFakeItEasyData(false)]
        public void CheckIfDelegateFactory(
            [Frozen]IValidateTypeCtor validateCtor,
            [Frozen]IRegistrations registrations,
            [Frozen]ICheckIsDelegateFactory deleg,
            ValidateType sut)
        {
            A.CallTo(() => deleg.Check(typeof(Class))).Returns(true);
            sut.Validate(typeof(Class));
            A.CallTo(() => registrations.Items.ContainsKey(A<Type>._))
                .MustNotHaveHappened();
            A.CallTo(() => validateCtor.Validate(A<Type>._, A<HashSet<string>?>._))
                .MustNotHaveHappened();
        }
    }
}