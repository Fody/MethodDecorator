public class WhenDecoratingAbstractMethods() :
    ClassTestsBase("SimpleTest.InterceptingAbstractMethods")
{
    [Test]
    public async Task ShouldNotTryToDecorateAbstractMethod()
    {
        TestClass.AbstractMethod();
        await CheckMethodSeq([Method.Body]);
    }
}