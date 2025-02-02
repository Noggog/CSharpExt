using System.Reactive;
using System.Reactive.Subjects;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Xunit2;
using Noggog.Testing.AutoFixture;
using NSubstitute;
using Shouldly;

namespace CSharpExt.UnitTests.AutoFixture;

public class CustomAutoData : AutoDataAttribute
{
    public CustomAutoData()
        : base(() =>
        {
            var ret = new Fixture();
            ret.Customize(new AutoNSubstituteCustomization());
            ret.Behaviors.Add(new ObservableEmptyBehavior());
            return ret;
        })
    {
    }
}

public interface IServiceWithObservable
{
    IObservable<Unit> Obs { get; }
}
    
public class Sut
{
    public IServiceWithObservable Service { get; }

    public Sut(IServiceWithObservable service)
    {
        Service = service;
        service.Obs.Subscribe(x => throw new NotImplementedException());
    }
}
    
public class ObservableSpecimenCommand
{
    [Theory, CustomAutoData]
    public void DoesNotFireInitially(Sut sut)
    {
    }
        
    [Theory, CustomAutoData]
    public void CanStillBeOverridden(Sut sut)
    {
        Subject<Unit> subj = new();
        sut.Service.Obs.Returns(subj);
        bool received = false;
        sut.Service.Obs.Subscribe(x =>
        {
            received = true;
        });
        subj.OnNext(Unit.Default);
        received.ShouldBeTrue();
    }
}