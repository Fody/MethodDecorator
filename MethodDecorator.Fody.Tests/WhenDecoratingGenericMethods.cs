public class WhenDecoratingGenericMethods() :
    ClassTestsBase("SimpleTest.GenericMethod")
{
    [Test]
    public async Task ShouldCaptureOnEntryAndExit()
    {
        const string expected = "Hello world";
        var value = TestClass.GetValue<string>(expected);
        await Assert.That((object) value).IsEqualTo(expected);

        await CheckInit("SimpleTest.GenericMethod", "SimpleTest.GenericMethod.GetValue", 1);
        await CheckMethodSeq([Method.Init, Method.OnEnter, Method.Body, Method.OnExit]);
    }

    [Test]
    public async Task ShouldCaptureOnEntryAndExitWhenParameterValueType()
    {
        const int expected = 42;
        var value = TestClass.GetValue<int>(expected);
        await Assert.That((object) value).IsEqualTo(expected);

        await CheckInit("SimpleTest.GenericMethod", "SimpleTest.GenericMethod.GetValue", 1);
        await CheckMethodSeq([Method.Init, Method.OnEnter, Method.Body, Method.OnExit]);
    }
}