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

		List<ITypeSymbol> lstReturnTypeToCheck = [];

		// Check for Task<T> or ValueTask<T>
		if (returnType is INamedTypeSymbol namedType && namedType.IsGenericType &&
			(namedType.Name == "Task" || namedType.Name == "ValueTask"))
		{
			lstReturnTypeToCheck.Add(namedType.TypeArguments[0]);
		}
		else if (returnType is INamedTypeSymbol tupleType && tupleType.IsTupleType)
		{
			lstReturnTypeToCheck.AddRange(tupleType.TupleElements.Select(e => e.Type));
		}
		else if (returnType is INamedTypeSymbol tupleSymbol && tupleSymbol.OriginalDefinition.ToDisplayString().StartsWith("System.Tuple", StringComparison.Ordinal))
		{
			lstReturnTypeToCheck.AddRange(tupleSymbol.TypeArguments);
		}
		else
		{
			lstReturnTypeToCheck.Add(returnType);
		}

		foreach (var type in lstReturnTypeToCheck)
		{
			// Check if the type is a collection type
			if (!type.IsCollectionType())
				continue;

			// Get the display string once
			string returnTypeDisplay = type.ToDisplayString();

			// Check if the return type is IReadOnlyCollection<T>
			if (type.OriginalDefinition.ToDisplayString() != "System.Collections.Generic.IReadOnlyCollection<T>")
			{
				ReportDiagnostic(context, methodSymbol, returnTypeDisplay);
				return;
			}
		}
	}

	private static void ReportDiagnostic(SymbolAnalysisContext context, IMethodSymbol methodSymbol, string returnTypeDisplay)
	{
		var diagnostic = Diagnostic.Create(Rule, methodSymbol.Locations[0], methodSymbol.Name, returnTypeDisplay);
		context.ReportDiagnostic(diagnostic);
	}
}

/// <summary>
/// Extension methods for the <see cref="ITypeSymbol" /> interface.
/// </summary>
[SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "False positive.")]
public static class ITypeSymbolExtensions
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
	extension(ITypeSymbol? type)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
	{
		/// <summary>
		/// Checks if the type implements <see cref="IEnumerable{T}" /> or is a collection type.
		/// </summary>
		/// <returns></returns>
		public bool IsCollectionType()
		{
			if (type is null)
				return false;

			// Check if the type implements IEnumerable<T> but is not string
			if (type.SpecialType == SpecialType.System_String)
				return false;

			foreach (var interfaceType in type.AllInterfaces)
			{
				if (interfaceType.OriginalDefinition.ToDisplayString() == "System.Collections.Generic.IEnumerable<T>")
					return true;
			}

			return false;
		}
	}
}