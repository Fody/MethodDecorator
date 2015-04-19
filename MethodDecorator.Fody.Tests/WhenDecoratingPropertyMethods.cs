using System;
using System.Reflection;

using MethodDecorator.Fody.Tests;

using Xunit;

namespace MethodDecoratorEx.Fody.Tests {
    public class WhenDecoratingPropertyMethods : SimpleTestBase {
        public WhenDecoratingPropertyMethods()
            : base("SimpleTest.InterceptingPropertyMethods") { }

        [Fact]
        public void ShouldNotifyOnEntryAndExitForManualPropertySetter() {
            this.TestClass.ManualProperty = 199;
            this.CheckInit("SimpleTest.InterceptingPropertyMethods", "SimpleTest.InterceptingPropertyMethods.set_ManualProperty", 1);
            this.CheckMethodSeq(new[] { Method.Init, Method.OnEnter, Method.OnExit });
        }

        [Fact]
        public void ShouldNotifyOnEntryAndExitForManualPropertyGetter() {
            int value = this.TestClass.ManualProperty;
            Assert.Equal(0, value);

            this.CheckInit("SimpleTest.InterceptingPropertyMethods", "SimpleTest.InterceptingPropertyMethods.get_ManualProperty");
            this.CheckMethodSeq(new[] { Method.Init, Method.OnEnter, Method.OnExit });
        }

        [Fact]
        public void ShouldNotifyOnEntryAndExitForReadonlyPropertyAttributedOnGetter() {
            int value = this.TestClass.ReadOnlyProperty;
            Assert.Equal(42, value);

            this.CheckInit("SimpleTest.InterceptingPropertyMethods", "SimpleTest.InterceptingPropertyMethods.get_ReadOnlyProperty");
            this.CheckMethodSeq(new[] { Method.Init, Method.OnEnter, Method.OnExit });
        }

        [Fact]
        public void ShouldNotifyOnEntryAndExitForWriteonlyPropertyAttributedOnSetter() {
            this.TestClass.WriteOnlyProperty = 99;
            Assert.Equal(99, this.TestClass.WriteOnlyPropertyField);

            this.CheckInit("SimpleTest.InterceptingPropertyMethods", "SimpleTest.InterceptingPropertyMethods.set_WriteOnlyProperty", 1);
            this.CheckMethodSeq(new[] { Method.Init, Method.OnEnter, Method.OnExit });
        }

        [Fact]
        public void ShouldNotifyOnEntryAndExceptionForPropertyGetter() {
            Assert.Throws<InvalidOperationException>(() => this.TestClass.ThrowingProperty);

            this.CheckInit("SimpleTest.InterceptingPropertyMethods", "SimpleTest.InterceptingPropertyMethods.get_ThrowingProperty");
            this.CheckEntry();
            CheckException<InvalidOperationException>("Ooops");
        }
    }
}