using System.Reflection;

using Xunit;

namespace MethodDecorator.Fody.Tests
{
    public class When_decorating_by_external_interceptor : IUseFixture<DecorateWithExternalTest>
    {
        private Assembly assembly;
        private dynamic testClass;
        private dynamic testMessages;

        [Fact]
        public void Should_notify_of_method_entry_module_registered()
        {
            testClass.ExternalInterceptorDecorated();

            Assert.Contains("OnEntry: SimpleTest.MarkedFromAnotherAssembly.ExternalInterceptorDecorated", testMessages.Messages);
        }

        [Fact]
        public void Should_notify_of_method_entry_assembly_registered() {
            testClass.ExternalInterceptorAssemblyLevelDecorated();

            Assert.Contains("OnEntry: SimpleTest.MarkedFromAnotherAssembly.ExternalInterceptorAssemblyLevelDecorated", testMessages.Messages);
        }

        public void SetFixture(DecorateWithExternalTest data)
        {
            testClass = data.Assembly.GetInstance("SimpleTest.MarkedFromAnotherAssembly");
            testMessages = data.ExternalAssembly.GetStaticInstance("AnotherAssemblyAttributeContainer.TestMessages");
            testMessages.Clear();
        }
    }
}