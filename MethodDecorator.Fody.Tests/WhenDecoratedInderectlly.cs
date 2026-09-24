public class WhenDecoratedIndirectly() :
    ClassTestsBase("SimpleTest.MarkedWithIndirectAttribute")
{
    [Test]
    public async Task ObsoleteDecorated()
    {
        TestClass.ObsoleteDecorated();
        await CheckMethodSeq([Method.OnEnter, Method.Body, Method.OnExit]);
    }
}