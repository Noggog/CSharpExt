using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Noggog.Utility
{
    public class ProcessWrapper : IDisposable
    {
        public IObservable<string> Output { get; private set; } = null!;
        public IObservable<string> Error { get; private set; } = null!;
        private Task<int> _complete = null!;
        public Task<int> Complete => WatchComplete();
        private Process _process = null!;
        public ProcessStartInfo StartInfo => _process.StartInfo;
        private IDisposable? _dispose = null!;
        private bool _hookingOutput;

        private ProcessWrapper()
        {
        }

        public static ProcessWrapper Start(
            ProcessStartInfo startInfo,
            CancellationToken? cancel = null,
            bool hideWindow = true,
            bool hookOntoOutput = true,
            bool childProcess = true)
        {
            var process = new Process();
            if (hideWindow)
            {
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                startInfo.CreateNoWindow = true;
            }
            if (hookOntoOutput)
            {
                startInfo.RedirectStandardError = true;
                startInfo.RedirectStandardOutput = true;
                startInfo.UseShellExecute = false;
            }
            process.StartInfo = startInfo;
            process.EnableRaisingEvents = true;
            CancellationTokenRegistration? cancelSub;
            cancelSub = cancel?.Register(() =>
            {
                try
                {
                    process.Kill();
                }
                catch (InvalidOperationException)
                {
                }
            });

            Subject<string>? _output = null;
            Subject<string>? _error = null;
            TaskCompletionSource<int> completeTask = new TaskCompletionSource<int>();
            var wrapper = new ProcessWrapper()
            {
                _process = process,
                _complete = completeTask.Task,
                _hookingOutput = hookOntoOutput,
            };

            if (hookOntoOutput)
            {
                _output = new Subject<string>();
                // Latch on and read output
                process.OutputDataReceived += (_, data) =>
                {
                    if (data.Data == null)
                    {
                        _output.OnCompleted();
                    }
                    else
                    {
                        _output.OnNext(data.Data);
                    }
                };
                _error = new Subject<string>();
                process.ErrorDataReceived += (_, data) =>
                {
                    if (data.Data == null)
                    {
                        _error.OnCompleted();
                    }
                    else
                    {
                        _error.OnNext(data.Data);
                    }
                };
                wrapper.Output = _output;
                wrapper.Error = _error;
            }
            else
            {
                wrapper.Output = Observable.Empty<string>();
                wrapper.Error = Observable.Empty<string>();
            }

            cancel ??= CancellationToken.None;
            process.Exited += (s, e) =>
            {
                completeTask.SetResult(process.ExitCode);
            };

            wrapper._dispose = cancelSub;
            return wrapper;
        }

        public void Dispose()
        {
            _dispose?.Dispose();
        }

        private async Task<int> WatchComplete()
        {
            var ret = await _complete;
            if (_hookingOutput)
            {
                await Output.LastOrDefaultAsync();
                await Error.LastOrDefaultAsync();
            }
            return ret;
        }

        public async Task<int> Start()
        {
            _process.Start();
            if (_hookingOutput)
            {
                _process.BeginErrorReadLine();
                _process.BeginOutputReadLine();
            }
            return await WatchComplete();
        }
    }
}
