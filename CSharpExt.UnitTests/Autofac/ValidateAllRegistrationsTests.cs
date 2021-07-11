using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture.Xunit2;
using FakeItEasy;
using Noggog;
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
            ValidateTypes sut)
        {
            A.CallTo(() => registrations.Items).Returns(new Dictionary<Type, IReadOnlyList<Type>>());
            sut.Validate(Enumerable.Empty<Type>());
        }
        
        [Theory, AutoFakeItEasyData(false, ConfigureMembers: true)]
        public void NoImplementation(
            [Frozen]IRegistrations registrations,
            ValidateTypes sut)
        {
            A.CallTo(() => registrations.Items).Returns(new Dictionary<Type, IReadOnlyList<Type>>()
            {
                { typeof(string), new List<Type>() }
            });
            Assert.Throws<AutofacValidationException>(() =>
            {
                sut.Validate(typeof(string).AsEnumerable());
            });
        }
        
        [Theory, AutoFakeItEasyData(false, ConfigureMembers: true)]
        public void TypicalValidate(
            [Frozen]IRegistrations registrations,
            [Frozen]IValidateType validateType,
            ValidateTypes sut)
        {
            A.CallTo(() => registrations.Items).Returns(new Dictionary<Type, IReadOnlyList<Type>>()
            {
                { typeof(string), new List<Type>() { typeof(int) } }
            });
            sut.Validate(typeof(string).AsEnumerable());
            A.CallTo(() => validateType.Check(typeof(int), default(HashSet<string>?)))
                .MustHaveHappened();
        }
    }
}