using System;
using System.Reflection;

using Xunit;

namespace MethodDecorator.Fody.Tests {
    public class When_decorating_constructors : IUseFixture<DecoratedSimpleTest> {
        private Assembly assembly;
        private dynamic testMessages;

        public void SetFixture(DecoratedSimpleTest data) {
            this.assembly = data.Assembly;
            this.testMessages = this.assembly.GetStaticInstance("SimpleTest.TestMessages");
            this.testMessages.Clear();
        }

        [Fact]
        public void Should_report_on_entry_and_exit() {
            dynamic testClass = this.assembly.GetInstance("SimpleTest.InterceptingConstructors+SimpleConstructor");

            Assert.NotNull(testClass);

            Assert.Contains(
                "Init: SimpleTest.InterceptingConstructors+SimpleConstructor..ctor [0]",
                this.testMessages.Messages);
            Assert.Contains("OnEntry", this.testMessages.Messages);
            Assert.Contains("OnExit", this.testMessages.Messages);
        }

        [Fact]
        public void Should_report_on_entry_and_exception() {
            Exception exception =
                Record.Exception(
                    () => this.assembly.GetInstance("SimpleTest.InterceptingConstructors+ThrowingConstructor"));

            // This is because we're using reflection to create the instance.
            // It will wrap any exception
            if (exception is TargetInvocationException)
                exception = exception.InnerException;

            Assert.IsType<InvalidOperationException>(exception);

            Assert.Contains(
                "Init: SimpleTest.InterceptingConstructors+ThrowingConstructor..ctor [0]",
                this.testMessages.Messages);
            Assert.Contains("OnEntry", this.testMessages.Messages);
            Assert.Contains("OnException: System.InvalidOperationException: Ooops", this.testMessages.Messages);
        }
    }
}