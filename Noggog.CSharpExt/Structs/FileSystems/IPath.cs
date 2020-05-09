using System;
using System.Collections.Generic;
using System.Text;

namespace Noggog
{
    public interface IPath
    {
        bool Exists { get; }
        string Path { get; }
        string Name { get; }
    }
}
