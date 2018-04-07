using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xunit;

namespace MethodDecorator.Fody.Tests
{
	public class WhenDecoratedByDerivedMatchingModuleTypeExclude : ClassTestsBase
	{
		public WhenDecoratedByDerivedMatchingModuleTypeExclude() 
			: base("SimpleTest.DerivedMatchingModule.DerivedMatchingModuleTypeExclude") { }

		[Fact]
		public void ConstructorTrigger()
		{
			var m = this.TestClass;
			this.CheckMethodSeq(new Method[] { });
		}

		[Fact]
		public void ExcludeAtTypeLevel()
		{
			this.TestClass.ExcludeAtTypeLevel();
			this.CheckMethodSeq(new[] {
				 Method.Body }); 

			this.CheckBody("ExcludeAtTypeLevel");
		}

		[Fact]
		public void ReIncludeAtMethodLevel()
		{
			this.TestClass.ReIncludeAtMethodLevel();
			this.CheckMethodSeq(new[] {
				 Method.Init, Method.OnEnter, Method.Body, Method.OnExit});

			this.CheckBody("ReIncludeAtMethodLevel");
		}

	}
}