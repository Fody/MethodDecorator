using Xunit;

namespace MethodDecorator.Fody.Tests {
    public class WhenDecoratingExtensionMethods : ClassTestsBase {
        public WhenDecoratingExtensionMethods() : base("SimpleTest.InterceptingExtensionMethods") {}

        [Fact]
        public void ShouldInterceptExtensionMethod() {
            dynamic value = this.TestClass.ReturnsString();

            //Assert.Equal(4, this.testMessages.Messages.Count);
            this.CheckInit(null, "SimpleTest.StringExtensions.ToTitleCase", 1);
            this.CheckMethodSeq(new[] { Method.Init, Method.OnEnter, Method.Body, Method.OnExit });
            Assert.Equal("Hello World", value);
        }
    }
}