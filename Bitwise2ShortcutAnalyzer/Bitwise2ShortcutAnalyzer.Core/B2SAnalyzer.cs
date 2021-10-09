using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace Bitwise2ShortcutAnalyzer
{
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class B2SAnalyzer : DiagnosticAnalyzer
	{
		public const string BooleanAndRuleId = "BitwiseAndPotentiallyInneffecient";
		public const string BooleanOrRuleId = "BitwiseOrPotentiallyInneffecient";
		public const string MixedAndRuleId = "MixedBooleanIntBitwiseAnd";
		public const string MixedOrRuleId = "MixedBooleanIntBitwiseOr";

		private static readonly DiagnosticDescriptor BooleanAndRule
			= new DiagnosticDescriptor(
				BooleanAndRuleId,
				"Potentially inneficient use of bitwise (&)",
				"This bitwise (&) is potentially causing unnecessary subsequent evaluation",
				"Bitwise",
				DiagnosticSeverity.Warning,
				isEnabledByDefault: true);

		private static readonly DiagnosticDescriptor BooleanOrRule
			= new DiagnosticDescriptor(
				BooleanOrRuleId,
				"Potentially inneficient use of bitwise (|)",
				"This bitwise (|) is potentially causing unnecessary subsequent evaluation",
				"Bitwise",
				DiagnosticSeverity.Warning,
				isEnabledByDefault: true);

		private static readonly DiagnosticDescriptor MixedAndRule
			= new DiagnosticDescriptor(
				MixedAndRuleId,
				"Mixing booleans and integers with bitwise (&)",
				"This bitwise (&) combining a boolean and an integer and can potentially produce unexpected results or negatively affect performance",
				"Bitwise",
				DiagnosticSeverity.Warning,
				isEnabledByDefault: true);

		private static readonly DiagnosticDescriptor MixedOrRule
			= new DiagnosticDescriptor(
				MixedOrRuleId,
				"Mixing booleans and integers with bitwise (|)",
				"This bitwise (|) combining a boolean and an integer and can potentially produce unexpected results or negatively affect performance",
				"Bitwise",
				DiagnosticSeverity.Warning,
				isEnabledByDefault: true);

		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
			=> ImmutableArray.Create(BooleanAndRule, BooleanOrRule);

		public override void Initialize(AnalysisContext context)
		{
			context.EnableConcurrentExecution();
			context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
			context.RegisterSyntaxNodeAction(AynalyzeAmpersand, SyntaxKind.BitwiseAndExpression);
			context.RegisterSyntaxNodeAction(AynalyzePipe, SyntaxKind.BitwiseOrExpression);
		}

		enum Result
		{
			None,
			Left,
			Right,
			Both
		}

		private static Result GetBoolResult(SemanticModel model, BinaryExpressionSyntax node)
		{
			var left = model.GetTypeInfo(node.Left).Type.SpecialType == SpecialType.System_Boolean;
			var right = model.GetTypeInfo(node.Right).Type.SpecialType == SpecialType.System_Boolean;
			return left
				? right ? Result.Both : Result.Left
				: right ? Result.Right : Result.None;
		}

		private void AynalyzeAmpersand(SyntaxNodeAnalysisContext context)
		{
			var node = (BinaryExpressionSyntax)context.Node;
			var result = GetBoolResult(context.SemanticModel, node);
			if (result == Result.None) return;

			context.ReportDiagnostic(
				Diagnostic.Create(
					result == Result.Both ? BooleanAndRule : MixedAndRule,
					node.GetLocation()));
		}

		private void AynalyzePipe(SyntaxNodeAnalysisContext context)
		{
			var node = (BinaryExpressionSyntax)context.Node;
			var result = GetBoolResult(context.SemanticModel, node);
			if (result == Result.None) return;

			context.ReportDiagnostic(
				Diagnostic.Create(
					result == Result.Both ? BooleanOrRule : MixedOrRule,
					node.GetLocation()));
		}
	}
}
