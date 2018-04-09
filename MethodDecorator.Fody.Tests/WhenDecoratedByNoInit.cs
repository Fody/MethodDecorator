using Xunit;

namespace MethodDecorator.Fody.Tests {
    public class WhenDecoratedByNoInit : ClassTestsBase {
        public WhenDecoratedByNoInit() : base("SimpleTest.MarkedWithNoInit") { }

        [Fact]
        public void NoInitMethodDecorated() {
            this.TestClass.NoInitMethodDecorated();
            this.CheckMethodSeq(new[] { Method.OnEnter, Method.Body, Method.OnExit });
        }
    }
}