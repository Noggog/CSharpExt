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
    public class IsAllowableLazyTests
    {
        [Theory, AutoFakeItEasyData(false)]
        public void Typical([Frozen]IValidateTypeCtor validate, IsAllowableLazy sut)
        {
            sut.IsAllowed(typeof(Lazy<string>))
                .Should().BeTrue();
            A.CallTo(() => validate.Check(typeof(string), A<HashSet<string>?>._))
                .MustHaveHappenedOnceExactly();
        }
        
        [Theory, AutoFakeItEasyData]
        public void NotLazy(IsAllowableLazy sut)
        {
            sut.IsAllowed(typeof(string))
                .Should().BeFalse();
        }
    }
}