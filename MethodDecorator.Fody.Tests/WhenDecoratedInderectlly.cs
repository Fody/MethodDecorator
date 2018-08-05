using Xunit;

namespace MethodDecorator.Fody.Tests {
    public class WhenDecoratedIndirectly : ClassTestsBase {
        public WhenDecoratedIndirectly() : base("SimpleTest.MarkedWithIndirectAttribute") { }

        [Fact]
        public void ObsoleteDecorated() {
            this.TestClass.ObsoleteDecorated();
            this.CheckMethodSeq(new[] { Method.OnEnter, Method.Body, Method.OnExit });
        }
    }
}