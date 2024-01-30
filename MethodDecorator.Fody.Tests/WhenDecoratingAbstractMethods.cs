public class WhenDecoratingAbstractMethods() :
    ClassTestsBase("SimpleTest.InterceptingAbstractMethods")
{
    [Fact]
    public void ShouldNotTryToDecorateAbstractMethod()
    {
        TestClass.AbstractMethod();
        CheckMethodSeq([Method.Body]);
    }
}