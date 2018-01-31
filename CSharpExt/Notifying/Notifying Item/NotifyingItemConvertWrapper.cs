using System;

namespace Noggog.Notifying
{
    public struct NotifyingItemConvertWrapper<T> : INotifyingItem<T>
    {
        private readonly INotifyingItem<T> Source;
        private readonly Func<Change<T>, TryGet<T>> incomingConverter;

        public NotifyingItemConvertWrapper(
            Func<Change<T>, TryGet<T>> incomingConverter,
            T defaultVal = default(T))
            : this(new NotifyingItem<T>(defaultVal: defaultVal), incomingConverter, true)
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
                        source.Item));
                if (result.Succeeded)
                {
                    Source.Item = result.Value;
                }
            }
        }

        #region NotifyingItem interface
        public T Item { get => Source.Item; set => this.Set(value); }

        public void Bind(object owner, INotifyingItem<T> rhs, NotifyingBindParameters cmds = null)
        {
            throw new NotImplementedException();
        }

        public void Bind<R>(object owner, INotifyingItem<R> rhs, Func<T, R> toConv, Func<R, T> fromConv, NotifyingBindParameters cmds = null)
        {
            throw new NotImplementedException();
        }

        public void Bind(INotifyingItem<T> rhs, NotifyingBindParameters cmds = null)
        {
            throw new NotImplementedException();
        }

        public void Bind<R>(INotifyingItem<R> rhs, Func<T, R> toConv, Func<R, T> fromConv, NotifyingBindParameters cmds = null)
        {
            throw new NotImplementedException();
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

        public void Subscribe(Action callback, NotifyingSubscribeParameters cmds = null)
        {
            this.Source.Subscribe(callback: callback, cmds: cmds);
        }

        public void Subscribe(object owner, Action callback, NotifyingSubscribeParameters cmds = null)
        {
            this.Source.Subscribe(owner: owner, callback: callback, cmds: cmds);
        }

        public void Subscribe(NotifyingItemSimpleCallback<T> callback, NotifyingSubscribeParameters cmds = null)
        {
            this.Source.Subscribe(callback: callback, cmds: cmds);
        }

        public void Subscribe(object owner, NotifyingItemSimpleCallback<T> callback, NotifyingSubscribeParameters cmds = null)
        {
            this.Source.Subscribe(owner: owner, callback: callback, cmds: cmds);
        }

        public void Subscribe<O>(O owner, NotifyingItemCallback<O, T> callback, NotifyingSubscribeParameters cmds = null)
        {
            this.Source.Subscribe(owner: owner, callback: callback, cmds: cmds);
        }

        public void Unsubscribe(object owner)
        {
            this.Source.Unsubscribe(owner);
        }
        #endregion
    }

    public class NotifyingItemConvertWrapper<T, R> : INotifyingSetItem<R>
    {
        public readonly INotifyingSetItem<T> Source;
        private readonly Func<T, R> incomingConverter;
        private readonly Func<R, T> outgoingConverter;

        public NotifyingItemConvertWrapper(
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

        public void Set(R item, bool hasBeenSet, NotifyingFireParameters cmds = null)
        {
            this.Source.Set(this.outgoingConverter(item), hasBeenSet, cmds);
        }

        public void Subscribe(object owner, Action callback, NotifyingSubscribeParameters cmds = null)
        {
            this.Source.Subscribe(
                owner: owner,
                callback: callback,
                cmds: cmds);
        }

        public void Subscribe(Action callback, NotifyingSubscribeParameters cmds = null)
        {
            this.Source.Subscribe(
                callback: callback,
                cmds: cmds);
        }

        public void Subscribe(object owner, NotifyingItemSimpleCallback<R> callback, NotifyingSubscribeParameters cmds = null)
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
                cmds: cmds);
        }

        public void Subscribe(NotifyingItemSimpleCallback<R> callback, NotifyingSubscribeParameters cmds = null)
        {
            this.Source.Subscribe(
                callback: (change) =>
                {
                    callback(
                        new Change<R>(
                            this.incomingConverter(change.Old),
                            this.incomingConverter(change.New)));
                },
                cmds: cmds);
        }

        public void Subscribe<O>(O owner, NotifyingItemCallback<O, R> callback, NotifyingSubscribeParameters cmds = null)
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
                cmds: cmds);
        }

        public void Subscribe(NotifyingSetItemSimpleCallback<R> callback, NotifyingSubscribeParameters cmds = null)
        {
            this.Source.Subscribe(
                callback: (change) =>
                {
                    callback(
                        new ChangeSet<R>(
                            oldVal: this.incomingConverter(change.Old),
                            oldSet: change.OldSet,
                            newVal: this.incomingConverter(change.New),
                            newSet: change.NewSet));
                },
                cmds: cmds);
        }

        public void Subscribe(object owner, NotifyingSetItemSimpleCallback<R> callback, NotifyingSubscribeParameters cmds = null)
        {
            this.Source.Subscribe(
                owner: owner,
                callback: (change) =>
                {
                    callback(
                        new ChangeSet<R>(
                            oldVal: this.incomingConverter(change.Old),
                            oldSet: change.OldSet,
                            newVal: this.incomingConverter(change.New),
                            newSet: change.NewSet));
                },
                cmds: cmds);
        }

        public void Subscribe<O>(O owner, NotifyingSetItemCallback<O, R> callback, NotifyingSubscribeParameters cmds = null)
        {
            this.Source.Subscribe(
                owner: owner,
                callback: (ow, change) =>
                {
                    callback(
                        ow,
                        new ChangeSet<R>(
                            oldVal: this.incomingConverter(change.Old),
                            oldSet: change.OldSet,
                            newVal: this.incomingConverter(change.New),
                            newSet: change.NewSet));
                },
                cmds: cmds);
        }

        public void Unsubscribe(object owner)
        {
            this.Source.Unsubscribe(owner);
        }

        public void SetHasBeenSet(bool on)
        {
            this.HasBeenSet = on;
        }

        public void Bind(object owner, INotifyingSetItem<R> rhs, NotifyingBindParameters cmds = null)
        {
            throw new NotImplementedException();
        }

        public void Bind<R1>(object owner, INotifyingSetItem<R1> rhs, Func<R, R1> toConv, Func<R1, R> fromConv, NotifyingBindParameters cmds = null)
        {
            throw new NotImplementedException();
        }

        public void Bind(INotifyingSetItem<R> rhs, NotifyingBindParameters cmds = null)
        {
            throw new NotImplementedException();
        }

        public void Bind<R1>(INotifyingSetItem<R1> rhs, Func<R, R1> toConv, Func<R1, R> fromConv, NotifyingBindParameters cmds = null)
        {
            throw new NotImplementedException();
        }

        public void Bind(object owner, INotifyingItem<R> rhs, NotifyingBindParameters cmds = null)
        {
            throw new NotImplementedException();
        }

        public void Bind<R1>(object owner, INotifyingItem<R1> rhs, Func<R, R1> toConv, Func<R1, R> fromConv, NotifyingBindParameters cmds = null)
        {
            throw new NotImplementedException();
        }

        public void Bind(INotifyingItem<R> rhs, NotifyingBindParameters cmds = null)
        {
            throw new NotImplementedException();
        }

        public void Bind<R1>(INotifyingItem<R1> rhs, Func<R, R1> toConv, Func<R1, R> fromConv, NotifyingBindParameters cmds = null)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
