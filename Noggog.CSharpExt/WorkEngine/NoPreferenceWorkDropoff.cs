namespace Noggog.WorkEngine;

#if NETSTANDARD2_0 
#else

/// <summary>
/// Marker class that behaves like InlineWorkDropoff, but is meant
/// to be replaced by the default work dropoff as prescribed by the implementation its given to
/// </summary>
public class NoPreferenceWorkDropoff : InlineWorkDropoff
{
}

#endif