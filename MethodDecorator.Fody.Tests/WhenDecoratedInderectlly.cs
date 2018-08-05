using Xunit;

public class WhenDecoratedIndirectly : ClassTestsBase
{
    public WhenDecoratedIndirectly() : base("SimpleTest.MarkedWithIndirectAttribute")
    {
    }

    [Fact]
    public void ObsoleteDecorated()
    {
        TestClass.ObsoleteDecorated();
        CheckMethodSeq(new[] {Method.OnEnter, Method.Body, Method.OnExit});
    }
}