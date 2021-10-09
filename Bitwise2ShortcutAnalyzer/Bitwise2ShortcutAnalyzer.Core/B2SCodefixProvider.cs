using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;

namespace Bitwise2ShortcutAnalyzer.Core
{
	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(B2SCodefixProvider))]
	[Shared]
	public class B2SCodefixProvider : CodeFixProvider
	{
		public override ImmutableArray<string> FixableDiagnosticIds
			=> ImmutableArray.Create(B2SAnalyzer.BooleanAndRuleId, B2SAnalyzer.BooleanOrRuleId);

		public override async Task RegisterCodeFixesAsync(CodeFixContext context)
		{
			foreach(var diagnostic in context.Diagnostics)
			{
				var document = context.Document;

				var root = await document.GetSyntaxRootAsync();

				var node = root.FindNode(diagnostic.Location.SourceSpan);

				if (!(node is BinaryExpressionSyntax bitwise))
					throw new Exception("Expected node to be of type InvocationExpressionSyntax");

				context.RegisterCodeFix(
					CodeAction.Create("Replace bitwise with shortcut.", async c =>
					{
						var op = bitwise.OperatorToken;
						var updatedRoot
							= op.IsKind(SyntaxKind.BitwiseAndExpression)
							? root.ReplaceToken(op, SyntaxFactory.Token(SyntaxKind.AmpersandAmpersandToken))
							: op.IsKind(SyntaxKind.BitwiseOrExpression)
                            ? root.ReplaceToken(op, SyntaxFactory.Token(SyntaxKind.BarBarToken))
                            : throw new Exception("Unexpected SyntaxKind.");

						return document.WithSyntaxRoot(updatedRoot);
					}), diagnostic);
			}
		}
	}
}
