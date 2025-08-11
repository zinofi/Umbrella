using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Umbrella.Analyzers;

/// <summary>
/// An analyzer that checks if async methods with a CancellationToken parameter call `ThrowIfCancellationRequested` as the first line of the method body.
/// This ensures that cancellation requests are handled properly in asynchronous methods at the earliest opportunity.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class AsyncMethodThrowIfCancellationAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "UA004";

    private static readonly DiagnosticDescriptor _rule = new(
        DiagnosticId,
        "Async methods with CancellationToken should call ThrowIfCancellationRequested",
        "Async method '{0}' should call 'cancellationToken.ThrowIfCancellationRequested()' as the first line of the method body",
        "CodeStyle",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(_rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeMethod, SyntaxKind.MethodDeclaration);
    }

    private static void AnalyzeMethod(SyntaxNodeAnalysisContext context)
    {
        var methodDeclaration = (MethodDeclarationSyntax)context.Node;

        if (!methodDeclaration.Modifiers.Any(SyntaxKind.AsyncKeyword))
            return;

        var cancellationTokenParameter = methodDeclaration.ParameterList.Parameters
            .FirstOrDefault(p => p.Type is IdentifierNameSyntax type && type.Identifier.Text == "CancellationToken");

        if (cancellationTokenParameter == null)
            return;

        var body = methodDeclaration.Body;
        if (body == null || body.Statements.Count == 0)
            return;

        var firstStatement = body.Statements[0];
        if (firstStatement is ExpressionStatementSyntax expressionStatement &&
            expressionStatement.Expression is InvocationExpressionSyntax invocation &&
            invocation.Expression is MemberAccessExpressionSyntax memberAccess &&
            memberAccess.Name.Identifier.Text == "ThrowIfCancellationRequested")
        {
            return;
        }

        var diagnostic = Diagnostic.Create(_rule, methodDeclaration.Identifier.GetLocation(), methodDeclaration.Identifier.Text);
        context.ReportDiagnostic(diagnostic);
    }
}