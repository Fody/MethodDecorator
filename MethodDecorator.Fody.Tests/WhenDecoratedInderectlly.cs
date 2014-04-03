using Xunit;

namespace MethodDecoratorEx.Fody.Tests {
    public class WhenDecoratedInderectlly: ClassTestsBase<DecoratedSimpleTest> {
        public WhenDecoratedInderectlly() : base("SimpleTest.MarkedWithInderectAttribute") { }

        [Fact]
        public void ObsoleteDecorated() {
            this.TestClass.ObsoleteDecorated();
            this.CheckMethodSeq(new[] { Method.OnEnter, Method.Body, Method.OnExit });
        }
    }
}