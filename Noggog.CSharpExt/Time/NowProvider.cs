using System;

namespace Noggog.Time
{
    public interface INowProvider
    {
        public DateTime NowLocal { get; }
    }

    public class NowProvider : INowProvider
    {
        public DateTime NowLocal => DateTime.Now;
    }
}