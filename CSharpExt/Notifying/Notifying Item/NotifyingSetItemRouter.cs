using System;

namespace Noggog.Notifying
{
    /*
    *  Class intended to forward another NotifyingItem by default.  
    *  If it is set, then it breaks that subscription and becomes like a normal NotifyingItem until unset.
    */
    public class NotifyingSetItemRouter<T> : INotifyingSetItem<T>
    {
        INotifyingSetItemGetter<T> _base;
        INotifyingSetItem<T> _child;

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
        void IHasBeenSetItem<T>.Set(T value) => Set(value, cmd: null);
        void IHasBeenSetItem<T>.Unset() => Unset(cmds: null);

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

        public void SetCurrentAsDefault()
        {
            SwapOver();
            _child.SetCurrentAsDefault();
        }

        public void Subscribe(Action callback, bool fireInitial = true)
        {
            _child.Subscribe(
                callback: callback,
                fireInitial: fireInitial);
        }

        public void Subscribe(object owner, Action callback, bool fireInitial = true)
        {
            _child.Subscribe(
                owner: owner,
                callback: callback,
                fireInitial: fireInitial);
        }

        public void Subscribe(object owner, NotifyingItemSimpleCallback<T> callback, bool fireInitial = true)
        {
            _child.Subscribe(
                owner: owner,
                callback: callback,
                fireInitial: fireInitial);
        }

        public void Subscribe(NotifyingItemSimpleCallback<T> callback, bool fireInitial = true)
        {
            _child.Subscribe(
                callback: callback,
                fireInitial: fireInitial);
        }

        public void Subscribe<O>(O owner, NotifyingItemCallback<O, T> callback, bool fireInitial = true)
        {
            _child.Subscribe(
                owner: owner, 
                callback: callback,
                fireInitial: fireInitial);
        }

        public void Unset(NotifyingUnsetParameters? cmds = null)
        {
            _child.Unset(cmds);
            SwapBack(cmds);
        }

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

        private void SwapBack(NotifyingUnsetParameters? cmds, bool force = false)
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
                            markAsSet: _base.HasBeenSet, 
                            exceptionHandler: cmds?.ExceptionHandler, 
                            forceFire: cmds?.ForceFire ?? false));
                });
        }

        public void Set(T value, NotifyingFireParameters? cmd = null)
        {
            SwapOver();
            _child.Set(value, cmd);
        }

        public void SetHasBeenSet(bool on)
        {
            SwapOver();
            _child.HasBeenSet = on;
        }
    }
}
