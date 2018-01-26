using System;
using System.Collections.Generic;
using Noggog.Containers.Pools;

namespace Noggog.Notifying
{
    public class NotifyingArray2D<T> : NotifyingContainer2D<T>
    {
        private readonly T[,] arr;
        private static INotifyingItemGetter<int> zeroItem = new NotifyingSetItemWrapper<int>(0);
        private INotifyingItemGetter<int> _maxX;
        public override INotifyingItemGetter<int> MaxX => _maxX;
        private INotifyingItemGetter<int> _maxY;
        public override INotifyingItemGetter<int> MaxY => _maxY;
        public override INotifyingItemGetter<int> MinX => zeroItem;
        public override INotifyingItemGetter<int> MinY => zeroItem;
        private INotifyingItemGetter<int> _count;
        public override INotifyingItemGetter<int> CountProperty => _count;
        public override int Width => arr.GetLength(1);
        public override int Height => arr.GetLength(0);

        public override T this[P2Int p]
        {
            get
            {
                return arr[p.Y, p.X];
            }
            set
            {
                arr[p.Y, p.X] = value;
            }
        }

        public NotifyingArray2D(T[,] arr)
        {
            this.arr = arr;
            this._maxY = new NotifyingSetItemWrapper<int>(arr.GetLength(0) - 1);
            this._maxX = new NotifyingSetItemWrapper<int>(arr.GetLength(1) - 1);
            this._count = new NotifyingSetItemWrapper<int>(arr.GetLength(0) * arr.GetLength(1));
        }

        public NotifyingArray2D(P2Int dim)
            : this(new T[dim.X, dim.Y])
        {
        }

        public override void Add(IEnumerable<P2IntValue<T>> items, NotifyingFireParameters cmds = default(NotifyingFireParameters))
        {
            SetTo(items, cmds);
        }

        public override void Add(P2IntValue<T> item, NotifyingFireParameters cmds = default(NotifyingFireParameters))
        {
            Set(item, cmds);
        }

        public override void Clear(NotifyingFireParameters cmds = default(NotifyingFireParameters))
        {
            cmds = cmds ?? NotifyingFireParameters.Typical;

            if (cmds.MarkAsSet)
            {
                HasBeenSet = true;
            }

            if (HasSubscribers())
            { // Will be firing
                using (var changes = firePool.Checkout())
                {
                    for (int y = 0; y < arr.GetLength(0); y++)
                    {
                        for (int x = 0; x < arr.GetLength(1); x++)
                        {
                            var cur = arr[y, x];
                            arr[y, x] = default(T);
                            changes.Item.Add(
                                new ChangePoint<T>(cur, default(T), AddRemoveModify.Remove, new P2Int(x, y)));
                        }
                    }

                    FireChange(changes.Item, cmds);
                }
            }
            else
            { // just internals
                for (int y = 0; y < arr.GetLength(0); y++)
                {
                    for (int x = 0; x < arr.GetLength(1); x++)
                    {
                        arr[y, x] = default(T);
                    }
                }
            }
        }

        public override T Get(P2Int p) => arr[p.Y, p.X];

        public override IEnumerator<P2IntValue<T>> GetEnumerator()
        {
            for (int y = 0; y < arr.GetLength(0); y++)
            {
                for (int x = 0; x < arr.GetLength(1); x++)
                {
                    yield return new P2IntValue<T>(x, y, arr[y, x]);
                }
            }
        }

        public override bool Remove(P2IntValue<T> item, NotifyingFireParameters cmds = default(NotifyingFireParameters))
        {
            cmds = ProcessCmds(cmds);
            var cur = arr[item.Y, item.X];
            if (!object.Equals(item.Value, cur)) return false;
            arr[item.Y, item.X] = default(T);
            if (!HasSubscribers()) return true;
            FireChange(
                new ChangePoint<T>(cur, default(T), AddRemoveModify.Remove, item.Point).Single(),
                cmds);
            return true;
        }

        public override void Set(P2IntValue<T> item, NotifyingFireParameters cmds = default(NotifyingFireParameters))
        {
            cmds = ProcessCmds(cmds);
            var cur = arr[item.Y, item.X];
            arr[item.Y, item.X] = item;
            if (!HasSubscribers()) return;
            if (object.Equals(cur, item.Value)) return;
            FireChange(
                new ChangePoint<T>(cur, item, AddRemoveModify.Modify, item.Point).Single(),
                cmds);
        }

        public override void SetTo(IEnumerable<P2IntValue<T>> items, NotifyingFireParameters cmds = default(NotifyingFireParameters))
        {
            throw new NotImplementedException("Need to implement the clearing of other non-set indices.");
            cmds = ProcessCmds(cmds);
            if (HasSubscribers())
            {
                using (var fire = firePool.Checkout())
                {
                    foreach (var item in items)
                    {
                        var cur = arr[item.Y, item.X];
                        arr[item.Y, item.X] = item;
                        if (!object.Equals(cur, item.Value))
                        {
                            fire.Item.Add(new ChangePoint<T>(cur, item.Value, AddRemoveModify.Modify, item.Point));
                        }
                    }
                    FireChange(fire.Item, cmds);
                }
            }
            else
            {
                foreach (var item in items)
                {
                    arr[item.Y, item.X] = item;
                }
            }
        }

        protected override ObjectPoolCheckout<List<ChangePoint<T>>> CompileCurrent()
        {
            var changes = firePool.Checkout();
            for (int y = 0; y < arr.GetLength(0); y++)
            {
                for (int x = 0; x < arr.GetLength(1); x++)
                {
                    changes.Item.Add(new ChangePoint<T>(default(T), arr[y, x], AddRemoveModify.Add, new P2Int(x, y)));
                }
            }
            return changes;
        }

        protected override ObjectPoolCheckout<List<ChangeAddRem<P2IntValue<T>>>> CompileCurrentEnumer()
        {
            var changes = fireEnumerPool.Checkout();
            for (int y = 0; y < arr.GetLength(0); y++)
            {
                for (int x = 0; x < arr.GetLength(1); x++)
                {
                    changes.Item.Add(new ChangeAddRem<P2IntValue<T>>(new P2IntValue<T>(x, y, arr[y, x]), AddRemove.Add));
                }
            }
            return changes;
        }

        public void Fill(T val, NotifyingFireParameters cmds = default(NotifyingFireParameters))
        {
            cmds = ProcessCmds(cmds);
            if (HasSubscribers())
            {
                using (var fire = firePool.Checkout())
                {
                    for (int y = 0; y < arr.GetLength(0); y++)
                    {
                        for (int x = 0; x < arr.GetLength(1); x++)
                        {
                            var cur = arr[y, x];
                            arr[y, x] = val;
                            if (!object.Equals(cur, val))
                            {
                                fire.Item.Add(new ChangePoint<T>(cur, val, AddRemoveModify.Modify, new P2Int(x, y)));
                            }
                        }
                    }
                    FireChange(fire.Item, cmds);
                }
            }
            else
            {
                for (int y = 0; y < arr.GetLength(0); y++)
                {
                    for (int x = 0; x < arr.GetLength(1); x++)
                    {
                        arr[y, x] = val;
                    }
                }
            }
        }
    }
}
