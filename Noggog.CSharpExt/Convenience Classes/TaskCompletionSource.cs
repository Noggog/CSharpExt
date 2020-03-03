using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noggog
{
    public class TaskCompletionSource : TaskCompletionSource<bool>
    {
        public void Complete()
        {
            this.SetResult(true);
        }
    }
}
