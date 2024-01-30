public class WhenDecoratedByNoInit() :
    ClassTestsBase("SimpleTest.MarkedWithNoInit")
{
    [Fact]
    public void NoInitMethodDecorated()
    {
        TestClass.NoInitMethodDecorated();
        CheckMethodSeq([Method.OnEnter, Method.Body, Method.OnExit]);
    }
}