using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Umbrella.Analyzers;

/// <summary>
/// An analyzer that checks if method return types are appropriate collection types and that they are not null.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class CollectionReturnTypeAnalyzer : DiagnosticAnalyzer
{
	/// <summary>
	/// The diagnostic ID for this analyzer.
	/// </summary>
	public const string DiagnosticId = "UA007";

	private static readonly DiagnosticDescriptor _rule = new(
		DiagnosticId,
		"Method return types should use appropriate collection types",
		"Method '{0}' returns a collection type '{1}' which should be reviewed for consistency",
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
		if (returnType is INamedTypeSymbol namedType && namedType.IsGenericType &&
			(namedType.Name == "Task" || namedType.Name == "ValueTask"))
		{
			returnType = namedType.TypeArguments[0];
		}

		// Check for tuples (language tuples or System.Tuple<T>)
		if (returnType is INamedTypeSymbol tupleType && tupleType.IsTupleType)
		{
			foreach (var element in tupleType.TupleElements)
			{
				CheckCollectionTypeIsNotNull(context, methodSymbol, element.Type);
			}

			return;
		}

		if (returnType.OriginalDefinition.ToDisplayString().StartsWith("System.Tuple", StringComparison.Ordinal))
		{
			foreach (var typeArgument in ((INamedTypeSymbol)returnType).TypeArguments)
			{
				CheckCollectionTypeIsNotNull(context, methodSymbol, typeArgument);
			}

			return;
		}

		// Check if the return type is a non-null collection
		CheckCollectionTypeIsNotNull(context, methodSymbol, returnType);
	}

	private static void CheckCollectionTypeIsNotNull(SymbolAnalysisContext context, IMethodSymbol methodSymbol, ITypeSymbol type)
	{
		if (type.AllInterfaces.Any(i => i.OriginalDefinition.ToDisplayString() == "System.Collections.IEnumerable"))
		{
			// Ensure the collection type is not null
			if (!type.NullableAnnotation.HasFlag(NullableAnnotation.NotAnnotated))
			{
				var diagnostic = Diagnostic.Create(_rule, methodSymbol.Locations[0], methodSymbol.Name, type.ToDisplayString());
				context.ReportDiagnostic(diagnostic);
			}
		}
	}
}