using System.Reflection;

using MethodDecorator.Fody.Tests;

using Xunit;

namespace MethodDecoratorEx.Fody.Tests {
    public class WhenDecoratingExtensionMethods : ClassTestsBase<DecoratedSimpleTest> {
        public WhenDecoratingExtensionMethods() : base("SimpleTest.InterceptingExtensionMethods") { }

        [Fact]
        public void Should_intercept_extension_method() {
            dynamic value = this.TestClass.ReturnsString();

            //Assert.Equal(4, this.testMessages.Messages.Count);
            this.CheckMethodSeq(new []{Method.Init, Method.OnEnter,Method.Body,Method.OnExit});
            Assert.Equal("Hello World", value);
        }
    }
}