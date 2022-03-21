using System;

namespace Noggog;

public interface IShallowCloneable
{
    object ShallowClone();
}

public interface IShallowCloneable<out T> : IShallowCloneable
{
    new T ShallowClone();
}