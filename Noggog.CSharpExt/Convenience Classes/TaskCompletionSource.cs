using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noggog
{
    /// <summary>
    /// A typeless TaskCompletionSource
    /// </summary>
    public class TaskCompletionSource : TaskCompletionSource<bool>
    {
        /// <summary>
        /// Transitions the underlying Task into the RanToCompletion state.
        /// </summary>
        public void SetResult()
        {
            this.SetResult(true);
        }

        /// <summary>
        /// Transitions the underlying Task into the RanToCompletion state if it can.
        /// Will fail to do so if the task is already complete or is faulted.
        /// </summary>
        public bool TryComplete()
        {
            return this.TrySetResult(true);
        }
    }
}
