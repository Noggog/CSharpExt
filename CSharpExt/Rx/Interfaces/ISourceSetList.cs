using DynamicData;
using Noggog.Notifying;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpExt.Rx
{
    public interface ISourceSetList<T> : ISourceList<T>, IHasBeenSet, IObservableSetList<T>
    {
        new T this[int index] { get; set; }
        void Edit(Action<IExtendedList<T>> updateAction, bool hasBeenSet);
        void AddRange(IEnumerable<T> item);
        new int Count { get; }
    }
}
