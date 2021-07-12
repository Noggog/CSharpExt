using System;
using System.Reactive.Concurrency;
using AutoFixture;
using AutoFixture.Kernel;
using FakeItEasy;
using Microsoft.Reactive.Testing;
using Noggog.Reactive;

namespace Noggog.Testing.AutoFixture
{
    public class SchedulerBuilder : ISpecimenBuilder
    {
        private bool _queriedForTestScheduler = false;

        public object Create(object request, ISpecimenContext context)
        {
            if (request is not Type t) return new NoSpecimen();
            if (t == typeof(IScheduler))
            {
                if (_queriedForTestScheduler) return context.Create<TestScheduler>();
                return Scheduler.CurrentThread;
            }
            else if (t == typeof(TestScheduler))
            {
                _queriedForTestScheduler = true;
                return new TestScheduler();
            }
            return new NoSpecimen();
        }
    }
}