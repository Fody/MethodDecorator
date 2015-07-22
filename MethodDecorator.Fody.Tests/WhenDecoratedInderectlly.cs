namespace MethodDecoratorEx.Fody.Tests
{
    using Xunit;

    public class WhenDecoratedInderectlly : ClassTestsBase
    {
        public WhenDecoratedInderectlly() : base("SimpleTest.MarkedWithInderectAttribute")
        {
        }

        [Fact]
        public void ObsoleteDecorated()
        {
            TestClass.ObsoleteDecorated();
            CheckMethodSeq(new[] {Method.OnEnter, Method.Body, Method.OnExit});
        }
    }
}