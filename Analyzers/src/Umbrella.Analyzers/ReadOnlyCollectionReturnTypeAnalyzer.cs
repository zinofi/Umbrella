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
		bool isTuple = false;

		// Check for Task<T> or ValueTask<T>
		if (returnType is INamedTypeSymbol namedType && namedType.IsGenericType &&
			(namedType.Name == "Task" || namedType.Name == "ValueTask"))
		{
			lstReturnTypeToCheck.Add(namedType.TypeArguments[0]);
		}
		else if (returnType is INamedTypeSymbol tupleType && tupleType.IsTupleType)
		{
			lstReturnTypeToCheck.AddRange(tupleType.TupleElements.Select(e => e.Type));
			isTuple = true;
		}
		else if (returnType is INamedTypeSymbol tupleSymbol && tupleSymbol.OriginalDefinition.ToDisplayString().StartsWith("System.Tuple", StringComparison.Ordinal))
		{
			lstReturnTypeToCheck.AddRange(tupleSymbol.TypeArguments);
			isTuple = true;
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

[SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "<Pending>")]
public static class ITypeSymbolExtensions
{
	extension(ITypeSymbol? type)
	{
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

//public static class TypeSymbolExtensions
//{
//	public static ImmutableArray<INamedTypeSymbol> GetAllInterfacesRobust(this ITypeSymbol type)
//	{
//		// Normal (constructed, non-error) path first
//		if (type is INamedTypeSymbol named)
//		{
//			if (named.IsUnboundGenericType)
//			{
//				// Fall back to the generic definition
//				var def = named.ConstructedFrom; // or named.OriginalDefinition
//												 // def.AllInterfaces may STILL be empty in unbound form: use Interfaces + closure manually
//				return CollectAllInterfaces(def);
//			}

//			if (type.TypeKind != TypeKind.Error && !named.IsUnboundGenericType)
//			{
//				return named.AllInterfaces;
//			}
//		}

//		// Fallback: manual accumulation (covers error / unusual cases)
//		return CollectAllInterfaces(type);

//		static ImmutableArray<INamedTypeSymbol> CollectAllInterfaces(ITypeSymbol t)
//		{
//			var builder = ImmutableArray.CreateBuilder<INamedTypeSymbol>();
//			var seen = new HashSet<ITypeSymbol>(SymbolEqualityComparer.Default);

//			void Recurse(ITypeSymbol current)
//			{
//				if (!seen.Add(current))
//					return;

//				if (current is INamedTypeSymbol nts)
//				{
//					foreach (var i in nts.Interfaces)
//					{
//						if (i is INamedTypeSymbol ni)
//							builder.Add(ni);
						
//						Recurse(i);
//					}

//					// Climb base type chain
//					if (nts.BaseType is { } bt)
//						Recurse(bt);
//				}
//			}

//			Recurse(t);
//			return builder.ToImmutable();
//		}
//	}

//	public static bool ImplementsIEnumerable(this ITypeSymbol? type, bool includeString = false)
//	{
//		if (type == null)
//			return false;

//		// Handle string early
//		if (!includeString && type.SpecialType == SpecialType.System_String)
//			return false;

//		// Arrays are enumerable
//		if (type.TypeKind == TypeKind.Array)
//			return true;

//		// Fast path: try AllInterfaces if available
//		if (TryMatchAllInterfaces(type, out bool match))
//			return match;

//		// Fallback: robust interface collection
//		foreach (var iface in type.GetAllInterfacesRobust())
//		{
//			if (IsEnumerableInterface(iface))
//				return true;
//		}

//		return false;

//		bool TryMatchAllInterfaces(ITypeSymbol t, out bool result)
//		{
//			result = false;
//			if (t is INamedTypeSymbol nts && !nts.IsUnboundGenericType && t.TypeKind != TypeKind.Error)
//			{
//				foreach (var iface in nts.AllInterfaces)
//				{
//					if (IsEnumerableInterface(iface))
//					{
//						result = true;
//						return true;
//					}
//				}

//				return true; // looked, not found
//			}

//			return false; // cannot rely on AllInterfaces
//		}

//		bool IsEnumerableInterface(ITypeSymbol i)
//		{
//			// Non-generic IEnumerable
//			if (i.SpecialType == SpecialType.System_Collections_IEnumerable)
//				return true;

//			// Generic IEnumerable<T>
//			if (i is INamedTypeSymbol ni &&
//				ni.OriginalDefinition.SpecialType == SpecialType.System_Collections_Generic_IEnumerable_T)
//			{
//				return true;
//			}

//			return false;
//		}
//	}
//}