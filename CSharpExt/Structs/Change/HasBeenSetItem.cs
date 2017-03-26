using System;

namespace Noggog.Notifying
{
    public class HasBeenSetGetter : IHasBeenSetGetter
    {
        public static readonly HasBeenSetGetter NotBeenSet_Instance = new HasBeenSetGetter(false);
        public static readonly HasBeenSetGetter HasBeenSet_Instance = new HasBeenSetGetter(true);

        public readonly bool HasBeenSet;
        bool IHasBeenSetGetter.HasBeenSet => this.HasBeenSet;

        public HasBeenSetGetter(bool on)
        {
            this.HasBeenSet = on;
        }
    }

    // Keep class, as it needs pass by reference
    public class HasBeenSetItem<T> : IHasBeenSet<T>
    {
        private T _Item;
        public T Item { get { return _Item; } set { Set(value); } }
        public bool HasBeenSet;

        bool IHasBeenSet<T>.HasBeenSet { get { return this.HasBeenSet; } set { this.HasBeenSet = value; } }

        bool IHasBeenSetGetter.HasBeenSet { get { return this.HasBeenSet; } }

        public void Set(T item)
        {
            this._Item = item;
            this.HasBeenSet = true;
        }

        public void Unset()
        {
            this._Item = default(T);
            this.HasBeenSet = false;
        }

        void IHasBeenSet<T>.Set(T value, NotifyingFireParameters? cmds)
        {
            this.Set(value);
        }

        void IHasBeenSet<T>.Unset(NotifyingUnsetParameters? cmds)
        {
            this.Unset();
        }

        public void SetHasBeenSet(bool on)
        {
            this.HasBeenSet = on;
        }
    }

    public interface IHasBeenSetGetter
    {
        bool HasBeenSet { get; }
    }

    public interface IHasBeenSet<T> : IHasBeenSetGetter
    {
        new bool HasBeenSet { get; set; }
        void Set(T value, NotifyingFireParameters? cmds);
        void Unset(NotifyingUnsetParameters? cmds);
        void SetHasBeenSet(bool on);
    }
}
