public class WhenDecoratingExtensionMethods() :
    ClassTestsBase("SimpleTest.InterceptingExtensionMethods")
{
    [Test]
    public async Task ShouldInterceptExtensionMethod()
    {
        var value = TestClass.ReturnsString();

        //await Assert.That((object) this.testMessages.Messages.Count).IsEqualTo(4);
        await CheckInit(null, "SimpleTest.StringExtensions.ToTitleCase", 1);
        await CheckMethodSeq([Method.Init, Method.OnEnter, Method.Body, Method.OnExit]);
        await Assert.That((object) value).IsEqualTo("Hello World");
    }
}