using System;
using System.Collections.Generic;
using AutoFixture.Xunit2;
using FakeItEasy;
using Noggog.Autofac.Validation;
using Noggog.Testing.AutoFixture;
using Xunit;

namespace CSharpExt.UnitTests.Autofac
{
    public class ValidateAllRegistrationsTests
    {
        [Theory, AutoFakeItEasyData]
        public void Empty(
            [Frozen]IRegistrations registrations,
            ValidateAllRegistrations sut)
        {
            A.CallTo(() => registrations.Items).Returns(new Dictionary<Type, IReadOnlyList<Type>>());
            sut.Check(null);
        }
        
        [Theory, AutoFakeItEasyData(ConfigureMembers: true)]
        public void RespectsShouldSkip(
            Dictionary<Type, IReadOnlyList<Type>> items,
            [Frozen]IRegistrations registrations,
            [Frozen]IShouldSkipType shouldSkipType,
            ValidateAllRegistrations sut)
        {
            A.CallTo(() => registrations.Items).Returns(items);
            A.CallTo(() => shouldSkipType.ShouldSkip(A<Type>._)).Returns(true);
            sut.Check(null);
        }
        
        [Theory, AutoFakeItEasyData(false, ConfigureMembers: true)]
        public void NotInUsages(
            [Frozen]IShouldSkipType shouldSkipType,
            ValidateAllRegistrations sut)
        {
            A.CallTo(() => shouldSkipType.ShouldSkip(A<Type>._)).Returns(false);
            sut.Check(new HashSet<Type>());
        }
        
        [Theory, AutoFakeItEasyData(false, ConfigureMembers: true)]
        public void InUsages(
            [Frozen]IShouldSkipType shouldSkipType,
            [Frozen]IRegistrations registrations,
            [Frozen]IValidateType validateType,
            ValidateAllRegistrations sut)
        {
            A.CallTo(() => shouldSkipType.ShouldSkip(A<Type>._)).Returns(false);
            sut.Check(new HashSet<Type>(registrations.Items.Keys));
            A.CallTo(() => validateType.Check(A<Type>._, default(HashSet<string>?)))
                .MustHaveHappened();
        }
        
        [Theory, AutoFakeItEasyData(false, ConfigureMembers: true)]
        public void NoImplementation(
            [Frozen]IShouldSkipType shouldSkipType,
            [Frozen]IRegistrations registrations,
            ValidateAllRegistrations sut)
        {
            A.CallTo(() => registrations.Items).Returns(new Dictionary<Type, IReadOnlyList<Type>>()
            {
                { typeof(string), new List<Type>() }
            });
            A.CallTo(() => shouldSkipType.ShouldSkip(A<Type>._)).Returns(false);
            Assert.Throws<AutofacValidationException>(() =>
            {
                sut.Check(null);
            });
        }
        
        [Theory, AutoFakeItEasyData(false, ConfigureMembers: true)]
        public void TypicalValidate(
            [Frozen]IShouldSkipType shouldSkipType,
            [Frozen]IValidateType validateType,
            ValidateAllRegistrations sut)
        {
            A.CallTo(() => shouldSkipType.ShouldSkip(A<Type>._)).Returns(false);
            sut.Check(null);
            A.CallTo(() => validateType.Check(A<Type>._, default(HashSet<string>?)))
                .MustHaveHappened();
        }
    }
}