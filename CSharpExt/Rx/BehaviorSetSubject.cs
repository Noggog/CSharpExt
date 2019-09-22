using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;

namespace Noggog
{
    public class BehaviorSetSubject<T> :
        ISubject<(T Item, bool HasBeenSet)>,
        ISubject<(T Item, bool HasBeenSet), (T Item, bool HasBeenSet)>,
        IObserver<(T Item, bool HasBeenSet)>,
        IObservable<(T Item, bool HasBeenSet)>,
        IDisposable,
        IHasBeenSetItem<T>
    {
        private readonly BehaviorSubject<(T Item, bool HasBeenSet)> _subject;
        public T Value => _subject.Value.Item;

        T IHasBeenSetItem<T>.Item
        {
            get => this.Value;
            set => this.OnNext(value);
        }

        T IHasItem<T>.Item
        {
            get => this.Value;
            set => this.OnNext(value);
        }

        T IHasItem<T>.DefaultValue => default;

        T IHasItemGetter<T>.Item => this.Value;

        public bool HasBeenSet
        {
            get => this._subject.Value.HasBeenSet;
            set => this.OnNext(this.Value, true);
        }

        bool IHasBeenSetGetter.HasBeenSet => this.HasBeenSet;

        public BehaviorSetSubject(T defaultValue = default)
        {
            this._subject = new BehaviorSubject<(T Item, bool HasBeenSet)>((defaultValue, false));
        }

        public void Dispose()
        {
            _subject.Dispose();
        }

        public void OnCompleted()
        {
            _subject.OnCompleted();
        }

        public void OnError(Exception error)
        {
            _subject.OnError(error);
        }

        public void OnNext(T value)
        {
            _subject.OnNext((value, true));
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            return _subject
                .Select(i => i.Item)
                .Subscribe(observer);
        }

        void IHasBeenSetItem<T>.Set(T item, bool hasBeenSet) => this.OnNext(item, hasBeenSet);

        public void Unset() => this.OnNext(default, hasBeenSet: false);

        public void OnNext((T Item, bool HasBeenSet) value)
        {
            this._subject.OnNext(value);
        }

        public void OnNext(T item, bool hasBeenSet = true)
        {
            this._subject.OnNext((item, hasBeenSet));
        }

        public IDisposable Subscribe(IObserver<(T Item, bool HasBeenSet)> observer)
        {
            return this._subject.Subscribe(observer);
        }
    }
}
