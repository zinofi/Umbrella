using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Umbrella.Analyzers;

/// <summary>
/// An analyzer that checks for null checks using '==' or '!=' and suggests using pattern matching with 'is null' or 'is not null'.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class NullCheckAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "UA001";

    private static readonly DiagnosticDescriptor _rule = new(
        DiagnosticId,
        "Use pattern matching for null checks",
        "Use 'is null' or 'is not null' instead of '==' or '!='",
        "CodeStyle",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(_rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeBinaryExpression, SyntaxKind.EqualsExpression, SyntaxKind.NotEqualsExpression);
    }

    private static void AnalyzeBinaryExpression(SyntaxNodeAnalysisContext context)
    {
        var binaryExpression = (BinaryExpressionSyntax)context.Node;

        if (binaryExpression.Left.IsKind(SyntaxKind.NullLiteralExpression) ||
            binaryExpression.Right.IsKind(SyntaxKind.NullLiteralExpression))
        {
            var diagnostic = Diagnostic.Create(_rule, binaryExpression.GetLocation());
            context.ReportDiagnostic(diagnostic);
        }
    }
}