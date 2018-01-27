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
        void Set(T value, NotifyingFireParameters cmds);
    }

    public interface INotifyingSetItemGetter<T> : IHasBeenSetItemGetter<T>, INotifyingItemGetter<T>
    {
        new T Item { get; }
    }

    public interface INotifyingSetItem<T> : INotifyingItem<T>, IHasBeenSetItem<T>, INotifyingSetItemGetter<T>
    {
        new T Item { get; set; }
        void Unset(NotifyingUnsetParameters cmds);
    }
}
