using System;
using System.Collections;
using System.Collections.Generic;
using Noggog.Containers.Pools;

namespace Noggog.Notifying
{
    public interface INotifyingContainer2DGetter<T> : INotifyingEnumerable<P2IntValue<T>>
    {
        INotifyingItemGetter<int> MinX { get; }
        INotifyingItemGetter<int> MinY { get; }
        INotifyingItemGetter<int> MaxX { get; }
        INotifyingItemGetter<int> MaxY { get; }
        T this[P2Int p] { get; }
        int Width { get; }
        int Height { get; }
    }

    public interface INotifyingContainer2D<T> : INotifyingContainer2DGetter<T>, INotifyingCollection<P2IntValue<T>>
    {
        new T this[P2Int p] { get; set; }
    }

    public delegate void NotifyingContainer2DCallback<T>(IEnumerable<ChangePoint<T>> changes);

    public abstract class NotifyingContainer2D<T> : NotifyingCollection<P2IntValue<T>, ChangePoint<T>>, INotifyingContainer2D<T>
    {
        protected static ObjectListPool<ChangePoint<T>> firePool = new ObjectListPool<ChangePoint<T>>(100);

        public abstract INotifyingItemGetter<int> Count { get; }
        public abstract INotifyingItemGetter<int> MinX { get; }
        public abstract INotifyingItemGetter<int> MinY { get; }
        public abstract INotifyingItemGetter<int> MaxX { get; }
        public abstract INotifyingItemGetter<int> MaxY { get; }
        public abstract int Width { get; }
        public abstract int Height { get; }
        T INotifyingContainer2DGetter<T>.this[P2Int p] => this[p];

        public abstract T this[P2Int p] { get; set; }

        public abstract T Get(P2Int p);

        public abstract void Set(P2IntValue<T> p, NotifyingFireParameters? cmds = null);

        public void Unset(NotifyingUnsetParameters? cmds = null)
        {
            HasBeenSet = false;
            Clear(cmds.ToFireParams());
        }

        public abstract void Clear(NotifyingFireParameters? cmds = null);

        public abstract bool Remove(P2IntValue<T> item, NotifyingFireParameters? cmds = null);

        public abstract void SetTo(IEnumerable<P2IntValue<T>> enumer, NotifyingFireParameters? cmds = null);

        public abstract void Add(P2IntValue<T> item, NotifyingFireParameters? cmds = null);

        public abstract void Add(IEnumerable<P2IntValue<T>> items, NotifyingFireParameters? cmds = null);

        public abstract IEnumerator<P2IntValue<T>> GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        protected void FireChange(IEnumerable<ChangePoint<T>> changes, NotifyingFireParameters? cmds)
        {
            List<Exception> exceptions = null;

            using (var fireSubscribers = this.subscribers.GetSubs())
            {
                foreach (var sub in fireSubscribers)
                {
                    foreach (var eventItem in sub.Value)
                    {
                        try
                        {
                            eventItem(sub.Key, changes);
                        }
                        catch (Exception ex)
                        {
                            if (exceptions == null)
                            {
                                exceptions = new List<Exception>();
                            }
                            exceptions.Add(ex);
                        }
                    }
                }
            }

            if (this.enumerSubscribers.HasSubs)
            {
                using (var enumerChanges = fireEnumerPool.Checkout())
                {
                    foreach (var change in changes)
                    {
                        switch (change.AddRem)
                        {
                            case AddRemoveModify.Add:
                                enumerChanges.Item.Add(new ChangeAddRem<P2IntValue<T>>(new P2IntValue<T>(change.Point, change.New), AddRemove.Add));
                                break;
                            case AddRemoveModify.Remove:
                                enumerChanges.Item.Add(new ChangeAddRem<P2IntValue<T>>(new P2IntValue<T>(change.Point, change.Old), AddRemove.Remove));
                                break;
                            case AddRemoveModify.Modify:
                                enumerChanges.Item.Add(new ChangeAddRem<P2IntValue<T>>(new P2IntValue<T>(change.Point, change.Old), AddRemove.Remove));
                                enumerChanges.Item.Add(new ChangeAddRem<P2IntValue<T>>(new P2IntValue<T>(change.Point, change.New), AddRemove.Add));
                                break;
                            default:
                                break;
                        }
                    }
                    using (var fireSubscribers = this.enumerSubscribers.GetSubs())
                    {
                        foreach (var sub in fireSubscribers)
                        {
                            foreach (var eventItem in sub.Value)
                            {
                                try
                                {
                                    eventItem(sub.Key, enumerChanges.Item);
                                }
                                catch (Exception ex)
                                {
                                    if (exceptions == null)
                                    {
                                        exceptions = new List<Exception>();
                                    }
                                    exceptions.Add(ex);
                                }
                            }
                        }
                    }
                }
            }
            
            if (exceptions != null
                && exceptions.Count > 0)
            {
                Exception ex;
                if (exceptions.Count == 1)
                {
                    ex = exceptions[0];
                }
                else
                {
                    ex = new AggregateException(exceptions.ToArray());
                }

                if (cmds?.ExceptionHandler == null)
                {
                    throw ex;
                }
                else
                {
                    cmds.Value.ExceptionHandler(ex);
                }
            }
        }
    }
}
