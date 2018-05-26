using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Noggog.Notifying
{
    public class NotifyingFireParameters
    {
        public static readonly NotifyingFireParameters Typical = new NotifyingFireParameters(
            exceptionHandler: null,
            forceFire: false);
        public static readonly NotifyingFireParameters ThrowEvents = new NotifyingFireParameters(
            exceptionHandler: (ex) =>
            {
                if (ex == null) return;
                throw ex;
            },
            forceFire: false);
        
        public readonly Action<Exception> ExceptionHandler;
        public readonly bool ForceFire;

        public NotifyingFireParameters(
            Action<Exception> exceptionHandler = null,
            bool forceFire = false)
        {
            this.ExceptionHandler = exceptionHandler;
            this.ForceFire = forceFire;
        }
    }

    public static class NotifyingFireParametersExt
    {
        public static NotifyingUnsetParameters ToUnsetParams(this NotifyingFireParameters param)
        {
            if (param == null) return null;
            return new NotifyingUnsetParameters(
                exceptionHandler: param.ExceptionHandler,
                forceFire: param.ForceFire);
        }

        public static NotifyingFireParameters ToFireParams(this NotifyingFireParameters param)
        {
            return param;
        }
    }
}
