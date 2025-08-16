using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Umbrella.Analyzers;

/// <summary>
/// An analyzer that checks if methods return types implement <see cref="IReadOnlyCollection{T}" />
/// and ensures that methods returning collections use <see cref="IReadOnlyCollection{T}" /> instead of concrete collection types.
/// Additionally, it ensures that tuples (both language tuples and <see cref="Tuple{T}" />) are not used as return types.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class ReadOnlyCollectionReturnTypeAnalyzer : DiagnosticAnalyzer
{
	/// <summary>
	/// The diagnostic ID for this analyzer.
	/// </summary>
	public const string DiagnosticId = "UA006";

	/// <summary>
	/// Gets the diagnostic rule for the analyzer.
	/// </summary>
	public static readonly DiagnosticDescriptor Rule = new(
		DiagnosticId,
		"Method return types should use IReadOnlyCollection<T>",
		"Method '{0}' should return IReadOnlyCollection<T> instead of {1}",
		"CodeStyle",
		DiagnosticSeverity.Error,
		isEnabledByDefault: true);

	/// <inheritdoc />
	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [Rule];

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
		if (returnType is INamedTypeSymbol namedType && namedType.IsGenericType &&
			(namedType.Name == "Task" || namedType.Name == "ValueTask"))
		{
			returnType = namedType.TypeArguments[0];
		}

		// Get the display string once
		string returnTypeDisplay = returnType.ToDisplayString();

		// Check if the return type is IReadOnlyCollection<T>
		if (returnType.OriginalDefinition.ToDisplayString() != "IReadOnlyCollection<>")
		{
			ReportDiagnostic(context, methodSymbol, returnTypeDisplay);
			return;
		}

		// Check for language tuples
		if (returnType is INamedTypeSymbol tupleType && tupleType.IsTupleType)
		{
			ReportDiagnostic(context, methodSymbol, "tuple");
			return;
		}

		// Check for System.Tuple<T>
		if (returnTypeDisplay.StartsWith("System.Tuple", StringComparison.Ordinal))
		{
			ReportDiagnostic(context, methodSymbol, returnTypeDisplay);
		}
	}

	private static void ReportDiagnostic(SymbolAnalysisContext context, IMethodSymbol methodSymbol, string returnTypeDisplay)
	{
		var diagnostic = Diagnostic.Create(Rule, methodSymbol.Locations[0], methodSymbol.Name, returnTypeDisplay);
		context.ReportDiagnostic(diagnostic);
	}
}