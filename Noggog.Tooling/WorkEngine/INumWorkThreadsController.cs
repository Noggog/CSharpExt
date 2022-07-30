﻿using System.Reactive.Linq;

namespace Noggog.Tooling.WorkEngine;

public interface INumWorkThreadsController
{
    IObservable<int?> NumDesiredThreads { get; }
}

public class NumWorkThreadsUnopinionated : INumWorkThreadsController
{
    public IObservable<int?> NumDesiredThreads => Observable.Return(default(int?));
}