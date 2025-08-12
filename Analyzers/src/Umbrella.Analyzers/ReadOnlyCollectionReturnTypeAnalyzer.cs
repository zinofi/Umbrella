using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Umbrella.Analyzers;

/// <summary>
/// An analyzer that checks if methods return types implement <see cref="IReadOnlyCollection{T}" />
/// and ensures that methods returning collections use <see cref="IReadOnlyCollection{T}" /> instead of concrete collection types.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class ReadOnlyCollectionReturnTypeAnalyzer : DiagnosticAnalyzer
{
	/// <summary>
	/// The diagnostic ID for this analyzer.
	/// </summary>
	public const string DiagnosticId = "UA006";

	private static readonly DiagnosticDescriptor _rule = new(
		DiagnosticId,
		"Method return types should use IReadOnlyCollection<T>",
		"Method '{0}' should return IReadOnlyCollection<T> instead of {1}",
		"CodeStyle",
		DiagnosticSeverity.Error,
		isEnabledByDefault: true);

	/// <inheritdoc />
	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [_rule];

	/// <inheritdoc />
	public override void Initialize(AnalysisContext context)
	{
		if (context is null)
			throw new ArgumentNullException(nameof(context));

		context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
		context.EnableConcurrentExecution();
		context.RegisterSymbolAction(AnalyzeMethod, SymbolKind.Method);
	}

	private static void AnalyzeMethod(SymbolAnalysisContext context)
	{
		var methodSymbol = (IMethodSymbol)context.Symbol;
		var returnType = methodSymbol.ReturnType;

		// Check for Task<T> or ValueTask<T>
		if (returnType is INamedTypeSymbol namedType &&
			(namedType.Name == "Task" || namedType.Name == "ValueTask") &&
			namedType.IsGenericType)
		{
			returnType = namedType.TypeArguments[0];
		}

		// Check if the return type implements IReadOnlyCollection<T>
		if (returnType.AllInterfaces.Any(i => i.OriginalDefinition.ToDisplayString() == "System.Collections.Generic.IReadOnlyCollection<T>") &&
			returnType.OriginalDefinition.ToDisplayString() != "System.Collections.Generic.IReadOnlyCollection<T>")
		{
			var diagnostic = Diagnostic.Create(_rule, methodSymbol.Locations[0], methodSymbol.Name, returnType.ToDisplayString());
			context.ReportDiagnostic(diagnostic);
		}
	}
}