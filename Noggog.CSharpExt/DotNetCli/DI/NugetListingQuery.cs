namespace Noggog.DotNetCli.DI;

public record NugetListingQuery(string Package, string Requested, string Resolved, string? Latest);