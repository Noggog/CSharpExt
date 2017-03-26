using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Noggog.Notifying
{
    public struct NotifyingFireParameters
    {
        public static readonly NotifyingFireParameters Typical = new NotifyingFireParameters(
            markAsSet: true,
            exceptionHandler: null,
            forceFire: false);
        public static readonly NotifyingFireParameters ThrowEvents = new NotifyingFireParameters(
            markAsSet: true,
            exceptionHandler: (ex) =>
            {
                if (ex == null) return;
                throw ex;
            },
            forceFire: false);

        public readonly bool MarkAsSet;
        public readonly Action<Exception> ExceptionHandler;
        public readonly bool ForceFire;

        public NotifyingFireParameters(
            bool markAsSet = true,
            Action<Exception> exceptionHandler = null,
            bool forceFire = false)
        {
            this.MarkAsSet = markAsSet;
            this.ExceptionHandler = exceptionHandler;
            this.ForceFire = forceFire;
        }
    }

    public static class NotifyingFireParametersExt
    {
        public static NotifyingUnsetParameters? ToUnsetParams(this NotifyingFireParameters? param)
        {
            if (param == null) return null;
            return new NotifyingUnsetParameters(
                exceptionHandler: param.Value.ExceptionHandler,
                forceFire: param.Value.ForceFire);
        }

        public static NotifyingFireParameters? ToFireParams(this NotifyingFireParameters? param)
        {
            return param;
        }
    }
}
