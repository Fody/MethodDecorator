public class WhenDecoratingGenericTypes() :
    ClassTestsBase("SimpleTest.GenericType`1[[System.String, mscorlib]]")
{
    [Test]
    public async Task ShouldCaptureOnEntryAndExit()
    {
        const string expected = "Hello world";
        var value = TestClass.GetValue(expected);
        await Assert.That((object) value).IsEqualTo(expected);

        await CheckInit("SimpleTest.GenericType`1[System.String]", "SimpleTest.GenericType`1.GetValue", 1);
        await CheckMethodSeq([Method.Init, Method.OnEnter, Method.Body, Method.OnExit]);
    }
}