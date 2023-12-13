using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DiniMad.NavigationProperty;

[Generator]
public class NavigationPropertyGenerator : IIncrementalGenerator
{
    private static readonly string BaseNavigationNamespace   = typeof(NavigationProperty<>).Namespace!;
    private const           string BaseNavigationName        = nameof(NavigationProperty);
    private const           string EntityNavigationClassName = "Navigations";
    private const           string MarkerInterfaceName       = nameof(IWithNavigationProperties);

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var syntaxProvider = context.SyntaxProvider.CreateSyntaxProvider(IsTarget, Transform).Where(FilterOutNull);
        
        context.RegisterSourceOutput(syntaxProvider.Collect(), Execute!);
    }


    private static bool IsTarget(SyntaxNode node, CancellationToken _ = default)
    {
        if (node is not ClassDeclarationSyntax {BaseList.Types.Count: > 0} classDeclaration) return false;

        var baseTypes = classDeclaration.BaseList?.Types.Select(baseType => baseType);
        return baseTypes is not null && baseTypes.Any(static baseType => baseType.ToString() == MarkerInterfaceName);
    }

    private static ITypeSymbol? Transform(GeneratorSyntaxContext context, CancellationToken _ = default)
    {
        var classDeclaration = (ClassDeclarationSyntax) context.Node;
        var symbol           = context.SemanticModel.GetDeclaredSymbol(classDeclaration);
        if (symbol is ITypeSymbol typeSymbol) return typeSymbol;
        return null;
    }

    private static bool FilterOutNull(ITypeSymbol? symbol) => symbol is not null;
    
    private static void Execute(SourceProductionContext context, ImmutableArray<ITypeSymbol> typeSymbols)
    {
        if (typeSymbols.IsDefaultOrEmpty) return;

        foreach (var typeSymbol in typeSymbols)
        {
            var navigationSource = GenerateNavigationSource(typeSymbol);
            context.AddSource($"{typeSymbol.Name}.g.cs", navigationSource);
        }
    }
    
    private static string GenerateNavigationSource(ITypeSymbol targetType)
    {
        return $$"""
                 // Auto-generated code
                 #nullable enable

                 using {{BaseNavigationNamespace}};

                 namespace {{targetType.ContainingNamespace}}
                 {
                    partial class {{targetType.Name}}
                    {
                        public class {{EntityNavigationClassName}} : {{BaseNavigationName}}<{{targetType.Name}}>
                        {
                 {{GenerateNavigationPropertyMethods(targetType)}}
                        }
                    }
                 }
                 """;
    }
    private static string GenerateNavigationPropertyMethods(ITypeSymbol targetType)
    {
        var properties = targetType.GetMembers()
                                   .OfType<IPropertySymbol>()
                                   .Where(symbol => symbol.IsVirtual)
                                   .Select(symbol => GenerateMethod(symbol, targetType))
                                   .ToArray();

        return string.Join("\n\n", properties);
    }
    private static string GenerateMethod(IPropertySymbol symbol, ITypeSymbol targetType)
    {
        var symbolType     = (INamedTypeSymbol) symbol.Type;
        var targetTypeName = symbolType.IsGenericType ? symbolType.TypeArguments[0].Name : symbolType.Name;
        return $$"""
            public static {{BaseNavigationName}}<{{targetType.Name}}> {{symbol.Name.Replace(targetType.Name, string.Empty)}} (params {{BaseNavigationName}}<{{targetTypeName}}>[] thenIncludes)
            {
               var includes = thenIncludes.SelectMany(navigation => navigation.Includes).ToArray();
               return {{BaseNavigationName}}<{{targetType.Name}}>.Create("{{symbol.Name}}", includes);
            }
""";
    }

}