using System;

namespace Noggog
{
    public struct LifecycleActions<T>
    {
        public Action<T> OnCreate;
        public Action<T> OnGet;
        public Action<T> OnReturn;
        public Action<T> OnDestroy;

        public static LifecycleActions<T> operator +(LifecycleActions<T> l1, LifecycleActions<T> l2)
        {
            return new LifecycleActions<T>()
            {
                OnCreate = l1.OnCreate + l2.OnCreate,
                OnDestroy = l1.OnDestroy + l2.OnDestroy,
                OnGet = l1.OnGet + l2.OnGet,
                OnReturn = l1.OnReturn + l2.OnReturn
            };
        }
    }
}
