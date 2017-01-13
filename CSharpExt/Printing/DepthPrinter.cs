using System;
using System.Collections.Generic;
using System.Text;

namespace Noggog.Printing
{
    public class DepthPrinter
    {
        internal int depth = 0;
        public string DepthBuffer = "    ";
        private List<KeyValuePair<int, List<string>>> list = new List<KeyValuePair<int, List<string>>>();

        public void AddLine(params object[] objs)
        {
            List<string> strs = new List<string>(objs.Length);
            foreach (var o in objs)
            {
                strs.Add(o.ToString());
            }
            list.Add(new KeyValuePair<int, List<string>>(depth, strs));
        }

        public void Add(DepthPrinter rhs)
        {
            foreach (var line in rhs.list)
            {
                list.Add(new KeyValuePair<int, List<string>>(line.Key, new List<string>(line.Value)));
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var line in list)
            {
                for (int i = 0; i < line.Key; i++)
                {
                    sb.Append(DepthBuffer);
                }
                foreach (var s in line.Value)
                {
                    sb.Append(s);
                }
                sb.Append("\n");
            }
            return sb.ToString();
        }

        public DepthCounter IncrementDepth()
        {
            return new DepthCounter(this);
        }

        public void Shift(int amount)
        {
            for (int i = 0; i < list.Count; i++)
            {
                var line = list[i];
                list[i] = new KeyValuePair<int, List<string>>(line.Key + amount, line.Value);
            }
        }
    }

    public class DepthCounter : IDisposable
    {
        DepthPrinter printer;

        internal DepthCounter(DepthPrinter printer)
        {
            printer.depth++;
            this.printer = printer;
        }

        public void Dispose()
        {
            printer.depth--;
        }
    }
}
