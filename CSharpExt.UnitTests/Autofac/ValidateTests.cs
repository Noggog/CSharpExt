using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using AutoFixture.Xunit2;
using FakeItEasy;
using Noggog;
using Noggog.Autofac.Validation;
using Noggog.Testing.AutoFixture;
using Noggog.Testing.FakeItEasy;
using Xunit;

namespace CSharpExt.UnitTests.Autofac
{
    public class ValidateTests
    {
        [Theory]
        [AutoFakeItEasyData(false)]
        public void NotEvaluatingUsages(
            [Frozen]IGetUsages getUsages,
            Validate validate)
        {
            validate.ValidateRegistrations(evaluateUsages: false, new []{ typeof(string) });
            validate.ValidateRegistrations(evaluateUsages: false, Array.Empty<Type>());
            A.CallTo(() => getUsages.Get(A<Type[]>._)).MustNotHaveHappened();
        }
        
        [Theory, AutoFakeItEasyData(false, ConfigureMembers: true)]
        public void EvaluatingUsages(
            [Frozen]IGetUsages getUsages, 
            [Frozen]IRegistrations getRegistrations, 
            [Frozen]IValidateAllRegistrations validateAllRegistrations,
            HashSet<Type> usageRet,
            Validate validate)
        {
            var expectedTypes = getRegistrations.Items.Keys.ToArray();
            A.CallTo(() => getUsages.Get(A<Type[]>.That.IsSameSequenceAs(expectedTypes))).Returns(usageRet);
            validate.ValidateRegistrations(true, Array.Empty<Type>());
            A.CallTo(() => getRegistrations.Items).MustHaveHappened(2, Times.Exactly);
            A.CallTo(() => getUsages.Get(A<Type[]>.That.IsSameSequenceAs(expectedTypes)))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => validateAllRegistrations.Check(A<HashSet<Type>>.That.SetEquals(usageRet)))
                .MustHaveHappenedOnceExactly();
        }
        
        [Theory, AutoFakeItEasyData(false, ConfigureMembers: true)]
        public void EvaluatingUsagesWithExtra(
            [Frozen]IGetUsages getUsages, 
            [Frozen]IRegistrations getRegistrations, 
            [Frozen]IValidateAllRegistrations validateAllRegistrations,
            HashSet<Type> usageRet,
            Validate validate)
        {
            var expectedTypes = getRegistrations.Items.Keys.ToArray();
            A.CallTo(() => getUsages.Get(A<Type[]>.That.IsSameSequenceAs(expectedTypes))).Returns(usageRet);
            validate.ValidateRegistrations(true, typeof(string));
            A.CallTo(() => getRegistrations.Items).MustHaveHappened(2, Times.Exactly);
            A.CallTo(() => getUsages.Get(A<Type[]>.That.IsSameSequenceAs(expectedTypes)))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => validateAllRegistrations.Check(A<HashSet<Type>>.That.SetEquals(usageRet.And(typeof(string)))))
                .MustHaveHappenedOnceExactly();
        }

    }
}