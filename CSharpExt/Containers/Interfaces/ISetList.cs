﻿using Noggog.Notifying;
using System;
using System.Collections.Generic;
using System.Text;

namespace Noggog
{
    public interface ISetList<T> : IList<T>, IHasBeenSet
    {
    }

    public interface IReadOnlySetList<T> : IReadOnlyList<T>, IHasBeenSetGetter
    {
    }
}