using System.Collections.Generic;
using FluentAssertions;
using Noggog;
using Noggog.Autofac.Validation;
using Noggog.Testing.AutoFixture;
using NSubstitute;
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
        
        [Theory, TestData]
        public void Typical(CheckIsDelegateFactory sut)
        {
            sut.Check(typeof(ClassWithFactory.Factory))
                .Should().BeTrue();
            var set = new HashSet<string>()
            {
                "str",
                "i"
            };
            sut.ValidateTypeCtor.Received(1).Validate(typeof(ClassWithFactory),
                Arg.Is<HashSet<string>>(x => x.SetEquals("str", "i")));
            sut.ValidateType.Received(1).Validate(typeof(ClassWithFactory), false);
        }
        
        [Theory, TestData]
        public void RandomType(CheckIsDelegateFactory sut)
        {
            sut.Check(typeof(string))
                .Should().BeFalse();
        }
    }
}