using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = BitwiseToShortCircuitAnalyzer.Test.CSharpCodeFixVerifier<
	BitwiseToShortCircuitAnalyzer.B2SAnalyzer,
	BitwiseToShortCircuitAnalyzer.B2SCodeFixProvider>;

namespace BitwiseToShortCircuitAnalyzer.Test
{
	[TestClass]
	public class BitwiseAndPotentiallyInneffecientUnitTest
    {
		//No diagnostics expected to show up
		[TestMethod]
		public async Task TestMethod1()
		{
            var test = @"
class TypeName
{
    public bool Test(bool a, bool b)
    {
        return a & b;
    }
}";
            var expected = new DiagnosticResult(
                B2SAnalyzer.BooleanAndRuleId,
                DiagnosticSeverity.Warning);

            await VerifyCS.VerifyAnalyzerAsync(test, expected);
		}

		//Diagnostic and CodeFix both triggered and checked for
		[TestMethod]
		public async Task TestMethod2()
		{
			var test = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        class {|#0:TypeName|}
        {
            public bool And(bool a, bool b)
            {
                return a & b;
            }

            public int And(int a, int b)
            {
                return a & b;
            }
        }
    }";

			var fixtest = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        class TYPENAME
        {  
            public bool And(bool a, bool b)
            {
                return a && b;
            }

            public int And(int a, int b)
            {
                return a & b;
            }
        }
    }";

			var expected = VerifyCS
                .Diagnostic("BitwiseAndPotentiallyInneffecient")
                .WithLocation(0)
                .WithArguments("TypeName");
			await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest);
		}
	}
}