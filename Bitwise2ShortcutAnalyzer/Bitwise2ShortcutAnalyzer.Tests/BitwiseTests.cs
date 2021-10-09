using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.MSBuild;
using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Bitwise2ShortcutAnalyzer.Tests
{
	public class BitwiseTests
	{
		const string RelativePath
			= @"..\..\..\..\Bitwise2ShortcutAnalyzer.Sandbox";
		const string FileName
			= "Bitwise2ShortcutAnalyzer.Sandbox.sln";

		static Task<Solution> GetSolution(MSBuildWorkspace workspace)
		{
			Assert.True(Directory.Exists(RelativePath), "Solution directory not found.");
			var filePath = Path.Combine(RelativePath, FileName);
			Assert.True(File.Exists(filePath), "Solution file not found.");
			return workspace.OpenSolutionAsync(filePath);
		}

		const string SampleCode = @"
public static class Program
{
	public static void Main() { }

	public static bool BitwiseBoolAnd(bool a, bool b) => a & b;
	public static bool BitwiseBoolOr(bool a, bool b) => a | b;
	public static int BitwiseIntAnd(int a, int b) => a & b;
	public static int BitwiseIntOr(int a, int b) => a | b;
}";

		static async ValueTask<CompilationWithAnalyzers> GetCompilationSampleCode(AdhocWorkspace workspace)
		{
			var projectId = ProjectId.CreateNewId();
			var solution = workspace.CurrentSolution
				.AddProject(projectId, "BitwiseTest", "BitwiseTest", LanguageNames.CSharp)
				.AddDocument(DocumentId.CreateNewId(projectId), "Program.cs", SampleCode);

			var project = solution.GetProject(projectId);
			var compilation = await project.GetCompilationAsync();
			return compilation
				.AddReferences(MetadataReference.CreateFromFile(typeof(object).Assembly.Location))
				.WithAnalyzers(ImmutableArray.Create<DiagnosticAnalyzer>(
					new B2SAnalyzer()));
		}

		[Fact]
		public async Task EnsureBitwiseDiagnostics()
		{
			using var workspace = new AdhocWorkspace();
			var compilation = await GetCompilationSampleCode(workspace);

			var diagnostics = await compilation.GetAllDiagnosticsAsync();

			Assert.True(diagnostics.Any(d => d.Id == B2SAnalyzer.BooleanOrRuleId));
			Assert.True(diagnostics.Any(d => d.Id == B2SAnalyzer.BooleanAndRuleId));

			// We should only get 2 as we only care about boolean comparisons.
			Assert.Equal(2, diagnostics.Length);
		}
	}
}

