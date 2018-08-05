using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MethodDecorator.Fody.Tests
{
	public class WhenMatchingByCommaSeparatedA : ClassTestsBase
	{
		public WhenMatchingByCommaSeparatedA()
			: base("SimpleTest.MatchingCommaSeparated.MatchingCommaSeparatedA") { }

		[Fact]
		public void AppliesToNamespace()
		{
			TestClass.AppliesToNamespace();

			CheckMethodSeq(new[] {
				Method.Init, Method.OnEnter, Method.OnExit,
				Method.Init, Method.OnEnter, Method.Body, Method.OnExit });

			CheckBody("AppliesToNamespace");
		}
	}

	public class WhenMatchingByCommaSeparatedB : ClassTestsBase
	{
		public WhenMatchingByCommaSeparatedB()
			: base("SimpleTest.MatchingCommaSeparated.MatchingCommaSeparatedB") { }

		[Fact]
		public void AppliesToNamespace()
		{
			TestClass.AppliesToNamespace();

			CheckMethodSeq(new[] {
				Method.Init, Method.OnEnter, Method.OnExit,
				Method.Init, Method.OnEnter, Method.Body, Method.OnExit });

			CheckBody("AppliesToNamespace");
		}
	}

}