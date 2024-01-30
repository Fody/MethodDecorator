public class WhenDecoratedIndirectly() :
    ClassTestsBase("SimpleTest.MarkedWithIndirectAttribute")
{
    [Fact]
    public void ObsoleteDecorated()
    {
        TestClass.ObsoleteDecorated();
        CheckMethodSeq([Method.OnEnter, Method.Body, Method.OnExit]);
    }
}