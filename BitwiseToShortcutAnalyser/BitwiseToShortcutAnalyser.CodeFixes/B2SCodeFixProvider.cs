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

namespace BitwiseToShortcutAnalyser
{
	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(B2SCodeFixProvider)), Shared]
	public class B2SCodeFixProvider : CodeFixProvider
	{
		public sealed override ImmutableArray<string> FixableDiagnosticIds
			=> ImmutableArray.Create(B2SAnalyser.BooleanAndRuleId, B2SAnalyser.BooleanOrRuleId);

		public sealed override FixAllProvider GetFixAllProvider() =>
			// See https://github.com/dotnet/roslyn/blob/main/docs/analyzers/FixAllProvider.md for more information on Fix All Providers
			WellKnownFixAllProviders.BatchFixer;

		public override async Task RegisterCodeFixesAsync(CodeFixContext context)
		{
			foreach (var diagnostic in context.Diagnostics)
			{
				var document = context.Document;

				var root = await document.GetSyntaxRootAsync();

				var node = root.FindNode(diagnostic.Location.SourceSpan);

				if (!(node is BinaryExpressionSyntax bitwise))
					throw new Exception("Expected node to be of type InvocationExpressionSyntax");

				context.RegisterCodeFix(
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