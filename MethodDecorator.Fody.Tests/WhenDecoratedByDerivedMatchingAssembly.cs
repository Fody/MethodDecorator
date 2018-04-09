﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xunit;

namespace MethodDecorator.Fody.Tests
{
	public class WhenDecoratedByDerivedMatchingAssembly : ClassTestsBase
	{
		public WhenDecoratedByDerivedMatchingAssembly() 
			: base("SimpleTest.DerivedMatchingAssembly.DerivedMatchingAssembly") { }

		[Fact]
		public void ConstructorTrigger()
		{
			var m = this.TestClass;
			this.CheckMethodSeq(new[] { Method.Init, Method.OnEnter, Method.OnExit });
		}

		[Fact]
		public void AppliesToNamespace()
		{
			this.TestClass.AppliesToNamespace();
			this.CheckMethodSeq(new[] {
				Method.Init, Method.OnEnter, Method.OnExit,						// Constructor
				Method.Init, Method.OnEnter, Method.Body, Method.OnExit });     // AppliesToNamespace()

			this.CheckBody("AppliesToNamespace");

		}

		[Fact]
		public void TurnOffAtMethodLevel()
		{
			this.TestClass.TurnOffAtMethodLevel();
			this.CheckMethodSeq(new[] { Method.Init, Method.OnEnter, Method.OnExit, // Constructor
				Method.Body});													// Nothing in body

			this.CheckBody("TurnOffAtMethodLevel");
		}

	}
}