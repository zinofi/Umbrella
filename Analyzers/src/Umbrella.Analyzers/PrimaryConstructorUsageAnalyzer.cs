using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Umbrella.Analyzers;

/// <summary>
/// Analyzer that forbids use of primary constructors on non-record classes and structs.
/// (e.g. class C(int x) { } or struct S(int x) { }) .
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class PrimaryConstructorUsageAnalyzer : DiagnosticAnalyzer
{
	/// <summary>
	/// Represents the unique identifier for the analyzer diagnostic UA010.
	/// </summary>
	public const string DiagnosticId = "UA010";

	/// <summary>
	/// Provides a diagnostic rule that enforces the prohibition of primary constructors in types.
	/// </summary>
	/// <remarks>This rule triggers an error diagnostic when a type is declared with a primary constructor. It is
	/// intended for use in code style analyzers to ensure compliance with coding standards that disallow primary
	/// constructors.</remarks>
	public static readonly DiagnosticDescriptor Rule = new(
		DiagnosticId,
		"Primary constructors are not allowed",
		"Type '{0}' must not use a primary constructor",
		"CodeStyle",
		DiagnosticSeverity.Error,
		isEnabledByDefault: true);

	/// <inheritdoc/>
	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [Rule];

	/// <inheritdoc/>
	public override void Initialize(AnalysisContext context)
	{
		if (context is null)
			throw new ArgumentNullException(nameof(context));

		context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
		context.EnableConcurrentExecution();

		context.RegisterSyntaxNodeAction(AnalyzeClass, SyntaxKind.ClassDeclaration);
		context.RegisterSyntaxNodeAction(AnalyzeStruct, SyntaxKind.StructDeclaration);
	}

	private static void AnalyzeClass(SyntaxNodeAnalysisContext ctx)
	{
		if (ctx.Node is not ClassDeclarationSyntax cds)
			return;

		// Skip record class declarations (they surface as RecordDeclarationSyntax, not ClassDeclarationSyntax).
		if (cds.ParameterList is null)
			return;

		Report(ctx, cds.Identifier);
	}

	private static void AnalyzeStruct(SyntaxNodeAnalysisContext ctx)
	{
		if (ctx.Node is not StructDeclarationSyntax sds)
			return;

		if (sds.ParameterList is null)
			return;

		Report(ctx, sds.Identifier);
	}

	private static void Report(SyntaxNodeAnalysisContext ctx, SyntaxToken identifier)
	{
		var diagnostic = Diagnostic.Create(Rule, identifier.GetLocation(), identifier.Text);
		ctx.ReportDiagnostic(diagnostic);
	}
}