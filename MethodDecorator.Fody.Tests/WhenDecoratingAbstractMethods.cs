using Xunit;

public class WhenDecoratingAbstractMethods : ClassTestsBase
{
    public WhenDecoratingAbstractMethods() : base("SimpleTest.InterceptingAbstractMethods")
    {
    }

    [Fact]
    public void ShouldNotTryToDecorateAbstractMethod()
    {
        TestClass.AbstractMethod();
        CheckMethodSeq(new[] {Method.Body});
    }
}