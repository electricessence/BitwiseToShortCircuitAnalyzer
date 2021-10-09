using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using System.Threading.Tasks;

namespace Bitwise2ShortcutAnalyzer.Core
{
	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(B2SCodefixProvider))]
	public class B2SCodefixProvider : CodeFixProvider
	{
		public override ImmutableArray<string> FixableDiagnosticIds
			=> ImmutableArray.Create(B2SAnalyzer.BooleanAndRuleId, B2SAnalyzer.BooleanOrRuleId);

		public override async Task RegisterCodeFixesAsync(CodeFixContext context)
		{
			foreach(var diagnostic in context.Diagnostics)
			{
				var document = context.Document;

				context.RegisterCodeFix(
					CodeAction.Create("Replace bitwise with shortcut.", async e =>
					{
						return document;
					}), diagnostic);
			}
		}
	}
}
