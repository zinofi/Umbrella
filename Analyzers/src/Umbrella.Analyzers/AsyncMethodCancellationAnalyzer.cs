using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Umbrella.Analyzers;

/// <summary>
/// An analyzer that checks if async methods have a CancellationToken parameter.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class AsyncMethodCancellationAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "UA003";

    private static readonly DiagnosticDescriptor _rule = new(
        DiagnosticId,
        "Async methods should have a CancellationToken parameter",
        "Async method '{0}' should have a 'CancellationToken cancellationToken = default' parameter",
        "CodeStyle",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(_rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSymbolAction(AnalyzeMethod, SymbolKind.Method);
    }

    private static void AnalyzeMethod(SymbolAnalysisContext context)
    {
        var methodSymbol = (IMethodSymbol)context.Symbol;

        if (methodSymbol.IsAsync &&
            (methodSymbol.ReturnType.Name == "Task" || methodSymbol.ReturnType.Name == "ValueTask") &&
            !methodSymbol.Parameters.Any(p => p.Type.Name == "CancellationToken"))
        {
            var diagnostic = Diagnostic.Create(_rule, methodSymbol.Locations[0], methodSymbol.Name);
            context.ReportDiagnostic(diagnostic);
        }
    }
}