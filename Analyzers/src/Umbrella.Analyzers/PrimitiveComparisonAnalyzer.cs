﻿using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Umbrella.Analyzers;

/// <summary>
/// An analyzer that checks for primitive and enum comparisons using '==' or '!=' and enforces using pattern matching with 'is' or 'is not'.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class PrimitiveComparisonAnalyzer : DiagnosticAnalyzer
{
	/// <summary>
	/// The diagnostic ID for this analyzer.
	/// </summary>
	public const string DiagnosticId = "UA002";

    private static readonly DiagnosticDescriptor _rule = new(
        DiagnosticId,
        "Use pattern matching for primitive and enum comparisons",
        "Use 'is' or 'is not' instead of '==' or '!=' for primitive and enum comparisons",
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
        context.RegisterSyntaxNodeAction(AnalyzeBinaryExpression, SyntaxKind.EqualsExpression, SyntaxKind.NotEqualsExpression);
    }

    private static void AnalyzeBinaryExpression(SyntaxNodeAnalysisContext context)
    {
        var binaryExpression = (BinaryExpressionSyntax)context.Node;
        var leftType = context.SemanticModel.GetTypeInfo(binaryExpression.Left).Type;
        var rightType = context.SemanticModel.GetTypeInfo(binaryExpression.Right).Type;

        if (leftType != null && rightType != null &&
            (leftType.IsValueType || leftType.TypeKind == TypeKind.Enum) &&
            SymbolEqualityComparer.Default.Equals(leftType, rightType))
        {
            var diagnostic = Diagnostic.Create(_rule, binaryExpression.GetLocation());
            context.ReportDiagnostic(diagnostic);
        }
    }
}