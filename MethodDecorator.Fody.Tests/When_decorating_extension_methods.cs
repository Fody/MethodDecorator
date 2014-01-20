using System.Reflection;

using Xunit;

namespace MethodDecorator.Fody.Tests {
    public class When_decorating_extension_methods : IUseFixture<DecoratedSimpleTest> {
        private Assembly assembly;
        private dynamic testClass;
        private dynamic testMessages;

        public void SetFixture(DecoratedSimpleTest data) {
            this.assembly = data.Assembly;
            this.testClass = this.assembly.GetInstance("SimpleTest.InterceptingExtensionMethods");
            this.testMessages = this.assembly.GetStaticInstance("SimpleTest.TestMessages");
            this.testMessages.Clear();
        }

        [Fact]
        public void Should_intercept_extension_method() {
            dynamic value = this.testClass.ReturnsString();

            Assert.Equal(4, this.testMessages.Messages.Count);
            Assert.Contains("Init: SimpleTest.StringExtensions.ToTitleCase [1]", this.testMessages.Messages);
            Assert.Contains("OnEntry", this.testMessages.Messages);
            Assert.Contains("ToTitleCase: In extension method", this.testMessages.Messages);
            Assert.Contains("OnExit", this.testMessages.Messages);
            Assert.Equal("Hello World", value);
        }
    }
}