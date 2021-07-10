using System;
using System.Collections.Generic;
using AutoFixture.Xunit2;
using FakeItEasy;
using FluentAssertions;
using Noggog.Autofac.Validation;
using Noggog.Testing.AutoFixture;
using Noggog.Testing.FakeItEasy;
using Xunit;

namespace CSharpExt.UnitTests.Autofac
{
    public class CheckIsDelegateFactoryTests
    {
        class ClassWithFactory
        {
            public delegate ClassWithFactory Factory(string str, int i);
            
            public ClassWithFactory(string str, int i, string otherParam)
            {
            }
        }
        
        [Theory, AutoFakeItEasyData]
        public void Typical([Frozen]IValidateType validate, CheckIsDelegateFactory sut)
        {
            A.CallTo(() => validate.Check(A<Type>._, A<HashSet<string>>._)).DoesNothing();
            sut.Check(typeof(ClassWithFactory.Factory))
                .Should().BeTrue();
            var set = new HashSet<string>()
            {
                "str",
                "i"
            };
            A.CallTo(() => validate.Check(typeof(ClassWithFactory), A<HashSet<string>>.That.SetEquals("str", "i")))
                .MustHaveHappenedOnceExactly(); 
        }
        
        [Theory, AutoFakeItEasyData]
        public void RandomType(CheckIsDelegateFactory sut)
        {
            sut.Check(typeof(string))
                .Should().BeFalse();
        }
    }
}