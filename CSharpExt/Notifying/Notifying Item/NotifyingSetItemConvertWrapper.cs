using System;

namespace Noggog.Notifying
{
    public struct NotifyingSetItemConvertWrapper<T> : INotifyingSetItem<T>
    {
        private readonly INotifyingSetItem<T> Source;
        private readonly Func<Change<T>, TryGet<T>> incomingConverter;

        public NotifyingSetItemConvertWrapper(
            Func<Change<T>, TryGet<T>> incomingConverter,
            T defaultVal = default(T))
            : this(
                  source: new NotifyingSetItem<T>(defaultVal: defaultVal),
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

        public void Set(T value, NotifyingFireParameters? cmd)
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

        public void Subscribe<O>(O owner, NotifyingItemCallback<O, T> callback, bool fireInitial)
        {
            this.Source.Subscribe(owner, callback, fireInitial);
        }

        public void Subscribe(NotifyingItemSimpleCallback<T> callback, bool fireInitial)
        {
            this.Source.Subscribe(callback, fireInitial);
        }

        public void Subscribe(NotifyingItemSimpleCallback<T> callback)
        {
            this.Source.Subscribe(callback);
        }

        public void Subscribe<O>(O owner, NotifyingItemCallback<O, T> callback)
        {
            this.Source.Subscribe(owner, callback);
        }

        public void Unset(NotifyingUnsetParameters? cmds)
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

        public void Unset(NotifyingUnsetParameters? cmds)
        {
            this.Source.Unset(cmds);
        }

        public void SetCurrentAsDefault()
        {
            this.Source.SetCurrentAsDefault();
        }

        public void Set(R value, NotifyingFireParameters? cmds)
        {
            this.Source.Set(this.outgoingConverter(value), cmds);
        }

        public void Subscribe<O>(O owner, NotifyingItemCallback<O, R> callback, bool fireInitial)
        {
            this.Source.Subscribe(
                owner,
                (ow, change) =>
                {
                    callback(
                        ow,
                        new Change<R>(
                            this.incomingConverter(change.Old),
                            this.incomingConverter(change.New)));
                },
                fireInitial);
        }

        public void Subscribe(NotifyingItemSimpleCallback<R> callback, bool fireInitial)
        {
            this.Source.Subscribe(
                (change) =>
                {
                    callback(
                        new Change<R>(
                            this.incomingConverter(change.Old),
                            this.incomingConverter(change.New)));
                },
                fireInitial);
        }

        public void Subscribe(NotifyingItemSimpleCallback<R> callback)
        {
            this.Source.Subscribe(
                (change) =>
                {
                    callback(
                        new Change<R>(
                            this.incomingConverter(change.Old),
                            this.incomingConverter(change.New)));
                });
        }

        public void Subscribe<O>(O owner, NotifyingItemCallback<O, R> callback)
        {
            this.Source.Subscribe(
                owner,
                (ow, change) =>
                {
                    callback(
                        ow,
                        new Change<R>(
                            this.incomingConverter(change.Old),
                            this.incomingConverter(change.New)));
                });
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
