using Xunit;

namespace MethodDecoratorEx.Fody.Tests {
    public class WhenDecoratingExtensionMethods : ClassTestsBase<DecoratedSimpleTest> {
        public WhenDecoratingExtensionMethods() : base("SimpleTest.InterceptingExtensionMethods") {}

        [Fact]
        public void ShouldInterceptExtensionMethod() {
            dynamic value = this.TestClass.ReturnsString();

            //Assert.Equal(4, this.testMessages.Messages.Count);
            this.CheckMethodSeq(new[] { Method.Init, Method.OnEnter, Method.Body, Method.OnExit });
            Assert.Equal("Hello World", value);
        }
    }
}