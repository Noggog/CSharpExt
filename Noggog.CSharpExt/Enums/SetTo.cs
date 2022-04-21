namespace Noggog;

public enum SetTo
{
    /// <summary>
    /// Clear existing collection, and set it to the new set of values
    /// </summary>
    Whitewash,

    /// <summary>
    /// Adds only new values that don't already exist to the collection
    /// </summary>
    SkipExisting,

    /// <summary>
    /// Sets all new values into the collection, replacing existing collisions
    /// </summary>
    SetExisting,
}