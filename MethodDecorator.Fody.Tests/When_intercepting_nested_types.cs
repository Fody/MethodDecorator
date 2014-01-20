using System.Reflection;

using Xunit;

namespace MethodDecorator.Fody.Tests {
    public class When_intercepting_nested_types : IUseFixture<DecoratedSimpleTest> {
        private Assembly assembly;
        private dynamic testMessages;

        public void SetFixture(DecoratedSimpleTest data) {
            this.assembly = data.Assembly;
            this.testMessages = this.assembly.GetStaticInstance("SimpleTest.TestMessages");
            this.testMessages.Clear();
        }

        [Fact]
        public void Should_decorate_method_in_nested_type() {
            dynamic testClass = this.assembly.GetInstance("SimpleTest.InterceptingNestedTypes+Nested");
            dynamic value = testClass.StringMethod();

            Assert.Equal("sausages", value);

            Assert.Contains(
                "Init: SimpleTest.InterceptingNestedTypes+Nested.StringMethod [0]",
                this.testMessages.Messages);
            Assert.Contains("OnEntry", this.testMessages.Messages);
            Assert.Contains("OnExit", this.testMessages.Messages);
        }

        [Fact]
        public void Should_decorate_a_deeply_nested_type() {
            dynamic testClass =
                this.assembly.GetInstance("SimpleTest.InterceptingNestedTypes+FirstLevel+SecondLevel+DeeplyNested");
            dynamic value = testClass.NumberMethod();

            Assert.Equal(42, value);

            Assert.Contains(
                "Init: SimpleTest.InterceptingNestedTypes+FirstLevel+SecondLevel+DeeplyNested.NumberMethod [0]",
                this.testMessages.Messages);
            Assert.Contains("OnEntry", this.testMessages.Messages);
            Assert.Contains("OnExit", this.testMessages.Messages);
        }
    }
}