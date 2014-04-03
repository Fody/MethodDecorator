using Xunit;

namespace MethodDecoratorEx.Fody.Tests {
    public class WhenDecoratedByNoInit : ClassTestsBase<DecoratedSimpleTest> {
        public WhenDecoratedByNoInit() : base("SimpleTest.MarkedWithNoInit") { }

        [Fact]
        public void NoInitMethodDecorated() {
            this.TestClass.NoInitMethodDecorated();
            this.CheckMethodSeq(new[] { Method.OnEnter, Method.Body, Method.OnExit });
        }
    }
}