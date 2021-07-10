using System;

namespace Noggog.Autofac.Validation
{
    public class AutofacValidationException : Exception
    {
        public AutofacValidationException(string message)
            : base(message)
        {
        }
    }
}