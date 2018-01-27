using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Noggog.Notifying
{
    public class NotifyingListRouter<T> : INotifyingList<T>
    {
        INotifyingListGetter<T> _base;
        INotifyingList<T> _child;

        public bool HasBeenSwapped { get; private set; }

        public NotifyingListRouter(
            INotifyingListGetter<T> _base,
            INotifyingList<T> _child)
        {
            this._base = _base;
            this._child = _child;
        }
        
        private void SwapOver()
        {
            if (HasBeenSwapped) return;
            _base.Unsubscribe(this);
        }

        private void SwapBack()
        {
            if (!HasBeenSwapped) return;
            _base.Subscribe(
                this,
                (changes) =>
                {
                    this._child.SetTo(_base);
                });
        }

        public T this[int index]
        {
            get
            {
                return _child[index];
            }

            set
            {
                SwapOver();
                _child[index] = value;
            }
        }
         
        T INotifyingListGetter<T>.this[int index] => this[index];

        public INotifyingItemGetter<int> CountProperty => _child.CountProperty;

        public int Count => _child.Count;

        public bool HasBeenSet
        {
            get
            {
                return (HasBeenSwapped ? _child.HasBeenSet : _base.HasBeenSet);
            }
            set
            {
                SwapOver();
                _child.HasBeenSet = value;
            }
        }

        IEnumerable<T> IHasItemGetter<IEnumerable<T>>.Item => _child.Item;

        public void Set(int index, T item, NotifyingFireParameters cmds)
        {
            SwapOver();
            _child.Set(index, item, cmds);
        }

        public void Add(T item, NotifyingFireParameters cmds)
        {
            SwapOver();
            _child.Add(item, cmds);
        }

        public void Add(IEnumerable<T> items, NotifyingFireParameters cmds)
        {
            SwapOver();
            _child.Add(items, cmds);
        }

        public void Insert(int index, T item, NotifyingFireParameters cmds)
        {
            SwapOver();
            _child.Insert(index, item, cmds);
        }
        
        public void Clear(NotifyingFireParameters cmds)
        {
            SwapOver();
            _child.Clear(cmds);
        }

        public bool Contains(T item)
        {
            return _child.Contains(item);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _child.GetEnumerator();
        }

        public int IndexOf(T item)
        {
            return ((INotifyingListGetter<T>)_child).IndexOf(item);
        }

        public IEnumerable<T> Iterate()
        {
            return _child;
        }

        public bool Remove(T item, NotifyingFireParameters cmds = null)
        {
            SwapOver();
            return _child.Remove(item, cmds);
        }

        public void RemoveAt(int index, NotifyingFireParameters cmds)
        {
            SwapOver();
            _child.RemoveAt(index, cmds);
        }

        public void Subscribe<O>(O owner, NotifyingCollection<T, ChangeIndex<T>>.NotifyingCollectionCallback<O> callback, NotifyingSubscribeParameters cmds = null)
        {
            _child.Subscribe(owner, callback, cmds: cmds);
        }

        public void Subscribe(NotifyingCollection<T, ChangeIndex<T>>.NotifyingCollectionSimpleCallback callback, NotifyingSubscribeParameters cmds = null)
        {
            _child.Subscribe(callback, cmds: cmds);
        }

        public void Subscribe_Enumerable<O>(O owner, NotifyingEnumerableCallback<O, T> callback, NotifyingSubscribeParameters cmds = null)
        {
            _child.Subscribe_Enumerable(owner, callback, cmds: cmds);
        }

        public void Unset(NotifyingUnsetParameters cmds = null)
        {
            SwapBack();
            _child.Unset(cmds);
        }

        public void Unsubscribe(object owner)
        {
            _child.Unsubscribe(owner);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _child.GetEnumerator();
        }
        
        public void SetTo(IEnumerable<T> enumer, NotifyingFireParameters cmds)
        {
            SwapOver();
            _child.SetTo(enumer, cmds);
        }
    }
}
