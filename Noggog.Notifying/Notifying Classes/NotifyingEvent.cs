using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Noggog.Notifying
{
    public class NotifyingEvent
    {
        private readonly Dictionary<object, FireList<Action>> _event = new Dictionary<object, FireList<Action>>();
        private readonly ReaderWriterLockSlim _rwl = new ReaderWriterLockSlim();

        public void Fire()
        {
            _rwl.EnterReadLock();

            foreach (var f in _event.Values)
                foreach (var a in f)
                    a();

            _rwl.ExitReadLock();

        }

        public void Register(object o, Action a)
        {
            _rwl.EnterWriteLock();

            FireList<Action> list;
            if (!_event.TryGetValue(o, out list))
            {
                _event.Add(o, new FireList<Action>());
            }
            _event[o].Add(a);

            _rwl.ExitWriteLock();
        }

        public void UnRegister(object o)
        {
            _rwl.EnterWriteLock();
            _event.Remove(o);
            _rwl.ExitWriteLock();
        }
    }

    public class NotifyingEvent<T>
    {
        private readonly Dictionary<object, FireList<Action<T>>> _event = new Dictionary<object, FireList<Action<T>>>();
        private readonly ReaderWriterLockSlim _rwl = new ReaderWriterLockSlim();

        public void Fire(T t)
        {
            _rwl.EnterReadLock();

            foreach (var f in _event.Values)
                foreach (var a in f)
                    a(t);

            _rwl.ExitReadLock();

        }

        public void Register(object o, Action<T> a)
        {
            _rwl.EnterWriteLock();

            FireList<Action<T>> list;
            if (!_event.TryGetValue(o, out list))
            {
                _event.Add(o, new FireList<Action<T>>());
            }
            _event[o].Add(a);

            _rwl.ExitWriteLock();
        }

        public void UnRegister(object o)
        {
            _rwl.EnterWriteLock();
            _event.Remove(o);
            _rwl.ExitWriteLock();
        }
    }

    public class NotifyingEvent<T, K>
    {
        private readonly Dictionary<object, FireList<Action<T, K>>> _event = new Dictionary<object, FireList<Action<T, K>>>();
        private readonly ReaderWriterLockSlim _rwl = new ReaderWriterLockSlim();

        public void Fire(T t, K k)
        {
            _rwl.EnterReadLock();

            foreach (var f in _event.Values)
                foreach (var a in f)
                    a(t, k);

            _rwl.ExitReadLock();

        }

        public void Register(object o, Action<T, K> a)
        {
            _rwl.EnterWriteLock();

            FireList<Action<T, K>> list;
            if (!_event.TryGetValue(o, out list))
            {
                _event.Add(o, new FireList<Action<T, K>>());
            }
            _event[o].Add(a);

            _rwl.ExitWriteLock();
        }

        public void UnRegister(object o)
        {
            _rwl.EnterWriteLock();
            _event.Remove(o);
            _rwl.ExitWriteLock();
        }
    }

    public class NotifyingTaskEvents
    {
        public async Task Pre()
        {
            
        }

        public async Task Post()
        {

        }
    }
}
