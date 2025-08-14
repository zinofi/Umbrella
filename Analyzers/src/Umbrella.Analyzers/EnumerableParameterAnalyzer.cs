using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Umbrella.Analyzers;

/// <summary>
/// An analyzer that checks if method parameters that implement <see cref="IEnumerable{T}" /> are specified as <see cref="IEnumerable{T}" /> instead of their concrete type.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class EnumerableParameterAnalyzer : DiagnosticAnalyzer
{
	/// <summary>
	/// The diagnostic ID for this analyzer.
	/// </summary>
	public const string DiagnosticId = "UA005";

	/// <summary>
	/// Gets the diagnostic rule for the analyzer.
	/// </summary>
	public static readonly DiagnosticDescriptor Rule = new(
		DiagnosticId,
		"Method parameters should use IEnumerable<T>",
		"Parameter '{0}' should be specified as IEnumerable<T> instead of {1}",
		"CodeStyle",
		DiagnosticSeverity.Error,
		isEnabledByDefault: true);

	/// <inheritdoc />
	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

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

		foreach (var parameter in methodSymbol.Parameters)
		{
			var parameterType = parameter.Type;

			if (parameterType.OriginalDefinition.ToDisplayString() is not "IEnumerable<>")
			{
				var diagnostic = Diagnostic.Create(Rule, parameter.Locations[0], parameter.Name, parameterType.ToDisplayString());
				context.ReportDiagnostic(diagnostic);
			}
		}
	}
}