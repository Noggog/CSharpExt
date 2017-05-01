using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noggog.Notifying
{
    public class HasBeenSetGetter : IHasBeenSet
    {
        public static readonly HasBeenSetGetter NotBeenSet_Instance = new HasBeenSetGetter(false);
        public static readonly HasBeenSetGetter HasBeenSet_Instance = new HasBeenSetGetter(true);

        public readonly bool HasBeenSet;
        bool IHasBeenSet.HasBeenSet => this.HasBeenSet;

        public HasBeenSetGetter(bool on)
        {
            this.HasBeenSet = on;
        }
    }

}
