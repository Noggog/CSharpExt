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
    public class IsAllowableFuncTests
    {
        [Theory, AutoFakeItEasyData(false)]
        public void Typical([Frozen]IValidateTypeCtor validate, IsAllowableFunc sut)
        {
            sut.IsAllowed(typeof(Func<string>))
                .Should().BeTrue();
            A.CallTo(() => validate.Check(typeof(string), A<HashSet<string>?>._))
                .MustHaveHappenedOnceExactly();
        }
        
        [Theory, AutoFakeItEasyData]
        public void TooManyArgs(IsAllowableFunc sut)
        {
            sut.IsAllowed(typeof(Func<string, string>))
                .Should().BeFalse();
        }
        
        [Theory, AutoFakeItEasyData]
        public void NotEnumerable(IsAllowableFunc sut)
        {
            sut.IsAllowed(typeof(string))
                .Should().BeFalse();
        }
    }
}