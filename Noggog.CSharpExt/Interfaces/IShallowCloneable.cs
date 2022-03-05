using System;

namespace Noggog;

public interface IShallowCloneable
{
    object ShallowClone();
}

public interface IShallowCloneable<T> : IShallowCloneable
{
    new T ShallowClone();
}