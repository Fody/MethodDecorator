using MethodDecorator.Fody.Tests;

using Xunit;

namespace MethodDecoratorEx.Fody.Tests {
    public class WhenDecoratingByExternalInterceptor : ClassTestsBase<DecorateWithExternalTest> {
        public WhenDecoratingByExternalInterceptor() : base("SimpleTest.MarkedFromAnotherAssembly") {}

        public override void SetFixture(DecorateWithExternalTest data) {
            base.SetFixture(data);
            this.RecordHost = data.ExternalAssembly.GetStaticInstance("SimpleTest.TestRecords");
            this.RecordHost.Clear();
        }

        [Fact]
        public void ShouldNotifyOnInitModuleRegistered() {
            this.TestClass.ExternalInterceptorDecorated();
            this.CheckInit("SimpleTest.MarkedFromAnotherAssembly", "SimpleTest.MarkedFromAnotherAssembly.ExternalInterceptorDecorated");
        }

        [Fact]
        public void ShouldNotifyOnInitAssemblyRegistered() {
            this.TestClass.ExternalInterceptorAssemblyLevelDecorated();
            this.CheckInit("SimpleTest.MarkedFromAnotherAssembly", "SimpleTest.MarkedFromAnotherAssembly.ExternalInterceptorAssemblyLevelDecorated");
        }
    }
}