public class WhenDecoratingWithParameters :
    SimpleTestBase
{
    [Test]
    public async Task ShouldReportInitWithAttrParameters()
    {
        var testClass = WeaverHelperWrapper.Assembly.GetInstance("SimpleTest.PnP.InterceptedMethods");
        await Assert.That((object) testClass).IsNotNull();

        testClass.ExplicitIntercepted();

        await CheckMethod(Method.Init, [15, "parameter", "property", "field"]);
    }

    [Test]
    public async Task ShouldNotAffectNext()
    {
        var testClass = WeaverHelperWrapper.Assembly.GetInstance("SimpleTest.PnP.InterceptedMethods");
        await Assert.That((object) testClass).IsNotNull();

        testClass.ExplicitIntercepted();
        testClass.ExplicitIntercepted();

        await CheckMethod(
            Method.Init,
            [[15, "parameter", "property", "field"], [15, "parameter", "property", "field"]]);

        await CheckMethod(
            Method.OnExit,
            [[16, "parameter", "property", "field"], [16, "parameter", "property", "field"]]);
    }

    [Test]
    public async Task ShouldNotAffectInnerMethods()
    {
        var testClass = WeaverHelperWrapper.Assembly.GetInstance("SimpleTest.PnP.InterceptedMethods");
        await Assert.That((object) testClass).IsNotNull();

        testClass.OuterMethod();

        await CheckMethod(
            Method.Init,
            [[1, "parameter", "property", "field"], [1, "parameter", "property", "field"]]);

        await CheckMethod(
            Method.OnExit,
            [[2, "parameter", "property", "field"], [2, "parameter", "property", "field"]]);
    }

    [Test]
    public async Task ShouldImplicitIntercept()
    {
        var testClass = WeaverHelperWrapper.Assembly.GetInstance("SimpleTest.PnP.InterceptedClass");
        await Assert.That((object) testClass).IsNotNull();

        testClass.ImplicitIntercepted();

        await CheckMethod(Method.Init, [1, "class_parameter", "class_property", "class_field"]);
    }

    [Test]
    public async Task ShouldPreferExplicitIntercept()
    {
        var testClass = WeaverHelperWrapper.Assembly.GetInstance("SimpleTest.PnP.InterceptedClass");
        await Assert.That((object) testClass).IsNotNull();

        testClass.ExplicitIntercepted();

        await CheckMethod(Method.Init, [10, "method_parameter", "method_property", "method_field"]);
    }

    [Test]
    public async Task ShouldInterceptRetval()
    {
        var testClass = WeaverHelperWrapper.Assembly.GetInstance("SimpleTest.PnP.InterceptedMethods");
        await Assert.That((object) testClass).IsNotNull();

        testClass.InterceptedReturns10();
        testClass.InterceptedReturnsString();
        testClass.InterceptedReturnsType();

        await CheckMethod(Method.OnExit, [[10], ["Intercepted"], [testClass.GetType()]]);
    }

    [Test]
    public async Task ShouldInterceptGenericRetval()
    {
        var testClass = WeaverHelperWrapper.Assembly.GetInstance("SimpleTest.PnP.InterceptedMethods");
        await Assert.That((object) testClass).IsNotNull();

        await Assert.That((object) testClass.GenericMethod<object>()).IsNotNull();

        await CheckMethod(Method.OnExit, ["string"]);
    }
}