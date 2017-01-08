using System;

namespace Noggog.Notifying
{
    public struct NotifyingUnsetParameters
    {
        public static readonly NotifyingUnsetParameters Typical = new NotifyingUnsetParameters(
            exceptionHandler: null,
            forceFire: false);
        public static readonly NotifyingUnsetParameters ThrowEvents = new NotifyingUnsetParameters(
            exceptionHandler: (ex) =>
            {
                if (ex == null) return;
                throw ex;
            },
            forceFire: false);

        public readonly Action<Exception> ExceptionHandler;
        public readonly bool ForceFire;

        public NotifyingUnsetParameters(
            Action<Exception> exceptionHandler = null,
            bool forceFire = false)
        {
            this.ExceptionHandler = exceptionHandler;
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
                exceptionHandler: param.Value.ExceptionHandler,
                forceFire: param.Value.ForceFire);
        }

        public static NotifyingUnsetParameters? ToUnsetParams(this NotifyingUnsetParameters? param)
        {
            return param;
        }
    }
}
