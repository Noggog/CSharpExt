using System;

namespace System
{
    public static class ActionExt
    {
        private static Action nothing = new Action(() => { });
        public static Action Nothing
        {
            get { return nothing; }
        }
    }
}
