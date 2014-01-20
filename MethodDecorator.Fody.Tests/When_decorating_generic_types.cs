using System.Reflection;

using Xunit;

namespace MethodDecorator.Fody.Tests {
    public class When_decorating_generic_types : IUseFixture<DecoratedSimpleTest> {
        private Assembly assembly;
        private dynamic testClass;
        private dynamic testMessages;

        public void SetFixture(DecoratedSimpleTest data) {
            this.assembly = data.Assembly;
            this.testClass = this.assembly.GetInstance("SimpleTest.GenericType`1[[System.String, mscorlib]]");
            this.testMessages = this.assembly.GetStaticInstance("SimpleTest.TestMessages");
            this.testMessages.Clear();
        }

        [Fact]
        public void Should_capture_on_entry_and_exit() {
            const string expected = "Hello world";
            dynamic value = this.testClass.GetValue(expected);

            Assert.Equal(expected, value);

            Assert.Contains("Init: SimpleTest.GenericType`1.GetValue [1]", this.testMessages.Messages);
            Assert.Contains("OnEntry", this.testMessages.Messages);
            Assert.Contains("OnExit", this.testMessages.Messages);
        }
    }
}