public class WhenDecoratingGenericMethods() :
    ClassTestsBase("SimpleTest.GenericMethod")
{
    [Fact]
    public void ShouldCaptureOnEntryAndExit()
    {
        const string expected = "Hello world";
        var value = TestClass.GetValue<string>(expected);
        Assert.Equal(expected, value);

        CheckInit("SimpleTest.GenericMethod", "SimpleTest.GenericMethod.GetValue", 1);
        CheckMethodSeq([Method.Init, Method.OnEnter, Method.Body, Method.OnExit]);
    }

    [Fact]
    public void ShouldCaptureOnEntryAndExitWhenParameterValueType()
    {
        const int expected = 42;
        var value = TestClass.GetValue<int>(expected);
        Assert.Equal(expected, value);

        CheckInit("SimpleTest.GenericMethod", "SimpleTest.GenericMethod.GetValue", 1);
        CheckMethodSeq([Method.Init, Method.OnEnter, Method.Body, Method.OnExit]);
    }
}