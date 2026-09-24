public class WhenDecoratedByNoInit() :
    ClassTestsBase("SimpleTest.MarkedWithNoInit")
{
    [Test]
    public async Task NoInitMethodDecorated()
    {
        TestClass.NoInitMethodDecorated();
        await CheckMethodSeq([Method.OnEnter, Method.Body, Method.OnExit]);
    }
}