﻿using System;
using System.Collections.Generic;
using AutoFixture.Xunit2;
using FakeItEasy;
using Noggog.Autofac.Validation;
using Noggog.Testing.AutoFixture;
using Xunit;

namespace CSharpExt.UnitTests.Autofac
{
    public class ValidateTypeCtorTests : TypicalTest
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
        public void NoShortCircuit([Frozen]IShouldSkipType shouldSkip, ValidateTypeCtor sut)
        {
            A.CallTo(() => shouldSkip.ShouldSkip(A<Type>._)).Returns(true);
            sut.Validate(typeof(ValidClass));
            sut.Validate(typeof(ValidClass), new HashSet<string>() { "test" });
            sut.Validate(typeof(ValidClass), new HashSet<string>() { "test2" });
            A.CallTo(() => shouldSkip.ShouldSkip(A<Type>._))
                .MustHaveHappened(3, Times.Exactly);
        }
        
        [Theory, AutoFakeItEasyData]
        public void RespectsShouldSkip([Frozen]IShouldSkipType shouldSkip, ValidateTypeCtor sut)
        {
            A.CallTo(() => shouldSkip.ShouldSkip(typeof(ValidClass))).Returns(true);
            sut.Validate(typeof(ValidClass));
        }
        
        [Theory, AutoFakeItEasyData]
        public void MultipleCtors([Frozen]IShouldSkipType shouldSkip, ValidateTypeCtor sut)
        {
            A.CallTo(() => shouldSkip.ShouldSkip(A<Type>._)).Returns(false);
            Assert.Throws<AutofacValidationException>(() =>
            {
                sut.Validate(typeof(MultipleCtor));
            });
        }
        
        [Theory, AutoFakeItEasyData]
        public void NoCtors([Frozen]IShouldSkipType shouldSkip, ValidateTypeCtor sut)
        {
            A.CallTo(() => shouldSkip.ShouldSkip(A<Type>._)).Returns(false);
            sut.Validate(typeof(NoCtor));
        }
        
        [Theory, AutoFakeItEasyData]
        public void Optional([Frozen]IShouldSkipType shouldSkip, ValidateTypeCtor sut)
        {
            A.CallTo(() => shouldSkip.ShouldSkip(A<Type>._)).Returns(false);
            sut.Validate(typeof(OptionalClass));
        }
        
        [Theory, AutoFakeItEasyData]
        public void ParamSkipped([Frozen]IShouldSkipType shouldSkip, ValidateTypeCtor sut)
        {
            A.CallTo(() => shouldSkip.ShouldSkip(A<Type>._)).Returns(false);
            sut.Validate(typeof(ValidClass), new HashSet<string>()
            {
                "cl"
            });
        }
    }
}