using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Noggog.SourceGenerators.AssemblyVersion;

[Generator]
public class AssemblyVersionGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var invocations = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (node, _) => node is InvocationExpressionSyntax,
                transform: static (node, _) => (InvocationExpressionSyntax)node.Node)
            .Select(static (invocation, cancel) =>
            {
                cancel.ThrowIfCancellationRequested();
                if (invocation.Expression is not MemberAccessExpressionSyntax memberAccessExpressionSyntax) return null;
                if (memberAccessExpressionSyntax.Expression is not IdentifierNameSyntax nameSyntax) return null;
                if (nameSyntax.ToString() != "AssemblyVersions") return null;
                if (memberAccessExpressionSyntax.Name is not GenericNameSyntax genName) return null;
                if (genName.Identifier.ToString() != "For") return null;
                if (genName.TypeArgumentList.Arguments.Count != 1) return null;
                if (genName.TypeArgumentList.Arguments[0] is not NameSyntax typeNameSyntax) return null;

                return typeNameSyntax;
            })
            .Where(x => x != null);

        var combination = context.CompilationProvider
            .Combine(invocations.Collect());
        
        context.RegisterSourceOutput(combination, (sourceContext, data) =>
        {
            var (compilation, classes) = data;
            
            Dictionary<IAssemblySymbol, HashSet<INamedTypeSymbol>> targets = new(SymbolEqualityComparer.Default);
            var namespaces = new HashSet<string>()
            {
                "System",
                "System.Reflection",
                "System.Diagnostics"
            };

            foreach (var identifier in classes)
            {
                if (identifier == null) continue;
                var model = compilation.GetSemanticModel(identifier.SyntaxTree.GetRoot().SyntaxTree);
                var typeInfo = model.GetTypeInfo(identifier);
                if (typeInfo.Type is not INamedTypeSymbol namedTypeSymbol)
                {
                    sourceContext.ReportDiagnostic(Diagnostic.Create(
                        new DiagnosticDescriptor(
                            "SY0001",
                            "Unknown type passed to AssemblyVersions",
                            "Unknown type passed to AssemblyVersions",
                            "AssemblyVersions",
                            DiagnosticSeverity.Error,
                            true,
                            description: "Need to pass a known concrete type, rather than a generic."), identifier.GetLocation()));
                    continue;
                }

                if (!targets.TryGetValue(namedTypeSymbol.ContainingAssembly, out var set))
                {
                    set = new(SymbolEqualityComparer.Default);
                    targets[namedTypeSymbol.ContainingAssembly] = set;
                }

                set.Add(namedTypeSymbol);
                namespaces.Add(namedTypeSymbol.ContainingNamespace.ToString());
            }

            var sb = new StringBuilder();
            foreach (var ns in namespaces.OrderBy(x => x))
            {
                sb.AppendLine($"using {ns};");
            }

            sb.AppendLine(
                """
                
                #nullable enable
                
                /// <summary>
                /// Struct holding the information about an Assembly's version
                /// </summary>
                /// <param name="PrettyName">Name of the assembly</param>
                /// <param name="ProductVersion">Version string for the assembly</param>
                public record AssemblyVersions(string PrettyName, string? ProductVersion)
                {
                """);
            foreach (var pair in targets)
            {
                INamedTypeSymbol? first = null;
                foreach (var item in pair.Value)
                {
                    if (first == null)
                    {
                        var attrs = item.ContainingAssembly.GetAttributes();
                        var vers = item.ContainingAssembly.GetAttributes()
                            .Where(x => x.AttributeClass?.Name == "AssemblyInformationalVersionAttribute")
                            .FirstOrDefault()?
                            .ConstructorArguments[0].Value?.ToString() ?? "0.0.0.0";
                        var pretty = item.ContainingAssembly.GetAttributes()
                            .Where(x => x.AttributeClass?.Name == "AssemblyTitleAttribute")
                            .FirstOrDefault()?
                            .ConstructorArguments[0].Value?.ToString() ?? "<global assembly>";
                        sb.AppendLine($"    private static readonly AssemblyVersions _{item.Name} = new(\"{pretty}\", \"{vers}\");");
                        first = item;
                    }
                    else
                    {
                        sb.AppendLine($@"    private static readonly AssemblyVersions _{item.Name} = _{first.Name};");
                    }
                }
            }

            sb.AppendLine(
                """
                
                    /// <summary>
                    /// Gets the assembly version information for a given type
                    /// </summary>
                    /// <typeparam name="TTypeFromAssembly">Type to get information about</typeparam>
                    /// <returns>Structure containing the assembly version information</returns>
                    public static AssemblyVersions For<TTypeFromAssembly>()
                    {
                        var t = typeof(TTypeFromAssembly);
                """);
            
            foreach (var item in targets.SelectMany(x => x.Value))
            {
                sb.AppendLine($"        if (t == typeof({item.ContainingNamespace}.{item.Name})) return _{item.Name};");
            }
            sb.AppendLine(@"
        throw new NotImplementedException();
    }
}");
            sourceContext.AddSource("AssemblyVersions.g.cs", sb.ToString());
        });
    }
}