using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Umbrella.Analyzers.ModelStandards;

/// <summary>
/// Roslyn analyzer that enforces coding standards for model classes and view models
/// following the Umbrella framework conventions for immutability and type safety.
/// </summary>
/// <remarks>
/// <para>
/// This analyzer enforces the following rules:
/// </para>
/// <list type="bullet">
/// <item><description>UMS001: Model types must be records for better immutability guarantees</description></item>
/// <item><description>UMS002: Model properties must use the 'required' keyword for initialization safety</description></item>
/// <item><description>UMS003: Model properties must have getter and be init-only to prevent mutation</description></item>
/// <item><description>UMS004: Collection properties must use IReadOnlyCollection&lt;T&gt; for immutability</description></item>
/// </list>
/// <para>
/// The analyzer targets types with names ending in: Model, ModelBase, ViewModel, or ViewModelBase.
/// </para>
/// <para>
/// Opt-out attributes are available to bypass specific rules when justified.
/// </para>
/// </remarks>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class UmbrellaModelStandardsAnalyzer : DiagnosticAnalyzer
{
	/// <summary>
	/// Diagnostic rule that requires model types to be defined as records instead of classes.
	/// </summary>
	/// <remarks>
	/// Records provide better immutability guarantees and value-based equality semantics
	/// which are essential for model types in the Umbrella framework.
	/// </remarks>
	public static readonly DiagnosticDescriptor ModelMustBeRecordRule = new(
		id: "UMS001",
		title: "Model types must be records",
		messageFormat: "The model type '{0}' should be defined as a record",
		category: "UmbrellaModelStandards",
		defaultSeverity: DiagnosticSeverity.Error,
		isEnabledByDefault: true,
		description: "Per Umbrella standards, model types should be defined as records for better immutability guarantees.");

	/// <summary>
	/// Diagnostic rule that requires model properties to use the 'required' keyword.
	/// </summary>
	/// <remarks>
	/// The 'required' keyword ensures that properties are initialized at object creation time,
	/// preventing null reference exceptions and improving type safety.
	/// </remarks>
	public static readonly DiagnosticDescriptor PropertiesMustBeRequiredRule = new(
		id: "UMS002",
		title: "Model properties must use the required keyword",
		messageFormat: "Property '{0}' in model type '{1}' should use the 'required' keyword",
		category: "UmbrellaModelStandards",
		defaultSeverity: DiagnosticSeverity.Error,
		isEnabledByDefault: true,
		description: "Per Umbrella standards, properties in model types should use the 'required' keyword.");

	/// <summary>
	/// Diagnostic rule that requires model properties to have a getter and be init-only.
	/// </summary>
	/// <remarks>
	/// Properties should be readable (getter) and only settable during initialization (init)
	/// to maintain immutability after object creation.
	/// </remarks>
	public static readonly DiagnosticDescriptor PropertiesMustBeGetterInitOnlyRule = new(
		id: "UMS003",
		title: "Model properties must have getter and be init-only",
		messageFormat: "Property '{0}' in model type '{1}' should have a getter and be init-only",
		category: "UmbrellaModelStandards",
		defaultSeverity: DiagnosticSeverity.Error,
		isEnabledByDefault: true,
		description: "Per Umbrella standards, properties in model types should have a getter and be init-only.");

	/// <summary>
	/// Diagnostic rule that requires collection properties to use IReadOnlyCollection&lt;T&gt;.
	/// </summary>
	/// <remarks>
	/// Using IReadOnlyCollection&lt;T&gt; prevents external code from modifying the collection
	/// contents, maintaining immutability and preventing unintended side effects.
	/// </remarks>
	public static readonly DiagnosticDescriptor CollectionsMustBeReadOnlyRule = new(
		id: "UMS004",
		title: "Collection properties must use IReadOnlyCollection<T>",
		messageFormat: "Collection property '{0}' in model type '{1}' should be of type IReadOnlyCollection<T>",
		category: "UmbrellaModelStandards",
		defaultSeverity: DiagnosticSeverity.Error,
		isEnabledByDefault: true,
		description: "Per Umbrella standards, collection properties in model types should use IReadOnlyCollection<T> for better immutability.");

	/// <inheritdoc />
	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
		ModelMustBeRecordRule,
		PropertiesMustBeRequiredRule,
		PropertiesMustBeGetterInitOnlyRule,
		CollectionsMustBeReadOnlyRule);

	/// <inheritdoc />
	public override void Initialize(AnalysisContext context)
	{
		if (context is null)
			throw new ArgumentNullException(nameof(context));

		context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
		context.EnableConcurrentExecution();

		// Register for syntax node analysis
		context.RegisterSyntaxNodeAction(AnalyzeTypeDeclaration, SyntaxKind.ClassDeclaration, SyntaxKind.RecordDeclaration);
		context.RegisterSyntaxNodeAction(AnalyzePropertyDeclaration, SyntaxKind.PropertyDeclaration);
	}

	private void AnalyzeTypeDeclaration(SyntaxNodeAnalysisContext context)
	{
		// Get type declaration (class or record)
		if (context.Node is not TypeDeclarationSyntax typeDecl)
			return;

		// Check if it's a model type based on naming convention
		if (!IsModelType(typeDecl.Identifier.Text))
			return;

		// Check if the type has an opt-out attribute
		if (HasOptOutAttribute(typeDecl, context.SemanticModel, "UmbrellaExcludeFromModelStandardsAttribute"))
			return;

		// Check if the type is a record
		if (typeDecl is not RecordDeclarationSyntax)
		{
			var diagnostic = Diagnostic.Create(ModelMustBeRecordRule, typeDecl.Identifier.GetLocation(), typeDecl.Identifier.Text);
			context.ReportDiagnostic(diagnostic);
		}
	}

	private void AnalyzePropertyDeclaration(SyntaxNodeAnalysisContext context)
	{
		var propertyDecl = (PropertyDeclarationSyntax)context.Node;

		// Find the containing type
		var typeDecl = propertyDecl.FirstAncestorOrSelf<TypeDeclarationSyntax>();
		if (typeDecl == null || !IsModelType(typeDecl.Identifier.Text))
			return;

		// Check if the containing type has an opt-out attribute
		if (HasOptOutAttribute(typeDecl, context.SemanticModel, "UmbrellaExcludeFromModelStandardsAttribute"))
			return;

		// Check for 'required' keyword (using text-based approach since RequiredKeyword may not be available)
		if (!HasRequiredModifier(propertyDecl) &&
			!HasOptOutAttribute(propertyDecl, context.SemanticModel, "UmbrellaAllowOptionalPropertyAttribute"))
		{
			var diagnostic = Diagnostic.Create(PropertiesMustBeRequiredRule, propertyDecl.Identifier.GetLocation(),
				propertyDecl.Identifier.Text, typeDecl.Identifier.Text);
			context.ReportDiagnostic(diagnostic);
		}

		// Check for getter and init-only
		if ((propertyDecl.AccessorList == null ||
			!propertyDecl.AccessorList.Accessors.Any(a => a.Kind() == SyntaxKind.GetAccessorDeclaration)) ||
			(propertyDecl.AccessorList != null &&
			propertyDecl.AccessorList.Accessors.Any(a => a.Kind() == SyntaxKind.SetAccessorDeclaration &&
			!HasInitModifier(a))) &&
			!HasOptOutAttribute(propertyDecl, context.SemanticModel, "UmbrellaAllowMutablePropertyAttribute"))
		{
			var diagnostic = Diagnostic.Create(PropertiesMustBeGetterInitOnlyRule, propertyDecl.Identifier.GetLocation(),
				propertyDecl.Identifier.Text, typeDecl.Identifier.Text);
			context.ReportDiagnostic(diagnostic);
		}

		// Check if the property is a collection type but not IReadOnlyCollection<T>
		var propertyType = context.SemanticModel.GetTypeInfo(propertyDecl.Type).Type;
		if (propertyType != null && IsCollectionType(propertyType) && !IsReadOnlyCollectionType(propertyType) &&
			!HasOptOutAttribute(propertyDecl, context.SemanticModel, "UmbrellaAllowMutableCollectionAttribute"))
		{
			var diagnostic = Diagnostic.Create(CollectionsMustBeReadOnlyRule, propertyDecl.Identifier.GetLocation(),
				propertyDecl.Identifier.Text, typeDecl.Identifier.Text);
			context.ReportDiagnostic(diagnostic);
		}
	}

	private static bool IsModelType(string typeName)
	{
		return typeName.EndsWith("Model", StringComparison.Ordinal) ||
			   typeName.EndsWith("ModelBase", StringComparison.Ordinal) ||
			   typeName.EndsWith("ViewModel", StringComparison.Ordinal) ||
			   typeName.EndsWith("ViewModelBase", StringComparison.Ordinal);
	}

	private static bool HasRequiredModifier(PropertyDeclarationSyntax property)
	{
		// Check for required keyword in modifiers text (more compatible approach)
		return property.Modifiers.Any(m => m.Text == "required");
	}

	private static bool HasInitModifier(AccessorDeclarationSyntax accessor)
	{
		// Check for init keyword in modifiers text
		return accessor.Modifiers.Any(m => m.Text == "init");
	}

	private static bool HasOptOutAttribute(SyntaxNode node, SemanticModel semanticModel, params string[] attributeNames)
	{
		if (node is MemberDeclarationSyntax memberDecl)
		{
			if (memberDecl.AttributeLists.Count == 0)
				return false;

			var symbol = semanticModel.GetDeclaredSymbol(memberDecl);
			if (symbol == null)
				return false;

			foreach (var attribute in symbol.GetAttributes())
			{
				var attributeClass = attribute.AttributeClass;
				if (attributeClass != null && attributeNames.Contains(attributeClass.Name))
					return true;
			}
		}

		return false;
	}

	private static bool IsCollectionType(ITypeSymbol type)
	{
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

	private static bool IsReadOnlyCollectionType(ITypeSymbol type)
	{
		// Check if the type is IReadOnlyCollection<T> or implements it
		if (type.OriginalDefinition.ToDisplayString() == "System.Collections.Generic.IReadOnlyCollection<T>")
			return true;

		foreach (var interfaceType in type.AllInterfaces)
		{
			if (interfaceType.OriginalDefinition.ToDisplayString() == "System.Collections.Generic.IReadOnlyCollection<T>")
				return true;
		}

		return false;
	}
}