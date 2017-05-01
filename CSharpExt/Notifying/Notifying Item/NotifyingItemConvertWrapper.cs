using System;

namespace Noggog.Notifying
{
    public struct NotifyingItemConvertWrapper<T> : INotifyingItem<T>
    {
        private readonly INotifyingItem<T> Source;
        private readonly Func<Change<T>, TryGet<T>> incomingConverter;

        public NotifyingItemConvertWrapper(
            Func<Change<T>, TryGet<T>> incomingConverter,
            T defaultVal = default(T),
            bool markAsSet = false)
            : this(new NotifyingItem<T>(markAsSet: markAsSet, defaultVal: defaultVal), incomingConverter, true)
        {
        }

        public NotifyingItemConvertWrapper(
            INotifyingItem<T> source,
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
                        source.DefaultValue));
                if (result.Succeeded)
                {
                    Source.Value = result.Value;
                }
            }
        }

        #region NotifyingItem interface
        public T Value { get => Source.Value; set => this.Set(value); }

        public T DefaultValue => Source.DefaultValue;

        public bool HasBeenSet { get => this.Source.HasBeenSet; set => this.Source.HasBeenSet = value; }

        T IHasBeenSetItemGetter<T>.Value => this.Source.Value;
        void IHasBeenSetItem<T>.Set(T value) => Set(value, cmd: null);
        void IHasBeenSetItem<T>.Unset() => Unset(cmds: null);

        public void Unset(NotifyingUnsetParameters? cmds = null)
        {
            HasBeenSet = false;
            Set(this.Source.DefaultValue,
                cmds.ToFireParams());
        }

        public void SetCurrentAsDefault()
        {
            this.Source.SetCurrentAsDefault();
        }

        public void Set(T value, NotifyingFireParameters? cmd)
        {
            var setting = this.incomingConverter(
                new Change<T>(
                    Source.Value,
                    value));
            if (setting.Succeeded)
            {
                this.Source.Set(setting.Value, cmd);
            }
        }

        public void Subscribe<O>(O owner, NotifyingItemCallback<O, T> callback, bool fireInitial)
        {
            this.Source.Subscribe(owner, callback, fireInitial);
        }

        public void Unsubscribe(object owner)
        {
            this.Source.Unsubscribe(owner);
        }
        #endregion
    }

    public class NotifyingItemConvertWrapper<T, R> : INotifyingItem<R>
    {
        public readonly INotifyingItem<T> Source;
        private readonly Func<T, R> incomingConverter;
        private readonly Func<R, T> outgoingConverter;

        public NotifyingItemConvertWrapper(
            INotifyingItem<T> source,
            Func<T, R> incomingConverter,
            Func<R, T> outgoingConverter)
        {
            Source = source;
            this.incomingConverter = incomingConverter;
            this.outgoingConverter = outgoingConverter;
        }

        #region NotifyingItem interface
        R INotifyingItem<R>.Value { get => this.incomingConverter(this.Source.Value); set => this.Source.Value = this.outgoingConverter(value); }
        R IHasBeenSetItem<R>.Value { get => this.incomingConverter(this.Source.Value); set => this.Source.Value = this.outgoingConverter(value); }
        void IHasBeenSetItem<R>.Set(R value) => Set(value, cmds: null);
        void IHasBeenSetItem<R>.Unset() => Unset(cmds: null);

        public R DefaultValue => this.incomingConverter(this.Source.DefaultValue);

        public bool HasBeenSet { get => this.Source.HasBeenSet; set => this.Source.HasBeenSet = value; }

        public R Value => this.incomingConverter(this.Source.Value);

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
