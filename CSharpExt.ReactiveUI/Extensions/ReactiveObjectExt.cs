using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveUI
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
    }
}
