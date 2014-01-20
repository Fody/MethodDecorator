using System.Reflection;

using Xunit;

namespace MethodDecorator.Fody.Tests {
    public class When_decorating_by_external_interceptor : IUseFixture<DecorateWithExternalTest> {
        private Assembly assembly;
        private dynamic testClass;
        private dynamic testMessages;

        public void SetFixture(DecorateWithExternalTest data) {
            this.testClass = data.Assembly.GetInstance("SimpleTest.MarkedFromAnotherAssembly");
            this.testMessages = data.ExternalAssembly.GetStaticInstance(
                "AnotherAssemblyAttributeContainer.TestMessages");
            this.testMessages.Clear();
        }

        [Fact]
        public void Should_notify_on_init_module_registered() {
            this.testClass.ExternalInterceptorDecorated();

            Assert.Contains(
                "Init: SimpleTest.MarkedFromAnotherAssembly.ExternalInterceptorDecorated [0]",
                this.testMessages.Messages);
        }

        [Fact]
        public void Should_notify_on_init_assembly_registered() {
            this.testClass.ExternalInterceptorAssemblyLevelDecorated();

            Assert.Contains(
                "Init: SimpleTest.MarkedFromAnotherAssembly.ExternalInterceptorAssemblyLevelDecorated [0]",
                this.testMessages.Messages);
        }
    }
}