public class WhenInterceptingNestedTypes :
    SimpleTestBase
{
    [Test]
    public async Task ShouldDecorateMethodInNestedType()
    {
        var testClass = WeaverHelperWrapper.Assembly.GetInstance("SimpleTest.InterceptingNestedTypes+Nested");
        var value = testClass.StringMethod();

        await Assert.That((object) value).IsEqualTo("sausages");

        await CheckMethodSeq([Method.Init, Method.OnEnter, Method.OnExit]);
        await CheckInit("SimpleTest.InterceptingNestedTypes+Nested", "SimpleTest.InterceptingNestedTypes+Nested.StringMethod");
    }

    [Test]
    public async Task ShouldDecorateADeeplyNestedType()
    {
        var testClass =
            WeaverHelperWrapper.Assembly.GetInstance("SimpleTest.InterceptingNestedTypes+FirstLevel+SecondLevel+DeeplyNested");
        var value = testClass.NumberMethod();

        await Assert.That((object) value).IsEqualTo(42);

        await CheckMethodSeq([Method.Init, Method.OnEnter, Method.OnExit]);
        await CheckInit("SimpleTest.InterceptingNestedTypes+FirstLevel+SecondLevel+DeeplyNested", "SimpleTest.InterceptingNestedTypes+FirstLevel+SecondLevel+DeeplyNested.NumberMethod");
    }
}