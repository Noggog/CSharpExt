using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noggog.Notifying
{
    public class NotifyingBindParameters
    {
        public static readonly NotifyingBindParameters Typical = new NotifyingBindParameters(
            NotifyingFireParameters.Typical,
            NotifyingSubscribeParameters.Typical);
        public static readonly NotifyingBindParameters NoFire = new NotifyingBindParameters(
            NotifyingFireParameters.Typical,
            NotifyingSubscribeParameters.NoFire);
        public readonly NotifyingFireParameters FireParameters;
        public readonly NotifyingSubscribeParameters SubscribeParameters;

        public NotifyingBindParameters(
            NotifyingFireParameters fireParams = null,
            NotifyingSubscribeParameters subParams = null)
        {
            this.FireParameters = fireParams;
            this.SubscribeParameters = subParams;
        }
    }
}
