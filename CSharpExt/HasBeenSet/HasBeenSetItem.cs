using System;

namespace Noggog
{
    public static class HasBeenSetItem
    {
        public static IHasBeenSetItem<T> Factory<T>(
            T defaultVal = default(T),
            bool markAsSet = false,
            Func<T> noNullFallback = null,
            Action<T> onSet = null,
            Func<T, T> converter = null)
        {
            if (noNullFallback == null)
            {
                if (onSet == null)
                {
                    if (converter == null)
                    {
                        return new HasBeenSetItem<T>(
                            defaultVal: defaultVal,
                            markAsSet: markAsSet);
                    }
                    else
                    {
                        return new HasBeenSetItemConverter<T>(
                            converter,
                            defaultVal: defaultVal,
                            markAsSet: markAsSet);
                    }
                }
                else
                {
                    if (converter == null)
                    {
                        return new HasBeenSetItemOnSet<T>(
                            onSet: onSet,
                            defaultVal: defaultVal,
                            markAsSet: markAsSet);
                    }
                    else
                    {
                        return new HasBeenSetItemConverterOnSet<T>(
                            converter: converter,
                            onSet: onSet,
                            defaultVal: defaultVal,
                            markAsSet: markAsSet);
                    }
                }
            }
            else
            {
                if (onSet == null)
                {
                    if (converter == null)
                    {
                        return new HasBeenSetItemNoNull<T>(
                            defaultFallback: noNullFallback,
                            defaultVal: defaultVal,
                            markAsSet: markAsSet);
                    }
                    else
                    {
                        return new HasBeenSetItemNoNullConverter<T>(
                            defaultFallback: noNullFallback,
                            converter: converter,
                            defaultVal: defaultVal,
                            markAsSet: markAsSet);
                    }
                }
                else
                {
                    if (converter == null)
                    {
                        return new HasBeenSetItemNoNullOnSet<T>(
                            defaultFallback: noNullFallback,
                            onSet: onSet,
                            defaultVal: defaultVal,
                            markAsSet: markAsSet);
                    }
                    else
                    {
                        return new HasBeenSetItemNoNullOnSetConverter<T>(
                            defaultFallback: noNullFallback,
                            onSet: onSet,
                            converter: converter,
                            defaultVal: defaultVal,
                            markAsSet: markAsSet);
                    }
                }
            }
        }

        public static IHasBeenSetItem<T> FactoryNoNull<T>(
            T defaultVal = default(T),
            bool markAsSet = false,
            Action<T> onSet = null,
            Func<T, T> converter = null)
            where T : new()
        {
            if (onSet == null)
            {
                if (converter == null)
                {
                    return new HasBeenSetItemNoNullDirect<T>(
                        defaultVal: defaultVal,
                        markAsSet: markAsSet);
                }
                else
                {
                    return new HasBeenSetItemNoNullDirectConverter<T>(
                        converter,
                        defaultVal: defaultVal,
                        markAsSet: markAsSet);
                }
            }
            else
            {
                if (converter == null)
                {
                    return new HasBeenSetItemNoNullDirectOnSet<T>(
                        onSet: onSet,
                        defaultVal: defaultVal,
                        markAsSet: markAsSet);
                }
                else
                {
                    return new HasBeenSetItemNoNullDirectOnSetConverter<T>(
                        converter: converter,
                        onSet: onSet,
                        defaultVal: defaultVal,
                        markAsSet: markAsSet);
                }
            }
        }
    }
    
    public class HasBeenSetItem<T> : IHasBeenSetItem<T>
    {
        private T _Item;
        public T Item
        {
            get => _Item;
            set => Set(value);
        }
        public bool HasBeenSet { get; set; }
        public T DefaultValue { get; private set; }

        public HasBeenSetItem(
            T defaultVal = default(T),
            bool markAsSet = false)
        {
            this.DefaultValue = defaultVal;
            this._Item = defaultVal;
            this.HasBeenSet = markAsSet;
        }

        public void Set(T item, bool hasBeenSet = true)
        {
            this._Item = item;
            this.HasBeenSet = hasBeenSet;
        }

        public void Unset()
        {
            this._Item = DefaultValue;
            this.HasBeenSet = false;
        }

        public void SetCurrentAsDefault()
        {
            this.DefaultValue = this._Item;
        }
    }
}
