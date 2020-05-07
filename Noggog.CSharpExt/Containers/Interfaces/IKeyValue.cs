using System;
using System.Collections.Generic;
using System.Text;

namespace Noggog
{
    /// <summary>
    /// A keyed value.
    /// Useful compared to KeyValuePair as the interface is covariant
    /// </summary>
    /// <typeparam name="TObject">The type of the object.</typeparam>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    public interface IKeyValue<out TObject, out TKey>
    {
        /// <summary>
        /// The key
        /// </summary>
        TKey Key{ get; }

        /// <summary>
        /// The value
        /// </summary>
        TObject Value { get; }
    }
}
