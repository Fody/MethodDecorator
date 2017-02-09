using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MethodDecoratorEx.Fody.Tests
{
	public class WhenMatchingByCommaSeparatedA : ClassTestsBase
	{
		public WhenMatchingByCommaSeparatedA()
			: base("SimpleTest.MatchingCommaSeparated.MatchingCommaSeparatedA") { }

		[Fact]
		public void AppliesToNamespace()
		{
			this.TestClass.AppliesToNamespace();

			this.CheckMethodSeq(new[] {
				Method.Init, Method.OnEnter, Method.OnExit,
				Method.Init, Method.OnEnter, Method.Body, Method.OnExit });

			this.CheckBody("AppliesToNamespace");
		}
	}

	public class WhenMatchingByCommaSeparatedB : ClassTestsBase
	{
		public WhenMatchingByCommaSeparatedB()
			: base("SimpleTest.MatchingCommaSeparated.MatchingCommaSeparatedB") { }

		[Fact]
		public void AppliesToNamespace()
		{
			this.TestClass.AppliesToNamespace();

			this.CheckMethodSeq(new[] {
				Method.Init, Method.OnEnter, Method.OnExit,
				Method.Init, Method.OnEnter, Method.Body, Method.OnExit });

			this.CheckBody("AppliesToNamespace");
		}
	}

}