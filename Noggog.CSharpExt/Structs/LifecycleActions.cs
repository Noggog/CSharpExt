namespace Noggog;

public struct LifecycleActions<T>
{
    public readonly Action<T>? OnCreate;
    public readonly Action<T>? OnGet;
    public readonly Action<T>? OnReturn;
    public readonly Action<T>? OnDestroy;

    public LifecycleActions(
        Action<T>? onCreate = null,
        Action<T>? onGet = null,
        Action<T>? onReturn = null,
        Action<T>? onDestroy = null)
    {
        OnCreate = onCreate;
        OnGet = onGet;
        OnReturn = onReturn;
        OnDestroy = onDestroy;
    }

    public static LifecycleActions<T> operator +(LifecycleActions<T> l1, LifecycleActions<T> l2)
    {
        return new LifecycleActions<T>(
            onCreate: l1.OnCreate + l2.OnCreate,
            onDestroy: l1.OnDestroy + l2.OnDestroy,
            onGet: l1.OnGet + l2.OnGet,
            onReturn: l1.OnReturn + l2.OnReturn);
    }
}