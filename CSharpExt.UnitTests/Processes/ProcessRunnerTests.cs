using System.Diagnostics;
using System.Reactive.Linq;
using AutoFixture.Xunit2;
using Noggog.Processes;
using Noggog.Processes.DI;
using Noggog.Testing.AutoFixture;
using Noggog.Testing.Extensions;
using NSubstitute;
using Shouldly;

namespace CSharpExt.UnitTests.Processes;

public class ProcessRunnerTests
{
    #region Run

    [Theory, DefaultAutoData(ConfigureMembers: false)]
    public async Task TypicalRun_StartInfoPassedToFactory(
        [Frozen]ProcessStartInfo startInfo,
        CancellationToken cancel,
        ProcessRunner sut)
    {
        await sut.Run(startInfo, cancel);
        sut.Factory.Received(1).Create(startInfo, cancel);
    }

    [Theory, DefaultAutoData(ConfigureMembers: false)]
    public async Task TypicalRun_FactoryResultIsRunAndReturned(
        int ret,
        [Frozen]ProcessStartInfo startInfo,
        IProcessWrapper process,
        CancellationToken cancel,
        ProcessRunner sut)
    {
        process.Run().Returns(Task.FromResult(ret));
        sut.Factory.Create(default!).ReturnsForAnyArgs(process);
        (await sut.Run(startInfo, cancel))
            .ShouldBe(ret);
    }

    #endregion

    #region RunAndCapture

    [Theory, DefaultAutoData]
    public async Task RunAndCapture_CallsFactory(
        ProcessStartInfo startInfo,
        CancellationToken cancel,
        ProcessRunner sut)
    {
        await sut.RunAndCapture(startInfo, cancel);
        sut.Factory.Received(1).Create(startInfo, cancel);
    }
        
    [Theory, DefaultAutoData(ConfigureMembers: false)]
    public async Task RunAndCapture_PutsOutIntoOut(
        string str,
        [Frozen]ProcessStartInfo startInfo,
        IProcessWrapper process,
        CancellationToken cancel,
        ProcessRunner sut)
    {
        process.Output.Returns(Observable.Return(str));
        sut.Factory.Create(default!).ReturnsForAnyArgs(process);
        var result = await sut.RunAndCapture(startInfo, cancel);
        result.Out.ShouldBe(str);
    }
        
    [Theory, DefaultAutoData(ConfigureMembers: false)]
    public async Task RunAndCapture_PutsErrIntoErr(
        string str,
        [Frozen]ProcessStartInfo startInfo,
        IProcessWrapper process,
        CancellationToken cancel,
        ProcessRunner sut)
    {
        process.Error.Returns(Observable.Return(str));
        sut.Factory.Create(default!).ReturnsForAnyArgs(process);
        var result = await sut.RunAndCapture(startInfo, cancel);
        result.Errors.ShouldBe(str);
    }
        
    [Theory, DefaultAutoData(ConfigureMembers: false)]
    public async Task RunAndCapture_ReturnsProcessReturn(
        int ret,
        [Frozen]ProcessStartInfo startInfo,
        IProcessWrapper process,
        CancellationToken cancel,
        ProcessRunner sut)
    {
        process.Run().Returns(Task.FromResult(ret));
        sut.Factory.Create(default!).ReturnsForAnyArgs(process);
        (await sut.RunAndCapture(startInfo, cancel))
            .Result.ShouldBe(ret);
    }

    #endregion

    #region RunWithCallbacks

    [Theory, DefaultAutoData]
    public async Task RunWithCallbacks_CallsFactory(
        ProcessStartInfo startInfo,
        CancellationToken cancel,
        Action<string> callback,
        ProcessRunner sut)
    {
        await sut.RunWithCallback(startInfo, callback, callback, cancel);
        sut.Factory.Received(1).Create(startInfo, cancel);
    }
        
    [Theory, DefaultAutoData(ConfigureMembers: false)]
    public async Task RunWithCallbacks_CallsOutCallback(
        string str,
        [Frozen]ProcessStartInfo startInfo,
        IProcessWrapper process,
        CancellationToken cancel,
        Action<string> errCb,
        ProcessRunner sut)
    {
        process.Output.Returns(Observable.Return(str));
        sut.Factory.Create(default!).ReturnsForAnyArgs(process);
        var received = new List<string>();
        await sut.RunWithCallback(startInfo, received.Add, errCb, cancel);
        received.ShouldBe(str);
    }
        
    [Theory, DefaultAutoData(ConfigureMembers: false)]
    public async Task RunWithCallbacks_PutsErrIntoErr(
        string str,
        [Frozen]ProcessStartInfo startInfo,
        IProcessWrapper process,
        Action<string> outCb,
        CancellationToken cancel,
        ProcessRunner sut)
    {
        process.Error.Returns(Observable.Return(str));
        sut.Factory.Create(default!).ReturnsForAnyArgs(process);
        var received = new List<string>();
        await sut.RunWithCallback(startInfo, outCb, received.Add, cancel);
        received.ShouldBe(str);
    }
        
    [Theory, DefaultAutoData(ConfigureMembers: false)]
    public async Task RunWithCallbacks_ReturnsProcessReturn(
        int ret,
        [Frozen]ProcessStartInfo startInfo,
        IProcessWrapper process,
        Action<string> outCb,
        Action<string> errCb,
        CancellationToken cancel,
        ProcessRunner sut)
    {
        process.Run().Returns(Task.FromResult(ret));
        sut.Factory.Create(default!).ReturnsForAnyArgs(process);
        (await sut.RunWithCallback(startInfo, outCb, errCb, cancel))
            .ShouldBe(ret);
    }

    #endregion

    #region RunWithCallback


    [Theory, DefaultAutoData]
    public async Task RunWithCallback_CallsFactory(
        ProcessStartInfo startInfo,
        CancellationToken cancel,
        Action<string> callback,
        ProcessRunner sut)
    {
        await sut.RunWithCallback(startInfo, callback, cancel);
        sut.Factory.Received(1).Create(startInfo, cancel);
    }
        
    [Theory, DefaultAutoData(ConfigureMembers: false)]
    public async Task RunWithCallback_CallsOutCallback(
        string str,
        [Frozen]ProcessStartInfo startInfo,
        IProcessWrapper process,
        CancellationToken cancel,
        ProcessRunner sut)
    {
        process.Output.Returns(Observable.Return(str));
        sut.Factory.Create(default!).ReturnsForAnyArgs(process);
        var received = new List<string>();
        await sut.RunWithCallback(startInfo, received.Add, cancel);
        received.ShouldBe(str);
    }
        
    [Theory, DefaultAutoData(ConfigureMembers: false)]
    public async Task RunWithCallback_PutsErrIntoCallback(
        string str,
        [Frozen]ProcessStartInfo startInfo,
        IProcessWrapper process,
        CancellationToken cancel,
        ProcessRunner sut)
    {
        process.Error.Returns(Observable.Return(str));
        sut.Factory.Create(default!).ReturnsForAnyArgs(process);
        var received = new List<string>();
        await sut.RunWithCallback(startInfo, received.Add, cancel);
        received.ShouldBe(str);
    }
        
    [Theory, DefaultAutoData(ConfigureMembers: false)]
    public async Task RunWithCallback_ReturnsProcessReturn(
        int ret,
        [Frozen]ProcessStartInfo startInfo,
        IProcessWrapper process,
        Action<string> callback,
        CancellationToken cancel,
        ProcessRunner sut)
    {
        process.Run().Returns(Task.FromResult(ret));
        sut.Factory.Create(default!).ReturnsForAnyArgs(process);
        (await sut.RunWithCallback(startInfo, callback, cancel))
            .ShouldBe(ret);
    }

    #endregion
}