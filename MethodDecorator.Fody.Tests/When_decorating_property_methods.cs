using System;
using System.Reflection;

using MethodDecoratorEx.Fody.Tests;

using Xunit;

namespace MethodDecorator.Fody.Tests {
    public class When_decorating_property_methods : IUseFixture<DecoratedSimpleTest> {
        private Assembly assembly;
        private dynamic testClass;
        private dynamic testMessages;

        public void SetFixture(DecoratedSimpleTest data) {
            this.assembly = data.Assembly;
            this.testClass = this.assembly.GetInstance("SimpleTest.InterceptingPropertyMethods");
            this.testMessages = this.assembly.GetStaticInstance("SimpleTest.TestMessages");
            this.testMessages.Clear();
        }

        [Fact]
        public void Should_notify_on_entry_and_exit_for_manual_property() {
            this.testClass.ManualProperty = 199;
            int value = this.testClass.ManualProperty;

            Assert.Equal(199, value);

            Assert.Contains(
                "Init: SimpleTest.InterceptingPropertyMethods.set_ManualProperty [1]",
                this.testMessages.Messages);
            Assert.Contains("OnEntry", this.testMessages.Messages);
            Assert.Contains("OnExit", this.testMessages.Messages);

            Assert.Contains(
                "Init: SimpleTest.InterceptingPropertyMethods.get_ManualProperty [0]",
                this.testMessages.Messages);
            Assert.Contains("OnEntry", this.testMessages.Messages);
            Assert.Contains("OnExit", this.testMessages.Messages);
        }

        [Fact]
        public void Should_notify_on_entry_and_exit_for_readonly_property_attributed_on_getter() {
            int value = this.testClass.ReadOnlyProperty;

            Assert.Equal(42, value);

            Assert.Contains(
                "Init: SimpleTest.InterceptingPropertyMethods.get_ReadOnlyProperty [0]",
                this.testMessages.Messages);
            Assert.Contains("OnEntry", this.testMessages.Messages);
            Assert.Contains("OnExit", this.testMessages.Messages);
        }

        [Fact]
        public void Should_notify_on_entry_and_exit_for_writeonly_property_attributed_on_setter() {
            this.testClass.WriteOnlyProperty = 99;

            Assert.Equal(99, this.testClass.WriteOnlyPropertyField);

            Assert.Contains(
                "Init: SimpleTest.InterceptingPropertyMethods.set_WriteOnlyProperty [1]",
                this.testMessages.Messages);
            Assert.Contains("OnEntry", this.testMessages.Messages);
            Assert.Contains("OnExit", this.testMessages.Messages);
        }

        [Fact]
        public void Should_notify_on_entry_and_exception_for_property_getter() {
            Assert.Throws<InvalidOperationException>(() => this.testClass.ThrowingProperty);

            Assert.Contains(
                "Init: SimpleTest.InterceptingPropertyMethods.get_ThrowingProperty [0]",
                this.testMessages.Messages);
            Assert.Contains("OnEntry", this.testMessages.Messages);
            Assert.Contains("OnException: System.InvalidOperationException: Ooops", this.testMessages.Messages);
        }
    }
}