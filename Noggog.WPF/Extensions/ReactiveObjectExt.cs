using ReactiveUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Noggog.WPF
{
    public static class ReactiveObjectExt
    {
        public static void RaiseAndSetIfChanged<T>(
            this ReactiveObject reactiveObj,
            ref T item,
            T newItem,
            ref bool hasBeenSet,
            bool newHasBeenSet,
            string name,
            string hasBeenSetName)
        {
            if (!newHasBeenSet)
            {
                reactiveObj.RaiseAndSetIfChanged(ref hasBeenSet, newHasBeenSet, propertyName: hasBeenSetName);
            }
            reactiveObj.RaiseAndSetIfChanged(ref item, newItem, propertyName: name);
            if (newHasBeenSet)
            {
                reactiveObj.RaiseAndSetIfChanged(ref hasBeenSet, newHasBeenSet, propertyName: hasBeenSetName);
            }
        }

        public static void RaiseAndSetIfChanged<T>(
            this ReactiveObject reactiveObj,
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
                reactiveObj.RaisePropertyChanging(hasBeenSetName);
                hasBeenSet[index] = newHasBeenSet;
            }
            if (!itemEqual)
            {
                reactiveObj.RaisePropertyChanging(name);
                item = newItem;
                reactiveObj.RaisePropertyChanged(name);
            }
            if (oldHasBeenSet != newHasBeenSet)
            {
                reactiveObj.RaisePropertyChanged(hasBeenSetName);
            }
        }

        public static void RaiseAndSetIfChanged(
            this ReactiveObject reactiveObj,
            BitArray hasBeenSet,
            bool newHasBeenSet,
            int index,
            string name)
        {
            var oldHasBeenSet = hasBeenSet[index];
            if (oldHasBeenSet == newHasBeenSet) return;
            reactiveObj.RaisePropertyChanging(name);
            hasBeenSet[index] = newHasBeenSet;
            reactiveObj.RaisePropertyChanged(name);
        }

        public static IDisposable InvokeCommand<T>(this IObservable<T> item, IReactiveCommand command)
        {
            return ReactiveUI.ReactiveCommandMixins.InvokeCommand(item, (ICommand)command);
        }
    }
}
