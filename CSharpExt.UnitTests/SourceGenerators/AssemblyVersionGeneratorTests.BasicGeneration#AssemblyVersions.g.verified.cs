//HintName: AssemblyVersions.g.cs
using System;
using System.Diagnostics;
using System.Reflection;

#nullable enable

/// <summary>
/// Struct holding the information about an Assembly's version
/// </summary>
/// <param name="PrettyName">Name of the assembly</param>
/// <param name="ProductVersion">Version string for the assembly</param>
public record AssemblyVersions(string PrettyName, string? ProductVersion)
{

    /// <summary>
    /// Gets the assembly version information for a given type
    /// </summary>
    /// <typeparam name="TTypeFromAssembly">Type to get information about</typeparam>
    /// <returns>Structure containing the assembly version information</returns>
    public static AssemblyVersions For<TTypeFromAssembly>()
    {
        var t = typeof(TTypeFromAssembly);

        throw new NotImplementedException();
    }
}
