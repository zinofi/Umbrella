using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace Umbrella.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class UmbrellaAnalyzersAnalyzer : DiagnosticAnalyzer
{
	public const string DiagnosticId = "UmbrellaAnalyzers";

	private const string Title = "{0}";
	private const string MessageFormat = "{0}";
	private const string Description = "{0}";
	private const string Category = "Naming";

	private static readonly DiagnosticDescriptor _rule = new(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);
	
	/// <inheritdoc/>
	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(_rule); } }

	public override void Initialize(AnalysisContext context)
	{
		context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
		context.EnableConcurrentExecution();

		// TODO: Consider registering other actions that act on syntax instead of or in addition to symbols
		// See https://github.com/dotnet/roslyn/blob/main/docs/analyzers/Analyzer%20Actions%20Semantics.md for more information
		context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.NamedType);
	}

	private static void AnalyzeSymbol(SymbolAnalysisContext context)
	{
		// TODO: Replace the following code with your own analysis, generating Diagnostic objects for any issues you find
		var namedTypeSymbol = (INamedTypeSymbol)context.Symbol;

		// Find just those named type symbols with names containing lowercase letters.
		if (namedTypeSymbol.Name.ToCharArray().Any(char.IsLower))
		{
			// For all such symbols, produce a diagnostic.
			var diagnostic = Diagnostic.Create(_rule, namedTypeSymbol.Locations[0], namedTypeSymbol.Name);

			context.ReportDiagnostic(diagnostic);
		}
	}
}
