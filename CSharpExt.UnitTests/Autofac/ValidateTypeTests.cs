using System;
using System.Collections.Generic;
using AutoFixture.Xunit2;
using FakeItEasy;
using FluentAssertions;
using Noggog.Autofac.Validation;
using Noggog.Testing.AutoFixture;
using Xunit;

namespace CSharpExt.UnitTests.Autofac
{
    public class ValidateTypeTests : TypicalTest
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
        
        [Theory, AutoFakeItEasyData]
        public void RespectsShouldSkip([Frozen]IShouldSkipType shouldSkip, ValidateType sut)
        {
            A.CallTo(() => shouldSkip.ShouldSkip(typeof(ValidClass))).Returns(true);
            sut.Check(typeof(ValidClass));
        }
        
        [Theory, AutoFakeItEasyData]
        public void MultipleCtors([Frozen]IShouldSkipType shouldSkip, ValidateType sut)
        {
            A.CallTo(() => shouldSkip.ShouldSkip(A<Type>._)).Returns(false);
            Assert.Throws<AutofacValidationException>(() =>
            {
                sut.Check(typeof(MultipleCtor));
            });
        }
        
        [Theory, AutoFakeItEasyData]
        public void NoCtors([Frozen]IShouldSkipType shouldSkip, ValidateType sut)
        {
            A.CallTo(() => shouldSkip.ShouldSkip(A<Type>._)).Returns(false);
            sut.Check(typeof(NoCtor));
        }
        
        [Theory, AutoFakeItEasyData]
        public void Optional([Frozen]IShouldSkipType shouldSkip, ValidateType sut)
        {
            A.CallTo(() => shouldSkip.ShouldSkip(A<Type>._)).Returns(false);
            sut.Check(typeof(OptionalClass));
        }
        
        [Theory, AutoFakeItEasyData]
        public void ParamSkipped([Frozen]IShouldSkipType shouldSkip, ValidateType sut)
        {
            A.CallTo(() => shouldSkip.ShouldSkip(A<Type>._)).Returns(false);
            sut.Check(typeof(ValidClass), new HashSet<string>()
            {
                "cl"
            });
        }
        
        [Theory, AutoFakeItEasyData(false)]
        public void CheckIfFuncAllowed([Frozen]IIsAllowableFunc allowable, ValidateType sut)
        {
            A.CallTo(() => allowable.IsAllowed(typeof(NoCtor))).Returns(true);
            sut.Check(typeof(ValidClass));
        }
        
        [Theory, AutoFakeItEasyData(false)]
        public void CheckIfLazyAllowed([Frozen]IIsAllowableLazy allowable, ValidateType sut)
        {
            A.CallTo(() => allowable.IsAllowed(typeof(NoCtor))).Returns(true);
            sut.Check(typeof(ValidClass));
        }
        
        [Theory, AutoFakeItEasyData(false)]
        public void CheckIfEnumerableAllowed([Frozen]IIsAllowableEnumerable allowable, ValidateType sut)
        {
            A.CallTo(() => allowable.IsAllowed(typeof(NoCtor))).Returns(true);
            sut.Check(typeof(ValidClass));
        }
        
        [Theory, AutoFakeItEasyData(false)]
        public void CheckIfRegistered([Frozen]IRegistrations registrations, ValidateType sut)
        {
            A.CallTo(() => registrations.Items.ContainsKey(typeof(NoCtor))).Returns(true);
            sut.Check(typeof(ValidClass));
        }
        
        [Theory, AutoFakeItEasyData(false)]
        public void CheckIfDelegateFactory([Frozen]ICheckIsDelegateFactory registrations, ValidateType sut)
        {
            A.CallTo(() => registrations.Check(typeof(NoCtor))).Returns(true);
            sut.Check(typeof(ValidClass));
        }
    }
}