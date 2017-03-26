using System;

namespace System
{
    public static class ActionExt
    {
        public static readonly Action Nothing = new Action(() => { });
    }
}
