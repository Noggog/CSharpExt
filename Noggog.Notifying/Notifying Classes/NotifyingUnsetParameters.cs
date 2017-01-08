using System;

namespace Noggog.Notifying
{
    public struct NotifyingUnsetParameters
    {
        public static readonly NotifyingUnsetParameters Typical = new NotifyingUnsetParameters(
            throwEventExceptions: false,
            forceFire: false);
        public static readonly NotifyingUnsetParameters ThrowEvents = new NotifyingUnsetParameters(
            throwEventExceptions: true,
            forceFire: false);

        public readonly bool ThrowEventExceptions;
        public readonly bool ForceFire;

        public NotifyingUnsetParameters(
            bool throwEventExceptions = false,
            bool forceFire = false)
        {
            this.ThrowEventExceptions = throwEventExceptions;
            this.ForceFire = forceFire;
        }
    }

    public static class NotifyingUnsetParametersExt
    {
        public static NotifyingFireParameters? ToFireParams(this NotifyingUnsetParameters? param)
        {
            if (param == null) return null;
            return new NotifyingFireParameters(
                markAsSet: false,
                throwEventExceptions: param.Value.ThrowEventExceptions,
                forceFire: param.Value.ForceFire);
        }

        public static NotifyingUnsetParameters? ToUnsetParams(this NotifyingUnsetParameters? param)
        {
            return param;
        }
    }
}
