using System.Threading.Channels;

namespace Noggog.Tooling.WorkEngine;

public interface IWorkQueue
{
    ChannelReader<IToDo> Reader { get; }
}