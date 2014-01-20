using Xunit;

namespace MethodDecoratorEx.Fody.Tests {
    public class WhenDecoratingAbstractMethods : ClassTestsBase<DecoratedSimpleTest> {
        public WhenDecoratingAbstractMethods() : base("SimpleTest.InterceptingAbstractMethods") {}

        [Fact]
        public void ShouldNotTryToDecorateAbstractMethod() {
            this.TestClass.AbstractMethod();
            this.CheckMethodSeq(new[] { Method.Body });
        }
    }
}