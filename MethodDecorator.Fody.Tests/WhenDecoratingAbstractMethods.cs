using Xunit;

namespace MethodDecorator.Fody.Tests {
    public class WhenDecoratingAbstractMethods : ClassTestsBase {
        public WhenDecoratingAbstractMethods() : base("SimpleTest.InterceptingAbstractMethods") {}

        [Fact]
        public void ShouldNotTryToDecorateAbstractMethod() {
            this.TestClass.AbstractMethod();
            this.CheckMethodSeq(new[] { Method.Body });
        }
    }
}