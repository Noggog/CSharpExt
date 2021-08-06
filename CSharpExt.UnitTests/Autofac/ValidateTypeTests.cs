using System;
using System.Collections.Generic;
using AutoFixture.Xunit2;
using Noggog.Autofac.Validation;
using Noggog.Testing.AutoFixture;
using NSubstitute;
using Xunit;

namespace CSharpExt.UnitTests.Autofac
{
    public class ValidateTypeTests
    {
        class Class
        {
        }

        [Theory, TestData]
        public void OnlyProcessesSameTypeOnce(ValidateType sut)
        {
            sut.AllowableEnumerable.IsAllowed(typeof(Class)).Returns(true);
            
            sut.Validate(typeof(Class));
            sut.Validate(typeof(Class));
            
            sut.AllowableEnumerable.Received(1).IsAllowed(typeof(Class));
        }
        
        [Theory, TestData]
        public void CheckIfFuncAllowed(ValidateType sut)
        {
            sut.AllowableFunc.IsAllowed(typeof(Class)).Returns(true);
            
            sut.Validate(typeof(Class));

            sut.Registrations.Items.DidNotReceiveWithAnyArgs().ContainsKey(default!);
            sut.ValidateCtor.DidNotReceiveWithAnyArgs().Validate(default!);
        }
        
        [Theory, TestData]
        public void CheckIfLazyAllowed(ValidateType sut)
        {
            sut.AllowableLazy.IsAllowed(typeof(Class)).Returns(true);
            
            sut.Validate(typeof(Class));
            
            sut.Registrations.Items.DidNotReceiveWithAnyArgs().ContainsKey(default!);
            sut.ValidateCtor.DidNotReceiveWithAnyArgs().Validate(default!);
        }
        
        [Theory, TestData]
        public void CheckIfEnumerableAllowed(ValidateType sut)
        {
            sut.AllowableEnumerable.IsAllowed(typeof(Class)).Returns(true);
            
            sut.Validate(typeof(Class));
            
            sut.Registrations.Items.DidNotReceiveWithAnyArgs().ContainsKey(default!);
            sut.ValidateCtor.DidNotReceiveWithAnyArgs().Validate(default!);
        }
        
        [Theory, TestData]
        public void CheckIfRegistered(ValidateType sut)
        {
            sut.Registrations.Items.Returns(new Dictionary<Type, IReadOnlyList<Registration>>()
            {
                { typeof (Class), new []{ new Registration(typeof(string), true) } },
            });
            
            sut.Validate(typeof(Class));

            sut.ValidateCtor.Received(1).Validate(typeof(string));
        }
        
        [Theory, TestData]
        public void ChecksLastRegistration(ValidateType sut)
        {
            sut.Registrations.Items.Returns(new Dictionary<Type, IReadOnlyList<Registration>>()
            {
                { typeof (Class), new []
                {
                    new Registration(typeof(string), true),
                    new Registration(typeof(int), true),
                } },
            });
            
            sut.Validate(typeof(Class));

            sut.ValidateCtor.Received(1).Validate(typeof(int));
        }
        
        [Theory, TestData]
        public void CheckIfDelegateFactory(ValidateType sut)
        {
            sut.IsDelegateFactory.Check(typeof(Class)).Returns(true);
            
            sut.Validate(typeof(Class));
            
            sut.Registrations.Items.DidNotReceiveWithAnyArgs().ContainsKey(default!);
            sut.ValidateCtor.DidNotReceiveWithAnyArgs().Validate(default!);
        }
    }
}