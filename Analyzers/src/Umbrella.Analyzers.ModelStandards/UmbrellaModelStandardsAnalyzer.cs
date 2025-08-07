using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Umbrella.Analyzers.ModelStandards;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class UmbrellaModelStandardsAnalyzer : DiagnosticAnalyzer
{
	// Diagnostics
	public static readonly DiagnosticDescriptor ModelMustBeRecordRule = new(
		id: "UMS001",
		title: "Model types must be records",
		messageFormat: "The model type '{0}' should be defined as a record",
		category: "UmbrellaModelStandards",
		defaultSeverity: DiagnosticSeverity.Warning,
		isEnabledByDefault: true,
		description: "Per Umbrella standards, model types should be defined as records for better immutability guarantees.");

	public static readonly DiagnosticDescriptor PropertiesMustBeRequiredRule = new(
		id: "UMS002",
		title: "Model properties must use the required keyword",
		messageFormat: "Property '{0}' in model type '{1}' should use the 'required' keyword",
		category: "UmbrellaModelStandards",
		defaultSeverity: DiagnosticSeverity.Warning,
		isEnabledByDefault: true,
		description: "Per Umbrella standards, properties in model types should use the 'required' keyword.");

	public static readonly DiagnosticDescriptor PropertiesMustBeGetterInitOnlyRule = new(
		id: "UMS003",
		title: "Model properties must have getter and be init-only",
		messageFormat: "Property '{0}' in model type '{1}' should have a getter and be init-only",
		category: "UmbrellaModelStandards",
		defaultSeverity: DiagnosticSeverity.Warning,
		isEnabledByDefault: true,
		description: "Per Umbrella standards, properties in model types should have a getter and be init-only.");

	public static readonly DiagnosticDescriptor CollectionsMustBeReadOnlyRule = new(
		id: "UMS004",
		title: "Collection properties must use IReadOnlyCollection<T>",
		messageFormat: "Collection property '{0}' in model type '{1}' should be of type IReadOnlyCollection<T>",
		category: "UmbrellaModelStandards",
		defaultSeverity: DiagnosticSeverity.Warning,
		isEnabledByDefault: true,
		description: "Per Umbrella standards, collection properties in model types should use IReadOnlyCollection<T> for better immutability.");

	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
		ModelMustBeRecordRule,
		PropertiesMustBeRequiredRule,
		PropertiesMustBeGetterInitOnlyRule,
		CollectionsMustBeReadOnlyRule);

	public override void Initialize(AnalysisContext context)
	{
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