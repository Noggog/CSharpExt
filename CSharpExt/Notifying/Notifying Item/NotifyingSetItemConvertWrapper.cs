using System;

namespace Noggog.Notifying
{
    public struct NotifyingSetItemConvertWrapper<T> : INotifyingSetItem<T>
    {
        private readonly INotifyingSetItem<T> Source;
        private readonly Func<Change<T>, TryGet<T>> incomingConverter;

        public NotifyingSetItemConvertWrapper(
            Func<Change<T>, TryGet<T>> incomingConverter,
            T defaultVal = default(T),
            bool markAsSet = false)
            : this(
                  source: new NotifyingSetItem<T>(
                      defaultVal: defaultVal, 
                      markAsSet: markAsSet),
                  incomingConverter: incomingConverter,
                  setIntially: true)
        {
        }

        public NotifyingSetItemConvertWrapper(
            INotifyingSetItem<T> source,
            Func<Change<T>, TryGet<T>> incomingConverter,
            bool setIntially)
        {
            Source = source;
            this.incomingConverter = incomingConverter;
            if (setIntially)
            {
                var result = this.incomingConverter(
                    new Change<T>(
                        default(T),
                        source.Item));
                if (result.Succeeded)
                {
                    Source.Item = result.Value;
                }
            }
        }

        #region NotifyingItem interface
        public T Item { get => Source.Item; set => this.Set(value); }

        public T DefaultValue => throw new NotImplementedException();

        public bool HasBeenSet => this.Source.HasBeenSet;

        bool IHasBeenSetItem<T>.HasBeenSet
        {
            get => this.Source.HasBeenSet;
            set => this.Source.HasBeenSet = value;
        }

        public void Set(T value, NotifyingFireParameters cmd)
        {
            var setting = this.incomingConverter(
                new Change<T>(
                    Source.Item,
                    value));
            if (setting.Succeeded)
            {
                this.Source.Set(setting.Value, cmd);
            }
        }

        public void Set(T value)
        {
            this.Set(value, cmd: null);
        }

        public void SetCurrentAsDefault()
        {
            this.Source.SetCurrentAsDefault();
        }

        public void Subscribe(Action callback, bool fireInitial = true)
        {
            this.Source.Subscribe(
                callback: callback,
                fireInitial: fireInitial);
        }

        public void Subscribe(object owner, Action callback, bool fireInitial = true)
        {
            this.Source.Subscribe(
                owner: owner,
                callback: callback,
                fireInitial: fireInitial);
        }

        public void Subscribe(object owner, NotifyingItemSimpleCallback<T> callback, bool fireInitial = true)
        {
            this.Source.Subscribe(
                owner: owner,
                callback: callback,
                fireInitial: fireInitial);
        }

        public void Subscribe<O>(O owner, NotifyingItemCallback<O, T> callback, bool fireInitial = true)
        {
            this.Source.Subscribe(
                owner: owner,
                callback: callback,
                fireInitial: fireInitial);
        }

        public void Subscribe(NotifyingItemSimpleCallback<T> callback, bool fireInitial = true)
        {
            this.Source.Subscribe(
                callback: callback, 
                fireInitial: fireInitial);
        }

        public void Unset(NotifyingUnsetParameters cmds)
        {
            this.Source.Unset(cmds);
        }

        public void Unset()
        {
            this.Source.Unset();
        }

        public void Unsubscribe(object owner)
        {
            this.Source.Unsubscribe(owner);
        }
        #endregion
    }

    public class NotifyingSetItemConvertWrapper<T, R> : INotifyingSetItem<R>
    {
        public readonly INotifyingSetItem<T> Source;
        private readonly Func<T, R> incomingConverter;
        private readonly Func<R, T> outgoingConverter;

        public NotifyingSetItemConvertWrapper(
            INotifyingSetItem<T> source,
            Func<T, R> incomingConverter,
            Func<R, T> outgoingConverter)
        {
            Source = source;
            this.incomingConverter = incomingConverter;
            this.outgoingConverter = outgoingConverter;
        }

        #region NotifyingItem interface
        R INotifyingItem<R>.Item { get => this.incomingConverter(this.Source.Item); set => this.Source.Item = this.outgoingConverter(value); }
        R IHasBeenSetItem<R>.Item { get => this.incomingConverter(this.Source.Item); set => this.Source.Item = this.outgoingConverter(value); }
        void IHasBeenSetItem<R>.Set(R value) => Set(value, cmds: null);
        void IHasBeenSetItem<R>.Unset() => Unset(cmds: null);

        public R DefaultValue => this.incomingConverter(this.Source.DefaultValue);

        public bool HasBeenSet { get => this.Source.HasBeenSet; set => this.Source.HasBeenSet = value; }

        public R Item
        {
            get => this.incomingConverter(this.Source.Item);
            set => this.Source.Item = this.outgoingConverter(value);
        }

        public void Unset(NotifyingUnsetParameters cmds)
        {
            this.Source.Unset(cmds);
        }

        public void SetCurrentAsDefault()
        {
            this.Source.SetCurrentAsDefault();
        }

        public void Set(R value, NotifyingFireParameters cmds)
        {
            this.Source.Set(this.outgoingConverter(value), cmds);
        }

        public void Subscribe(Action callback, bool fireInitial = true)
        {
            this.Source.Subscribe(
                callback: callback,
                fireInitial: fireInitial);
        }

        public void Subscribe(object owner, Action callback, bool fireInitial = true)
        {
            this.Source.Subscribe(
                owner: owner,
                callback: callback,
                fireInitial: fireInitial);
        }

        public void Subscribe(object owner, NotifyingItemSimpleCallback<R> callback, bool fireInitial = true)
        {
            this.Source.Subscribe(
                owner: owner,
                callback: (change) =>
                {
                    callback(
                        new Change<R>(
                            this.incomingConverter(change.Old),
                            this.incomingConverter(change.New)));
                },
                fireInitial: fireInitial);
        }

        public void Subscribe(NotifyingItemSimpleCallback<R> callback, bool fireInitial = true)
        {
            this.Source.Subscribe(
                callback: (change) =>
                {
                    callback(
                        new Change<R>(
                            this.incomingConverter(change.Old),
                            this.incomingConverter(change.New)));
                },
                fireInitial: fireInitial);
        }

        public void Subscribe<O>(O owner, NotifyingItemCallback<O, R> callback, bool fireInitial = true)
        {
            this.Source.Subscribe(
                owner: owner,
                callback: (ow, change) =>
                {
                    callback(
                        ow,
                        new Change<R>(
                            this.incomingConverter(change.Old),
                            this.incomingConverter(change.New)));
                },
                fireInitial: fireInitial);
        }

        public void Unsubscribe(object owner)
        {
            this.Source.Unsubscribe(owner);
        }

        public void SetHasBeenSet(bool on)
        {
            this.HasBeenSet = on;
        }
        #endregion
    }
}
