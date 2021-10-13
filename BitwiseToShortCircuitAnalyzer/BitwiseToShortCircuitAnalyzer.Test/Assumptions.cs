using Microsoft.VisualStudio.TestTools.UnitTesting;
using Open.Collections;

namespace BitwiseToShortCircuitAnalyzer.Test
{
	[TestClass]
	public class Assumptions
	{
		[TestMethod]
		public void Comparison1()
		{
			var options = new[] { true, false };
			var combinations = options.Combinations(6);

			foreach(var c in combinations)
			{
				var a = c[0] && c[1] || c[2] && c[3] || c[4] && c[5];
				var b = c[0] & c[1] | c[2] & c[3] | c[4] & c[5];
				Assert.AreEqual(a, b);
			}
		}

		[TestMethod]
		public void Comparison2()
		{
			var options = new[] { true, false };
			var combinations = options.Combinations(6);

			foreach (var c in combinations)
			{
				var a = c[0] && (c[1] || c[2]) && c[3] || c[4] && c[5];
				var b = c[0] & (c[1] | c[2]) & c[3] | c[4] & c[5];
				Assert.AreEqual(a, b);
			}
		}

		[TestMethod]
		public void Comparison3()
		{
			var options = new[] { true, false };
			var combinations = options.Combinations(6);

			foreach (var c in combinations)
			{
				var a = c[0] && (c[1] || c[2] && c[3]) || c[4] && c[5];
				var b = c[0] & (c[1] | c[2] & c[3]) | c[4] & c[5];
				Assert.AreEqual(a, b);
			}
		}
	}
}
