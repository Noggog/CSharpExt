using System;

namespace Noggog;

public interface IShallowCloneable
{
    object ShallowClone();
}