#if NETSTANDARD2_0

using System.Threading.Tasks;

namespace Noggog
{
    public class TaskCompletionSource : TaskCompletionSource<bool>
    {
        public void SetResult()
        {
            SetResult(true);
        }
    }
}

#else
#endif