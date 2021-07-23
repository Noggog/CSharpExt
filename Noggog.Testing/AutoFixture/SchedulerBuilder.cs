using System;
using System.Reactive.Concurrency;
using AutoFixture;
using AutoFixture.Kernel;
using Microsoft.Reactive.Testing;
using Noggog.Reactive;

namespace Noggog.Testing.AutoFixture
{
    public class SchedulerBuilder : ISpecimenBuilder
    {
        private TestScheduler? _testScheduler;

        public object Create(object request, ISpecimenContext context)
        {
            if (request is not Type t) return new NoSpecimen();
            if (t == typeof(IScheduler))
            {
                if (_testScheduler != null) return _testScheduler;
                return Scheduler.CurrentThread;
            }
            else if (t == typeof(TestScheduler))
            {
                if (_testScheduler == null)
                {
                    _testScheduler = new TestScheduler();
                }
                return _testScheduler;
            }
            else if (t == typeof(ISchedulerProvider))
            {
                if (_testScheduler != null)
                {
                    return new SchedulerProviderInjection(_testScheduler);
                }
                return new SchedulerProviderInjection(Scheduler.CurrentThread);
            }
            return new NoSpecimen();
        }
    }

    public class SchedulerProviderInjection : ISchedulerProvider
    {
        private readonly IScheduler _scheduler;
        public IScheduler MainThread => _scheduler;
        public IScheduler TaskPool => _scheduler;

        public SchedulerProviderInjection(IScheduler scheduler)
        {
            _scheduler = scheduler;
        }
    }
}