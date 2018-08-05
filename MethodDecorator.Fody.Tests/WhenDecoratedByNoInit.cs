using Xunit;

public class WhenDecoratedByNoInit : ClassTestsBase
{
    public WhenDecoratedByNoInit() : base("SimpleTest.MarkedWithNoInit")
    {
    }

    [Fact]
    public void NoInitMethodDecorated()
    {
        TestClass.NoInitMethodDecorated();
        CheckMethodSeq(new[] {Method.OnEnter, Method.Body, Method.OnExit});
    }
}