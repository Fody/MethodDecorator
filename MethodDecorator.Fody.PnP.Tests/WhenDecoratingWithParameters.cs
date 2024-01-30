public class WhenDecoratingWithParameters :
    SimpleTestBase
{
    [Fact]
    public void ShouldReportInitWithAttrParameters()
    {
        var testClass = WeaverHelperWrapper.Assembly.GetInstance("SimpleTest.PnP.InterceptedMethods");
        Assert.NotNull(testClass);

        testClass.ExplicitIntercepted();

        CheckMethod(Method.Init, [15, "parameter", "property", "field"]);
    }

    [Fact]
    public void ShouldNotAffectNext()
    {
        var testClass = WeaverHelperWrapper.Assembly.GetInstance("SimpleTest.PnP.InterceptedMethods");
        Assert.NotNull(testClass);

        testClass.ExplicitIntercepted();
        testClass.ExplicitIntercepted();

        CheckMethod(
            Method.Init,
            [[15, "parameter", "property", "field"], [15, "parameter", "property", "field"]]);

        CheckMethod(
            Method.OnExit,
            [[16, "parameter", "property", "field"], [16, "parameter", "property", "field"]]);
    }

    [Fact]
    public void ShouldNotAffectInnerMethods()
    {
        var testClass = WeaverHelperWrapper.Assembly.GetInstance("SimpleTest.PnP.InterceptedMethods");
        Assert.NotNull(testClass);

        testClass.OuterMethod();

        CheckMethod(
            Method.Init,
            [[1, "parameter", "property", "field"], [1, "parameter", "property", "field"]]);

        CheckMethod(
            Method.OnExit,
            [[2, "parameter", "property", "field"], [2, "parameter", "property", "field"]]);
    }

    [Fact]
    public void ShouldImplicitIntercept()
    {
        var testClass = WeaverHelperWrapper.Assembly.GetInstance("SimpleTest.PnP.InterceptedClass");
        Assert.NotNull(testClass);

        testClass.ImplicitIntercepted();

        CheckMethod(Method.Init, [1, "class_parameter", "class_property", "class_field"]);
    }

    [Fact]
    public void ShouldPreferExplicitIntercept()
    {
        var testClass = WeaverHelperWrapper.Assembly.GetInstance("SimpleTest.PnP.InterceptedClass");
        Assert.NotNull(testClass);

        testClass.ExplicitIntercepted();

        CheckMethod(Method.Init, [10, "method_parameter", "method_property", "method_field"]);
    }

    [Fact]
    public void ShouldInterceptRetval()
    {
        var testClass = WeaverHelperWrapper.Assembly.GetInstance("SimpleTest.PnP.InterceptedMethods");
        Assert.NotNull(testClass);

        testClass.InterceptedReturns10();
        testClass.InterceptedReturnsString();
        testClass.InterceptedReturnsType();

        CheckMethod(Method.OnExit, [[10], ["Intercepted"], [testClass.GetType()]]);
    }

    [Fact]
    public void ShouldInterceptGenericRetval()
    {
        var testClass = WeaverHelperWrapper.Assembly.GetInstance("SimpleTest.PnP.InterceptedMethods");
        Assert.NotNull(testClass);

        Assert.NotNull(testClass.GenericMethod<object>());

        CheckMethod(Method.OnExit, ["string"]);
    }
}