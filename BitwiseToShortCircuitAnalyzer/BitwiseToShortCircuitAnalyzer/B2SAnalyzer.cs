using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;

namespace BitwiseToShortCircuitAnalyzer
{
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class B2SAnalyzer : DiagnosticAnalyzer
	{
		public const string BooleanAndRuleId = "BitwiseAndPotentiallyInneffecient";
		public const string BooleanOrRuleId = "BitwiseOrPotentiallyInneffecient";

		private static readonly LocalizableString BooleanAndTitle
			= new LocalizableResourceString(nameof(Resources.BooleanAndTitle), Resources.ResourceManager, typeof(Resources));
		private static readonly LocalizableString BooleanAndDescription
			= new LocalizableResourceString(nameof(Resources.BooleanAndDescription), Resources.ResourceManager, typeof(Resources));
		private static readonly LocalizableString BooleanOrTitle
			= new LocalizableResourceString(nameof(Resources.BooleanAndTitle), Resources.ResourceManager, typeof(Resources));
		private static readonly LocalizableString BooleanOrDescription
			= new LocalizableResourceString(nameof(Resources.BooleanAndDescription), Resources.ResourceManager, typeof(Resources));
		private static readonly LocalizableString MessageFormat
			= new LocalizableResourceString(nameof(Resources.MessageFormat), Resources.ResourceManager, typeof(Resources));
		private const string Category = "Bitwise";

		private static readonly DiagnosticDescriptor BooleanAndRule
			= new DiagnosticDescriptor(BooleanAndRuleId, BooleanAndTitle, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: BooleanAndDescription);
		private static readonly DiagnosticDescriptor BooleanOrRule
			= new DiagnosticDescriptor(BooleanOrRuleId, BooleanOrTitle, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: BooleanOrDescription);

		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
			=> ImmutableArray.Create(BooleanAndRule, BooleanOrRule);

		public override void Initialize(AnalysisContext context)
		{
			context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
			context.EnableConcurrentExecution();
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
					BooleanAndRule,
					node.GetLocation()));
		}

		private void AynalyzePipe(SyntaxNodeAnalysisContext context)
		{
			var node = (BinaryExpressionSyntax)context.Node;
			var result = GetBoolResult(context.SemanticModel, node);
			if (result == Result.None) return;

			context.ReportDiagnostic(
				Diagnostic.Create(
					BooleanOrRule,
					node.GetLocation()));
		}
	}
}