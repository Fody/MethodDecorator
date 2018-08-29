using Xunit;

public class WhenInterceptingNestedTypes : SimpleTestBase
{
    [Fact]
    public void ShouldDecorateMethodInNestedType()
    {
        var testClass = WeaverHelperWrapper.Assembly.GetInstance("SimpleTest.InterceptingNestedTypes+Nested");
        var value = testClass.StringMethod();

        Assert.Equal("sausages", value);

        CheckMethodSeq(new[] {Method.Init, Method.OnEnter, Method.OnExit});
        CheckInit("SimpleTest.InterceptingNestedTypes+Nested", "SimpleTest.InterceptingNestedTypes+Nested.StringMethod");
    }

    [Fact]
    public void ShouldDecorateADeeplyNestedType()
    {
        var testClass =
            WeaverHelperWrapper.Assembly.GetInstance("SimpleTest.InterceptingNestedTypes+FirstLevel+SecondLevel+DeeplyNested");
        var value = testClass.NumberMethod();

        Assert.Equal(42, value);

        CheckMethodSeq(new[] {Method.Init, Method.OnEnter, Method.OnExit});
        CheckInit("SimpleTest.InterceptingNestedTypes+FirstLevel+SecondLevel+DeeplyNested", "SimpleTest.InterceptingNestedTypes+FirstLevel+SecondLevel+DeeplyNested.NumberMethod");
    }
}