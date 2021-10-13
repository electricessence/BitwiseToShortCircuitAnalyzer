using Microsoft.VisualStudio.TestTools.UnitTesting;
using Open.Collections;
using System.Collections.Generic;

namespace BitwiseToShortCircuitAnalyzer.Test
{
	[TestClass]
	public class Assumptions
	{

		// Produces all possible combinations of true/false
		// in an array of length 6.
		static readonly IEnumerable<bool[]> _combinations
			= new[] { false, true }.Combinations(6);

		[TestMethod]
		public void Comparison1()
		{
			foreach(var c in _combinations)
			{
				var a = c[0] && c[1] || c[2] && c[3] || c[4] && c[5];
				var b = c[0] & c[1] | c[2] & c[3] | c[4] & c[5];
				Assert.AreEqual(a, b);
			}
		}

		[TestMethod]
		public void Comparison2()
		{
			foreach (var c in _combinations)
			{
				var a = c[0] && (c[1] || c[2]) && c[3] || c[4] && c[5];
				var b = c[0] & (c[1] | c[2]) & c[3] | c[4] & c[5];
				Assert.AreEqual(a, b);
			}
		}

		[TestMethod]
		public void Comparison3()
		{
			foreach (var c in _combinations)
			{
				var a = c[0] && (c[1] || c[2] && c[3]) || c[4] && c[5];
				var b = c[0] & (c[1] | c[2] & c[3]) | c[4] & c[5];
				Assert.AreEqual(a, b);
			}
		}

		[TestMethod]
		public void Comparison4()
		{
			foreach (var c in _combinations)
			{
				var a = c[0] && c[1] && c[2] || c[3] && c[4] && c[5];
				var b = c[0] & c[1] & c[2] | c[3] & c[4] & c[5];
				Assert.AreEqual(a, b);
			}
		}

		[TestMethod]
		public void Comparison5()
		{
			foreach (var c in _combinations)
			{
				var a = c[0] && c[1] | c[2] && c[3] | c[4] && c[5];
				var b = c[0] & (c[1] || c[2]) & (c[3] || c[4]) & c[5];
				Assert.AreEqual(a, b);
			}
		}

		[TestMethod]
		public void Comparison6()
		{
			foreach (var c in _combinations)
			{
				var a = (c[0] || c[1]) & (c[2] || c[3]) & (c[4] || c[5]);
				var b = c[0] | c[1] && c[2] | c[3] && c[4] | c[5];
				Assert.AreEqual(a, b);
			}
		}

		[TestMethod]
		public void Comparison7()
		{
			foreach (var c in _combinations)
			{
				var a = c[0] || c[1] | c[2] || c[3] | c[4] || c[5];
				var b = c[0] | c[1] || c[2] | c[3] || c[4] | c[5];
				Assert.AreEqual(a, b);
			}
		}

		[TestMethod]
		public void Comparison8()
		{
			foreach (var c in _combinations)
			{
				var a = c[0] && c[1] & c[2] && c[3] & c[4] && c[5];
				var b = c[0] & c[1] && c[2] & c[3] && c[4] & c[5];
				Assert.AreEqual(a, b);
			}
		}

		[TestMethod]
		public void Comparison9()
		{
			foreach (var c in _combinations)
			{
				var a = c[0] && c[1] | c[2] & c[3];
				var b = c[0] && (c[1] || c[2] && c[3]);
				Assert.AreEqual(a, b);
			}
		}

	}
}
