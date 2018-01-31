using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noggog.Notifying
{
    public delegate void NotifyingItemCallback<O, T>(O owner, Change<T> change);
    public delegate void NotifyingItemSimpleCallback<T>(Change<T> change);
    public delegate void NotifyingItemInternalCallback<T>(object owner, Change<T> change);
    public delegate void NotifyingSetItemCallback<O, T>(O owner, ChangeSet<T> change);
    public delegate void NotifyingSetItemSimpleCallback<T>(ChangeSet<T> change);
    public delegate void NotifyingSetItemInternalCallback<T>(object owner, ChangeSet<T> change);

    public interface INotifyingItemGetter<T> : IHasItemGetter<T>
    {
        new T Item { get; }
        void Subscribe(Action callback, NotifyingSubscribeParameters cmds = null);
        void Subscribe(object owner,Action callback, NotifyingSubscribeParameters cmds = null);
        void Subscribe(NotifyingItemSimpleCallback<T> callback, NotifyingSubscribeParameters cmds = null);
        void Subscribe(object owner, NotifyingItemSimpleCallback<T> callback, NotifyingSubscribeParameters cmds = null);
        void Subscribe<O>(O owner, NotifyingItemCallback<O, T> callback, NotifyingSubscribeParameters cmds = null);
        void Unsubscribe(object owner);
    }

    public interface INotifyingItem<T> : INotifyingItemGetter<T>, IHasItem<T>
    {
        new T Item { get; set; }
        void Set(T value, NotifyingFireParameters cmds = null);
        void Bind(object owner, INotifyingItem<T> rhs, NotifyingBindParameters cmds = null);
        void Bind<R>(object owner, INotifyingItem<R> rhs, Func<T, R> toConv, Func<R, T> fromConv, NotifyingBindParameters cmds = null);
        void Bind(INotifyingItem<T> rhs, NotifyingBindParameters cmds = null);
        void Bind<R>(INotifyingItem<R> rhs, Func<T, R> toConv, Func<R, T> fromConv, NotifyingBindParameters cmds = null);
    }

    public interface INotifyingSetItemGetter<T> : IHasBeenSetItemGetter<T>, INotifyingItemGetter<T>
    {
        new T Item { get; }
        void Subscribe(NotifyingSetItemSimpleCallback<T> callback, NotifyingSubscribeParameters cmds = null);   
        void Subscribe(object owner, NotifyingSetItemSimpleCallback<T> callback, NotifyingSubscribeParameters cmds = null);
        void Subscribe<O>(O owner, NotifyingSetItemCallback<O, T> callback, NotifyingSubscribeParameters cmds = null);
    }

    public interface INotifyingSetItem<T> : INotifyingItem<T>, IHasBeenSetItem<T>, INotifyingSetItemGetter<T>
    {
        new T Item { get; set; }
        void Unset(NotifyingUnsetParameters cmds);
        void Set(T item, bool hasBeenSet, NotifyingFireParameters cmds = null);
        void Bind(object owner, INotifyingSetItem<T> rhs, NotifyingBindParameters cmds = null);
        void Bind<R>(object owner, INotifyingSetItem<R> rhs, Func<T, R> toConv, Func<R, T> fromConv, NotifyingBindParameters cmds = null);
        void Bind(INotifyingSetItem<T> rhs, NotifyingBindParameters cmds = null);
        void Bind<R>(INotifyingSetItem<R> rhs, Func<T, R> toConv, Func<R, T> fromConv, NotifyingBindParameters cmds = null);
    }
}
