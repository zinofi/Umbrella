using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Rename;
using System.Collections.Immutable;
using System.Composition;

namespace Umbrella.Analyzers;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(UmbrellaAnalyzersCodeFixProvider)), Shared]
public class UmbrellaAnalyzersCodeFixProvider : CodeFixProvider
{
	/// <inheritdoc/>
	public sealed override ImmutableArray<string> FixableDiagnosticIds { get; } = [UmbrellaAnalyzersAnalyzer.DiagnosticId];

	/// <inheritdoc/>
	public sealed override FixAllProvider GetFixAllProvider()
	{
		// See https://github.com/dotnet/roslyn/blob/main/docs/analyzers/FixAllProvider.md for more information on Fix All Providers
		return WellKnownFixAllProviders.BatchFixer;
	}

	/// <inheritdoc/>
	public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
	{
		var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

		// TODO: Replace the following code with your own analysis, generating a CodeAction for each fix to suggest
		var diagnostic = context.Diagnostics.First();
		var diagnosticSpan = diagnostic.Location.SourceSpan;

		// Find the type declaration identified by the diagnostic.
		var declaration = (root?.FindToken(diagnosticSpan.Start).Parent?.AncestorsAndSelf().OfType<TypeDeclarationSyntax>().First()) ?? throw new InvalidOperationException("Unable to find the type declaration.");

		// Register a code action that will invoke the fix.
		context.RegisterCodeFix(
			CodeAction.Create(
				"",
				c => MakeUppercaseAsync(context.Document, declaration, c),
				""),
			diagnostic);
	}

	private static async Task<Solution> MakeUppercaseAsync(Document document, TypeDeclarationSyntax typeDecl, CancellationToken cancellationToken)
	{
		// Compute new uppercase name.
		var identifierToken = typeDecl.Identifier;
		string newName = identifierToken.Text.ToUpperInvariant();

		// Get the symbol representing the type to be renamed.
		var semanticModel = await document.GetSemanticModelAsync(cancellationToken);
		var typeSymbol = semanticModel.GetDeclaredSymbol(typeDecl, cancellationToken) ?? throw new InvalidOperationException("Unable to find the type symbol.");

		// Produce a new solution that has all references to that type renamed, including the declaration.
		var originalSolution = document.Project.Solution;
		var optionSet = originalSolution.Workspace.Options;
		var newSolution = await Renamer.RenameSymbolAsync(document.Project.Solution, typeSymbol, newName, optionSet, cancellationToken).ConfigureAwait(false);

		// Return the new solution with the now-uppercase type name.
		return newSolution;
	}
}