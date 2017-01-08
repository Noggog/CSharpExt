using System;

namespace Noggog.Notifying
{
    public struct NotifyingFireParameters
    {
        public static readonly NotifyingFireParameters Typical = new NotifyingFireParameters(
            markAsSet: true,
            throwEventExceptions: false,
            forceFire: false);
        public static readonly NotifyingFireParameters ThrowEvents = new NotifyingFireParameters(
            markAsSet: true,
            throwEventExceptions: true,
            forceFire: false);

        public readonly bool MarkAsSet;
        public readonly bool ThrowEventExceptions;
        public readonly bool ForceFire;

        public NotifyingFireParameters(
            bool markAsSet = true,
            bool throwEventExceptions = false,
            bool forceFire = false)
        {
            this.MarkAsSet = markAsSet;
            this.ThrowEventExceptions = throwEventExceptions;
            this.ForceFire = forceFire;
        }
    }

    public static class NotifyingFireParametersExt
    {
        public static NotifyingUnsetParameters? ToUnsetParams(this NotifyingFireParameters? param)
        {
            if (param == null) return null;
            return new NotifyingUnsetParameters(
                throwEventExceptions: param.Value.ThrowEventExceptions,
                forceFire: param.Value.ForceFire);
        }

        public static NotifyingFireParameters? ToFireParams(this NotifyingFireParameters? param)
        {
            return param;
        }
    }
}
