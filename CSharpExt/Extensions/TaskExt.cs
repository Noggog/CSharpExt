﻿using Noggog;
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

        public static Task<bool> Timeout(this Task task, int? timeoutMS, bool throwIfTimeout = false)
        {
            return Timeout(task, timeoutMS, "Task", throwIfTimeout);
        }

        public static async Task<bool> Timeout(this Task task, int? timeoutMS, string taskMessage, bool throwIfTimeout = false)
        {
            if (!timeoutMS.HasValue)
            {
                await task;
                return false;
            }
            var retTask = await Task.WhenAny(task, Task.Delay(timeoutMS.Value));
            if (retTask == task) return false;
            if (throwIfTimeout) throw new TimeoutException($"{taskMessage} took longer than {timeoutMS.Value}ms.");
            return true;
        }

        public static Task<TryGet<T>> TryTimeout<T>(this Task<T> task, int? timeoutMS, bool throwIfTimeout = false)
        {
            return TryTimeout(task, timeoutMS, "Task", throwIfTimeout);
        }

        public static async Task<TryGet<T>> TryTimeout<T>(this Task<T> task, int? timeoutMS, string taskMessage, bool throwIfTimeout = false)
        {
            if (!timeoutMS.HasValue)
            {
                return TryGet<T>.Fail(await task);
            }
            var retTask = await Task.WhenAny(task, Task.Delay(timeoutMS.Value));
            if (retTask == task)
            {
                return TryGet<T>.Fail(await task);
            }
            if (throwIfTimeout) throw new TimeoutException($"{taskMessage} took longer than {timeoutMS.Value}ms.");
            return TryGet<T>.Create(successful: true);
        }

        public static Task<T> Timeout<T>(this Task<T> task, int? timeoutMS)
        {
            return Timeout(task, timeoutMS, "Task");
        }

        public static async Task<T> Timeout<T>(this Task<T> task, int? timeoutMS, string taskMessage)
        {
            if (!timeoutMS.HasValue)
            {
                return await task;
            }
            var retTask = await Task.WhenAny(task, Task.Delay(timeoutMS.Value));
            if (retTask == task)
            {
                return await task;
            }
            throw new TimeoutException($"{taskMessage} took longer than {timeoutMS.Value}ms.");
        }

        public static async Task DoThenComplete(TaskCompletionSource tcs, Func<Task> action)
        {
            try
            {
                await action();
                tcs?.Complete();
            }
            catch (Exception ex)
            {
                tcs?.SetException(ex);
                throw;
            }
        }
    }
}
