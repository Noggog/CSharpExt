using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Runtime.CompilerServices;
using System.Collections;

namespace Noggog.WPF
{
    public class ViewModel : ReactiveObject, IDisposableDropoff
    {
        private readonly Lazy<CompositeDisposable> _compositeDisposable = new();

        public virtual void Dispose()
        {
            if (_compositeDisposable.IsValueCreated)
            {
                _compositeDisposable.Value.Dispose();
            }
        }
        
        protected void RaiseAndSetIfChanged<T>(
            ref T item,
            T newItem,
            [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(item, newItem)) return;
            item = newItem;
            this.RaisePropertyChanged(propertyName);
        }

        protected void RaiseAndSetIfChanged<T>(
            ref T item,
            T newItem,
            ref bool hasBeenSet,
            bool newHasBeenSet,
            string name,
            string hasBeenSetName)
        {
            if (!newHasBeenSet)
            {
                this.RaiseAndSetIfChanged(ref hasBeenSet, newHasBeenSet, propertyName: hasBeenSetName);
            }
            this.RaiseAndSetIfChanged(ref item, newItem, propertyName: name);
            if (newHasBeenSet)
            {
                this.RaiseAndSetIfChanged(ref hasBeenSet, newHasBeenSet, propertyName: hasBeenSetName);
            }
        }

        protected void RaiseAndSetIfChanged<T>(
            ref T item,
            T newItem,
            BitArray hasBeenSet,
            bool newHasBeenSet,
            int index,
            string name,
            string hasBeenSetName)
        {
            var oldHasBeenSet = hasBeenSet[index];
            bool itemEqual = EqualityComparer<T>.Default.Equals(item, newItem);
            if (oldHasBeenSet != newHasBeenSet)
            {
                hasBeenSet[index] = newHasBeenSet;
            }
            if (!itemEqual)
            {
                item = newItem;
                this.RaisePropertyChanged(name);
            }
            if (oldHasBeenSet != newHasBeenSet)
            {
                this.RaisePropertyChanged(hasBeenSetName);
            }
        }

        protected void RaiseAndSetIfChanged(
            BitArray hasBeenSet,
            bool newHasBeenSet,
            int index,
            string name)
        {
            var oldHasBeenSet = hasBeenSet[index];
            if (oldHasBeenSet == newHasBeenSet) return;
            hasBeenSet[index] = newHasBeenSet;
            this.RaisePropertyChanged(name);
        }

        public override string ToString()
        {
            return this.GetType().Name;
        }

        public void Add(IDisposable disposable)
        {
            _compositeDisposable.Value.Add(disposable);
        }
    }
}
