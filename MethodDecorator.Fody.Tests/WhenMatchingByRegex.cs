using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MethodDecoratorEx.Fody.Tests
{
	public class WhenMatchingByRegex : ClassTestsBase
	{
		public WhenMatchingByRegex()
			: base("SimpleTest.MatchingByRegex.MatchingByRegex") { }

		[Fact]
		public void MethodMatchInclude()
		{
			this.TestClass.MethodMatchInclude();

			this.CheckMethodSeq(new[] {
				Method.Init, Method.OnEnter, Method.Body, Method.OnExit });

			this.CheckBody("MethodMatchInclude");
		}

		[Fact]
		public void MethodMatchExclude()
		{
			this.TestClass.MethodMatchExclude();

			this.CheckMethodSeq(new[] {
				Method.Body });

			this.CheckBody("MethodMatchExclude");
		}

		[Fact]
		public void PropertyGetInclude()
		{
			object dummy = this.TestClass.PropertyGetInclude;

			this.CheckMethodSeq(new[] {
				Method.Init, Method.OnEnter, Method.Body, Method.OnExit });

			this.CheckBody("PropertyGetInclude");
		}

		[Fact]
		public void PropertyGetExclude()
		{
			object dummy = this.TestClass.PropertyGetExclude;

			this.CheckMethodSeq(new[] {
				Method.Body });

			this.CheckBody("PropertyGetExclude");
		}
	}
}
