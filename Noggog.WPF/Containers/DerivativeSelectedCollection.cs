using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using ReactiveUI.Fody.Helpers;

namespace Noggog.WPF.Containers
{
    public interface IDerivativeSelectedCollection<T> : IReadOnlyList<SelectedVm<T>>, INotifyCollectionChanged
    {
        IList<T>? OriginalList { get; set; }
        ReadOnlyObservableCollection<SelectedVm<T>> DerivativeList { get; }
    }

    internal class DerivativeSelectedCollection<T> : ViewModel, IDerivativeSelectedCollection<T>
    {
        [Reactive] public IList<T>? OriginalList { get; set; }
        public ReadOnlyObservableCollection<SelectedVm<T>> DerivativeList { get; internal set; } = null!;
        
        public IEnumerator<SelectedVm<T>> GetEnumerator()
        {
            return DerivativeList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)DerivativeList).GetEnumerator();
        }

        public bool Contains(SelectedVm<T> item)
        {
            return DerivativeList.Contains(item);
        }

        public void CopyTo(SelectedVm<T>[] array, int arrayIndex)
        {
            DerivativeList.CopyTo(array, arrayIndex);
        }

        public int Count => DerivativeList.Count;

        public int IndexOf(SelectedVm<T> item)
        {
            return DerivativeList.IndexOf(item);
        }

        public SelectedVm<T> this[int index]
        {
            get => DerivativeList[index];
        }

        public event NotifyCollectionChangedEventHandler? CollectionChanged
        {
            add => ((INotifyCollectionChanged)DerivativeList).CollectionChanged += value;
            remove => ((INotifyCollectionChanged)DerivativeList).CollectionChanged -= value;
        }
    }
}