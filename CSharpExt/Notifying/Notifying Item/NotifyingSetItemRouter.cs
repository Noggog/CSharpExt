using System;

namespace Noggog.Notifying
{
    /*
    *  Class intended to forward another NotifyingItem by default.  
    *  If it is set, then it breaks that subscription and becomes like a normal NotifyingItem until unset.
    */
    public class NotifyingSetItemRouter<T> : INotifyingSetItem<T>
    {
        private readonly INotifyingSetItemGetter<T> _base;
        private readonly INotifyingSetItem<T> _child;

        public bool HasBeenSwapped { get; private set; }

        public NotifyingSetItemRouter(
            INotifyingSetItemGetter<T> _base,
            INotifyingSetItem<T> _child)
        {
            this._base = _base;
            this._child = _child;
            if (!this._child.HasBeenSet)
            {
                SwapBack(null, force: true);
            }
        }

        public T Item
        {
            get
            {
                return _child.Item;
            }

            set
            {
                SwapOver();
                _child.Item = value;
            }
        }

        public T DefaultValue => _child.DefaultValue;

        T IHasItemGetter<T>.Item => this.Item;
        void IHasBeenSetItem<T>.Set(T value, bool hasBeenSet) => Set(value, hasBeenSet, cmds: null);

        public bool HasBeenSet
        {
            get
            {
                return (HasBeenSwapped ? _child.HasBeenSet : _base.HasBeenSet);
            }

            set
            {
                SwapOver();
                _child.HasBeenSet = value;
            }
        }

        public void Subscribe(Action callback, NotifyingSubscribeParameters cmds = null)
        {
            _child.Subscribe(
                callback: callback,
                cmds: cmds);
        }

        public void Subscribe(object owner, Action callback, NotifyingSubscribeParameters cmds = null)
        {
            _child.Subscribe(
                owner: owner,
                callback: callback,
                cmds: cmds);
        }

        public void Subscribe(object owner, NotifyingItemSimpleCallback<T> callback, NotifyingSubscribeParameters cmds = null)
        {
            _child.Subscribe(
                owner: owner,
                callback: callback,
                cmds: cmds);
        }

        public void Subscribe(NotifyingItemSimpleCallback<T> callback, NotifyingSubscribeParameters cmds = null)
        {
            _child.Subscribe(
                callback: callback,
                cmds: cmds);
        }

        public void Subscribe<O>(O owner, NotifyingItemCallback<O, T> callback, NotifyingSubscribeParameters cmds = null)
        {
            _child.Subscribe(
                owner: owner, 
                callback: callback,
                cmds: cmds);
        }

        public void Subscribe(NotifyingSetItemSimpleCallback<T> callback, NotifyingSubscribeParameters cmds = null)
        {
            _child.Subscribe(
                callback: callback,
                cmds: cmds);
        }

        public void Subscribe(object owner, NotifyingSetItemSimpleCallback<T> callback, NotifyingSubscribeParameters cmds = null)
        {
            _child.Subscribe(
                owner: owner,
                callback: callback,
                cmds: cmds);
        }

        public void Subscribe<O>(O owner, NotifyingSetItemCallback<O, T> callback, NotifyingSubscribeParameters cmds = null)
        {
            _child.Subscribe(
                owner: owner,
                callback: callback,
                cmds: cmds);
        }

        public void Unset(NotifyingUnsetParameters cmds)
        {
            _child.Unset(cmds);
            SwapBack(cmds);
        }

        public void Unset() => Unset(cmds: null);

        public void Unsubscribe(object owner)
        {
            _child.Unsubscribe(owner);
        }

        private void SwapOver()
        {
            if (HasBeenSwapped) return;
            _base.Unsubscribe(this);
            this.HasBeenSwapped = true;
        }

        private void SwapBack(NotifyingUnsetParameters cmds, bool force = false)
        {
            if (!HasBeenSwapped && ! force) return;
            this.HasBeenSwapped = false;
            _base.Subscribe(
                this,
                (owner, change) =>
                {
                    owner._child.Set(
                        change.New, 
                        new NotifyingFireParameters(
                            exceptionHandler: cmds?.ExceptionHandler, 
                            forceFire: cmds?.ForceFire ?? false));
                });
        }

        public void Set(T item, NotifyingFireParameters cmds = null)
        {
            SwapOver();
            _child.Set(item, cmds);
        }

        public void Set(T item, bool hasBeenSet, NotifyingFireParameters cmds = null)
        {
            SwapOver();
            _child.Set(item, hasBeenSet, cmds);
        }

        public void SetHasBeenSet(bool on)
        {
            SwapOver();
            _child.HasBeenSet = on;
        }
    }
}
