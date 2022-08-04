#if NETSTANDARD2_0 
#else
using System.Threading.Channels;

namespace Noggog.WorkEngine;

public interface IWorkQueue
{
    ChannelReader<IToDo> Reader { get; }
}
#endif