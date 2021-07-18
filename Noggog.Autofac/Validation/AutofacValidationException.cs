using System;

namespace Noggog.Autofac.Validation
{
    public class AutofacValidationException : Exception
    {
        public AutofacValidationException(string message)
            : base(message)
        {
        }
        
        public AutofacValidationException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}