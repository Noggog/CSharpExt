#if NETSTANDARD2_0

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