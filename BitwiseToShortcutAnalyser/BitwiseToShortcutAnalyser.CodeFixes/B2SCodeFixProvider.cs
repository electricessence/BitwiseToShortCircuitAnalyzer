using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Rename;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BitwiseToShortcutAnalyzer
{
	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(B2SCodeFixProvider)), Shared]
	public class B2SCodeFixProvider : CodeFixProvider
	{
		public sealed override ImmutableArray<string> FixableDiagnosticIds
			=> ImmutableArray.Create(B2SAnalyzer.BooleanAndRuleId, B2SAnalyzer.BooleanOrRuleId);

		public sealed override FixAllProvider GetFixAllProvider() =>
			// See https://github.com/dotnet/roslyn/blob/main/docs/analyzers/FixAllProvider.md for more information on Fix All Providers
			WellKnownFixAllProviders.BatchFixer;

		public override async Task RegisterCodeFixesAsync(CodeFixContext context)
		{
			var document = context.Document;
			var root = await document.GetSyntaxRootAsync();
			foreach (var diagnostic in context.Diagnostics)
			{
				var node = root.FindNode(diagnostic.Location.SourceSpan);
				if (node is BinaryExpressionSyntax bitwise)	context.RegisterCodeFix(
					CodeAction.Create(CodeFixResources.CodeFixTitle, async c =>
					{
						await Task.Yield();
						var op = bitwise.OperatorToken;
						var updatedRoot
							= op.IsKind(SyntaxKind.AmpersandToken)
							? root.ReplaceToken(op, SyntaxFactory.Token(SyntaxKind.AmpersandAmpersandToken))
							: op.IsKind(SyntaxKind.BarToken)
							? root.ReplaceToken(op, SyntaxFactory.Token(SyntaxKind.BarBarToken))
							: throw new Exception("Unexpected SyntaxKind.");

						return document.WithSyntaxRoot(updatedRoot);
					}), diagnostic);
			}
		}
	}
}