using Xunit;

public class WhenDecoratingExtensionMethods : ClassTestsBase
{
    public WhenDecoratingExtensionMethods() : base("SimpleTest.InterceptingExtensionMethods")
    {
    }

    [Fact]
    public void ShouldInterceptExtensionMethod()
    {
        var value = TestClass.ReturnsString();

        //Assert.Equal(4, this.testMessages.Messages.Count);
        CheckInit(null, "SimpleTest.StringExtensions.ToTitleCase", 1);
        CheckMethodSeq(new[] {Method.Init, Method.OnEnter, Method.Body, Method.OnExit});
        Assert.Equal("Hello World", value);
    }
}