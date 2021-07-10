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
    public class IsAllowableEnumerableTests
    {
        [Theory, AutoFakeItEasyData(false)]
        public void Typical(
            [Frozen]IValidateType validate,
            IsAllowableEnumerable sut)
        {
            sut.IsAllowed(typeof(IEnumerable<string>))
                .Should().BeTrue();
            
            A.CallTo(() => validate.Check(typeof(string), null))
                .MustHaveHappenedOnceExactly();
        }
        
        [Theory, AutoFakeItEasyData]
        public void NotEnumerable(IsAllowableEnumerable sut)
        {
            sut.IsAllowed(typeof(string))
                .Should().BeFalse();
        }
    }
}