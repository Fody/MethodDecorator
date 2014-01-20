using System.Reflection;

using MethodDecoratorEx.Fody.Tests;

using Xunit;

namespace MethodDecorator.Fody.Tests {
    public class When_decorating_generic_methods : IUseFixture<DecoratedSimpleTest> {
        private Assembly assembly;
        private dynamic testClass;
        private dynamic testMessages;

        public void SetFixture(DecoratedSimpleTest data) {
            this.assembly = data.Assembly;
            this.testClass = this.assembly.GetInstance("SimpleTest.GenericMethod");
            this.testMessages = this.assembly.GetStaticInstance("SimpleTest.TestMessages");
            this.testMessages.Clear();
        }

        [Fact]
        public void Should_capture_on_entry_and_exit() {
            const string expected = "Hello world";
            dynamic value = this.testClass.GetValue<string>(expected);

            Assert.Equal(expected, value);

            Assert.Contains("Init: SimpleTest.GenericMethod.GetValue [1]", this.testMessages.Messages);
            Assert.Contains("OnEntry", this.testMessages.Messages);
            Assert.Contains("OnExit", this.testMessages.Messages);
        }

        [Fact]
        public void Should_capture_on_entry_and_exit_when_parameter_value_type() {
            const int expected = 42;
            dynamic value = this.testClass.GetValue<int>(expected);

            Assert.Equal(expected, value);

            Assert.Contains("Init: SimpleTest.GenericMethod.GetValue [1]", this.testMessages.Messages);
            Assert.Contains("OnEntry", this.testMessages.Messages);
            Assert.Contains("OnExit", this.testMessages.Messages);
        }
    }
}