using System;
using System.Collections.Generic;
using AutoFixture.Xunit2;
using FakeItEasy;
using FluentAssertions;
using Noggog;
using Noggog.Autofac.Validation;
using Noggog.Testing.AutoFixture;
using Noggog.Testing.FakeItEasy;
using Xunit;

namespace CSharpExt.UnitTests.Autofac
{
    public class ValidatorTests
    {
        [Theory]
        [AutoFakeItEasyData(false)]
        public void ValidateEverything(
            [Frozen]ICircularReferenceChecker circularReferenceChecker,
            [Frozen]IRegistrations registrations,
            [Frozen]IShouldSkipType shouldSkipType,
            [Frozen]IValidateTypes validateTypes,
            Validator sut)
        {
            A.CallTo(() => registrations.Items).Returns(new Dictionary<Type, IReadOnlyList<Type>>()
            {
                { typeof(string), new List<Type>() { typeof(int) } },
            });
            A.CallTo(() => shouldSkipType.ShouldSkip(A<Type>._)).Returns(false);
            sut.ValidateEverything();
            A.CallTo(() => validateTypes.Validate(A<IEnumerable<Type>>.That.IsSameSequenceAs(typeof(string))))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => circularReferenceChecker.Check()).MustHaveHappenedOnceExactly();
        }
        
        [Theory]
        [AutoFakeItEasyData(false)]
        public void ValidateEverythingRespectsSkip(
            [Frozen]ICircularReferenceChecker circularReferenceChecker,
            [Frozen]IRegistrations registrations,
            [Frozen]IShouldSkipType shouldSkipType,
            [Frozen]IValidateTypes validateTypes,
            Validator sut)
        {
            A.CallTo(() => registrations.Items).Returns(new Dictionary<Type, IReadOnlyList<Type>>()
            {
                { typeof(string), new List<Type>() { typeof(int) } },
                { typeof(double), new List<Type>() { typeof(float) } },
            });
            A.CallTo(() => shouldSkipType.ShouldSkip(A<Type>._)).Returns(false);
            A.CallTo(() => shouldSkipType.ShouldSkip(typeof(double))).Returns(true);
            sut.ValidateEverything();
            A.CallTo(() => validateTypes.Validate(A<IEnumerable<Type>>.That.IsSameSequenceAs(typeof(string))))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => circularReferenceChecker.Check()).MustHaveHappenedOnceExactly();
        }
        
        [Theory, AutoFakeItEasyData(false, ConfigureMembers: true)]
        public void Validate(
            [Frozen]ICircularReferenceChecker circularReferenceChecker,
            [Frozen]IRegistrations registrations,
            [Frozen]IShouldSkipType shouldSkipType,
            [Frozen]IValidateTypes validateTypes,
            Validator sut)
        {
            A.CallTo(() => registrations.Items).Returns(new Dictionary<Type, IReadOnlyList<Type>>()
            {
                { typeof(string), new List<Type>() { typeof(int) } },
            });
            sut.Validate(typeof(double), typeof(float));
            A.CallTo(() => validateTypes.Validate(A<IEnumerable<Type>>.That.IsSameSequenceAs(typeof(double), typeof(float))))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => shouldSkipType.ShouldSkip(A<Type>._)).MustNotHaveHappened();
            A.CallTo(() => circularReferenceChecker.Check()).MustHaveHappenedOnceExactly();
        }
    }
}