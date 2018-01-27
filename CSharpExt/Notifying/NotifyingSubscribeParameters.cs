using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noggog.Notifying
{
    public class NotifyingSubscribeParameters
    {
        public static readonly NotifyingSubscribeParameters Typical = new NotifyingSubscribeParameters(
            fireInitial: true);
        public static readonly NotifyingSubscribeParameters NoFire = new NotifyingSubscribeParameters(
            fireInitial: false);

        public readonly bool FireInitial;

        public NotifyingSubscribeParameters(bool fireInitial = true)
        {
            this.FireInitial = fireInitial;
        }
    }
}
