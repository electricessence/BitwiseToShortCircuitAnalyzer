using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Immutable;

namespace Bitwise2ShortcutAnalyzer
{
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class B2SAnalyzer : DiagnosticAnalyzer
	{
		public const string AndRuleId = "BitwiseAndPotentiallyInneffecient";
		public const string OrRuleId = "BitwiseOrPotentiallyInneffecient";

		private static readonly DiagnosticDescriptor AndRule
			= new DiagnosticDescriptor(
				AndRuleId,
				"Potentially inneficient use of bitwise (&)",
				"This bitwise (&) is potentially causing unnecessary subsequent evaluation",
				"Bitwise",
				DiagnosticSeverity.Warning,
				isEnabledByDefault: true);

		private static readonly DiagnosticDescriptor OrRule
			= new DiagnosticDescriptor(
				OrRuleId,
				"Potentially inneficient use of bitwise (|)",
				"This bitwise (|) is potentially causing unnecessary subsequent evaluation",
				"Bitwise",
				DiagnosticSeverity.Warning,
				isEnabledByDefault: true);

		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
			=> ImmutableArray.Create(AndRule, OrRule);

		public override void Initialize(AnalysisContext context)
		{
			context.EnableConcurrentExecution();
			context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
			context.RegisterSyntaxNodeAction(AynalyzeAmpersand, SyntaxKind.BitwiseAndExpression);
			context.RegisterSyntaxNodeAction(AynalyzePipe, SyntaxKind.BitwiseOrExpression);
		}

		private static int BoolCount(SemanticModel model, BinaryExpressionSyntax node)
		{
			var left = model.GetTypeInfo(node.Left).Type.SpecialType;
			var right = model.GetTypeInfo(node.Right).Type.SpecialType;
			int count = 0;
			if (left == SpecialType.System_Boolean) ++count;
			if (right == SpecialType.System_Boolean) ++count;
			return count;
		}

		private void AynalyzeAmpersand(SyntaxNodeAnalysisContext context)
		{
			var node = (BinaryExpressionSyntax)context.Node;
			if (BoolCount(context.SemanticModel, node) == 0) return;

			context.ReportDiagnostic(
				Diagnostic.Create(AndRule,
				node.GetLocation()));
		}

		private void AynalyzePipe(SyntaxNodeAnalysisContext context)
		{
			var node = (BinaryExpressionSyntax)context.Node;
			if (BoolCount(context.SemanticModel, node) == 0) return;

			context.ReportDiagnostic(
				Diagnostic.Create(OrRule,
				node.GetLocation()));
		}
	}
}
