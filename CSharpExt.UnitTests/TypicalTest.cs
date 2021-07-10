using System.Reactive.Concurrency;
using AutoFixture;
using AutoFixture.AutoFakeItEasy;
using AutoFixture.Kernel;
using FakeItEasy;
using Noggog.Reactive;
using Noggog.Testing.AutoFixture;

namespace CSharpExt.UnitTests
{
    public class TypicalTest
    {
        public ISpecimenBuilder Fixture { get; }

        public TypicalTest()
        {
            var fixture = new AutoFixture.Fixture();
            fixture.Customize(
                new AutoFakeItEasyCustomization()
                {
                    Relay = new FakeItEasyStrictRelay()
                });
            var scheduler = A.Fake<ISchedulerProvider>();
            A.CallTo(() => scheduler.TaskPool).Returns(Scheduler.CurrentThread);
            fixture.Register(() => scheduler);
            Fixture = fixture;
        }
    }
}