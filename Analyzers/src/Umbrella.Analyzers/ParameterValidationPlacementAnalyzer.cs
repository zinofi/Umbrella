using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Umbrella.Analyzers;

/// <summary>
/// Analyzer that enforces all parameter validation (Guard.* calls, Argument* Throw* helpers, or direct throws
/// of ArgumentException / ArgumentNullException / ArgumentOutOfRangeException) appear before the first
/// try...catch block in a method body and never inside any try block.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class ParameterValidationPlacementAnalyzer : DiagnosticAnalyzer
{
	public const string DiagnosticId = "UA009";
	public static readonly DiagnosticDescriptor Rule = new(
		DiagnosticId,
		"Parameter validation must appear before first try...catch block",
		"Parameter validation should occur before the first try...catch block in method '{0}'",
		"CodeStyle",
		DiagnosticSeverity.Error,
		isEnabledByDefault: true);
	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [Rule];
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
		var syntaxRef = methodSymbol.DeclaringSyntaxReferences.FirstOrDefault();
		if (syntaxRef is null)
			return;
		if (syntaxRef.GetSyntax(context.CancellationToken) is not Microsoft.CodeAnalysis.CSharp.Syntax.MethodDeclarationSyntax methodDecl)
			return;
		var body = methodDecl.Body;
		if (body is null)
			return;

		// Capture all try statements for later inside-try detection.
		var allTryStatements = body.DescendantNodes(static n => true).OfType<Microsoft.CodeAnalysis.CSharp.Syntax.TryStatementSyntax>().ToList();

		// Identify the first top-level try (appears directly in Statements collection)
		Microsoft.CodeAnalysis.CSharp.Syntax.TryStatementSyntax? firstTopLevelTry = null;
		foreach (var statement in body.Statements)
		{
			if (statement is Microsoft.CodeAnalysis.CSharp.Syntax.TryStatementSyntax ts)
			{
				firstTopLevelTry = ts;
				break;
			}
		}

		int firstTryStart = firstTopLevelTry?.SpanStart ?? int.MaxValue;

		// Iterate every descendant node (excluding local functions) and locate parameter validation nodes.
		foreach (var node in body.DescendantNodes(ShouldDescendInto))
		{
			if (!IsParameterValidationNode(node))
				continue;

			// If inside any try block, report.
			if (IsInsideTryBlock(node, allTryStatements))
			{
				Report(context, methodSymbol, node.GetLocation());
				return;
			}

			// If appears after the first top-level try, report.
			if (node.SpanStart > firstTryStart)
			{
				Report(context, methodSymbol, node.GetLocation());
				return;
			}
		}
	}

	private static bool ShouldDescendInto(SyntaxNode node)
	{
		if (node is Microsoft.CodeAnalysis.CSharp.Syntax.LocalFunctionStatementSyntax)
			return false;
		return true;
	}

	private static bool IsInsideTryBlock(SyntaxNode node, IEnumerable<Microsoft.CodeAnalysis.CSharp.Syntax.TryStatementSyntax> tryStatements)
	{
		foreach (var ts in tryStatements)
		{
			// Inside try block (not catch/finally) if the node span is within the Block span.
			var block = ts.Block;
			if (block is not null && node.SpanStart >= block.SpanStart && node.SpanStart < block.Span.End)
			{
				// Ensure not in a catch or finally by checking ancestor chain contains this try and the path is through Block.
				if (IsNodeWithinBlock(node, block))
					return true;
			}
		}

		return false;
	}

	private static bool IsNodeWithinBlock(SyntaxNode node, Microsoft.CodeAnalysis.CSharp.Syntax.BlockSyntax block)
	{
		SyntaxNode? current = node;
		
		while (current is not null)
		{
			if (current == block)
				return true;

			// Stop if we hit a catch/finally or another try.
			if (current is Microsoft.CodeAnalysis.CSharp.Syntax.CatchClauseSyntax or Microsoft.CodeAnalysis.CSharp.Syntax.FinallyClauseSyntax)
				return false;

			current = current.Parent;

		}

		return false;
	}

	private static bool IsParameterValidationNode(SyntaxNode node)
	{
		if (node is Microsoft.CodeAnalysis.CSharp.Syntax.InvocationExpressionSyntax invocation && invocation.Expression is Microsoft.CodeAnalysis.CSharp.Syntax.MemberAccessExpressionSyntax memberAccess)
		{
			var leftId = memberAccess.Expression as Microsoft.CodeAnalysis.CSharp.Syntax.IdentifierNameSyntax;
			var rightId = memberAccess.Name as Microsoft.CodeAnalysis.CSharp.Syntax.IdentifierNameSyntax;
			if (leftId is not null && rightId is not null)
			{
				string leftText = leftId.Identifier.Text;
				string rightText = rightId.Identifier.Text;
				if (leftText == "Guard")
					return true;

				if ((leftText == "ArgumentException" || leftText == "ArgumentNullException" || leftText == "ArgumentOutOfRangeException") && rightText.StartsWith("Throw", StringComparison.Ordinal))
					return true;
			}
		}

		if (node is Microsoft.CodeAnalysis.CSharp.Syntax.ThrowStatementSyntax throwStmt && throwStmt.Expression is Microsoft.CodeAnalysis.CSharp.Syntax.ObjectCreationExpressionSyntax objectCreation)
		{
			if (objectCreation.Type is Microsoft.CodeAnalysis.CSharp.Syntax.IdentifierNameSyntax typeId)
			{
				string name = typeId.Identifier.Text;
				if (name == "ArgumentException" || name == "ArgumentNullException" || name == "ArgumentOutOfRangeException")
					return true;

			}
			else if (objectCreation.Type is Microsoft.CodeAnalysis.CSharp.Syntax.QualifiedNameSyntax qn)
			{
				string right = qn.Right.Identifier.Text;
				if (right == "ArgumentException" || right == "ArgumentNullException" || right == "ArgumentOutOfRangeException")
					return true;
			}
		}

		return false;
	}

	private static void Report(SymbolAnalysisContext context, IMethodSymbol methodSymbol, Location location)
	{
		var diagnostic = Diagnostic.Create(Rule, location, methodSymbol.Name);
		context.ReportDiagnostic(diagnostic);
	}
}
