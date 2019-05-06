using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noggog.WPF
{
    public static class ObservableUtility
    {
        /// <summary>
        /// A convenience function to make a flip flop observable that triggers whenever the source observable changes
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obs">Source observable</param>
        /// <param name="span">How long to delay before flopping back to false</param>
        /// <returns>Observable that turns true when source observable changes, then back to false after a time period</returns>
        public static IObservable<bool> FlipFlop<T>(IObservable<T> obs, TimeSpan span)
        {
            return Observable.Merge(
                obs.Select(f => true),
                obs.Delay(span).Select(f => false));
        }
    }
}
