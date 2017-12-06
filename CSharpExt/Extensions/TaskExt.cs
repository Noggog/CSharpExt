using Noggog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public static class TaskExt
    {
        public static async Task<T> AwaitOrDefault<T>(Task<T> t)
        {
            if (t == null) return default(T);
            return await t;
        }

        public static async Task<T> AwaitOrDefaultValue<T>(Task<TryGet<T>> t)
        {
            if (t == null) return default(T);
            return (await t).Value;
        }

        public static async Task<bool> Timeout(this Task task, int? timeoutMS, bool throwIfTimeout = false)
        {
            if (!timeoutMS.HasValue) return false;
            var retTask = await Task.WhenAny(task, Task.Delay(timeoutMS.Value));
            if (retTask == task) return false;
            if (throwIfTimeout) throw new TimeoutException($"Task took longer than {timeoutMS.Value}ms.");
            return true;
        }

        public static async Task DoThenComplete(TaskCompletionSource<bool> tcs, Func<Task> action)
        {
            try
            {
                await action();
                tcs?.SetResult(true);
            }
            catch (Exception ex)
            {
                tcs?.SetException(ex);
                throw;
            }
        }
    }
}
