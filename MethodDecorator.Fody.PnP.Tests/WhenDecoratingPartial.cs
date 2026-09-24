public class WhenDecoratingPartial : SimpleTestBase
{
    [Test]
    public async Task ShouldInterceptInit1()
    {
        var testClass = WeaverHelperWrapper.Assembly.GetInstance("SimpleTest.PnP.InterceptedMethods");
        await Assert.That((object) testClass).IsNotNull();

        await Assert.That((object) testClass.InterceptedInit1(1)).IsEqualTo(2);

        await CheckMethod(Method.Init, ["InterceptedInit1"]);
    }

    [Test]
    public async Task ShouldInterceptInit2()
    {
        var testClass = WeaverHelperWrapper.Assembly.GetInstance("SimpleTest.PnP.InterceptedMethods");
        await Assert.That((object) testClass).IsNotNull();

        await Assert.That((object) testClass.InterceptedInit2(1)).IsEqualTo(2);

        await CheckMethod(Method.Init, [testClass, "InterceptedInit2"]);
    }

    [Test]
    public async Task ShouldInterceptInit3()
    {
        var testClass = WeaverHelperWrapper.Assembly.GetInstance("SimpleTest.PnP.InterceptedMethods");
        await Assert.That((object) testClass).IsNotNull();

        await Assert.That((object) testClass.InterceptedInit3(1)).IsEqualTo(2);

        await CheckMethod(Method.Init, [testClass, "InterceptedInit3", new object[] {1}]);
    }

    [Test]
    public async Task ShouldInterceptEntry()
    {
        var testClass = WeaverHelperWrapper.Assembly.GetInstance("SimpleTest.PnP.InterceptedMethods");
        await Assert.That((object) testClass).IsNotNull();

        await Assert.That((object) testClass.InterceptedEntry(1)).IsEqualTo(2);

        await CheckMethod(Method.OnEnter);
    }

    [Test]
    public async Task ShouldInterceptExit()
    {
        var testClass = WeaverHelperWrapper.Assembly.GetInstance("SimpleTest.PnP.InterceptedMethods");
        await Assert.That((object) testClass).IsNotNull();

        await Assert.That((object) testClass.InterceptedExit(1)).IsEqualTo(2);

        await CheckMethod(Method.OnExit);
    }

    [Test]
    public async Task ShouldInterceptExit1()
    {
        var testClass = WeaverHelperWrapper.Assembly.GetInstance("SimpleTest.PnP.InterceptedMethods");
        await Assert.That((object) testClass).IsNotNull();

        await Assert.That((object) testClass.InterceptedExit1(1)).IsEqualTo(2);

        await CheckMethod(Method.OnExit, [2]);
    }

    [Test]
    public async Task ShouldInterceptException()
    {
        var testClass = WeaverHelperWrapper.Assembly.GetInstance("SimpleTest.PnP.InterceptedMethods");
        await Assert.That((object) testClass).IsNotNull();

        try
        {
            testClass.InterceptedException(1);
        }
        catch (Exception e)
        {
            await Assert.That((object) e.Message).IsEqualTo("test");
        }

        await CheckMethod(Method.OnException, ["test"]);
    }

    [Test]
    public async Task ShouldInterceptExceptionExit1()
    {
        var testClass = WeaverHelperWrapper.Assembly.GetInstance("SimpleTest.PnP.InterceptedMethods");
        await Assert.That((object) testClass).IsNotNull();

        try
        {
            testClass.InterceptedExit1Exception(0);
        }
        catch (Exception e)
        {
            await Assert.That((object) e.Message).IsEqualTo("test");
        }

        await Assert.That((object) testClass.InterceptedExit1Exception(1)).IsEqualTo(2);

        await CheckMethod(Method.OnExit, [2]);
        await CheckMethod(Method.OnException, ["test"]);
    }

    [Test]
    public async Task ShouldBypassMethod()
    {
        var testClass = WeaverHelperWrapper.Assembly.GetInstance("SimpleTest.PnP.InterceptedMethods");
        await Assert.That((object) testClass).IsNotNull();

        testClass.BypassedMethod();

        await CheckMethodSeq([]);
    }

    [Test]
    public async Task ShouldNotBypassMethod()
    {
        var testClass = WeaverHelperWrapper.Assembly.GetInstance("SimpleTest.PnP.InterceptedMethods");
        await Assert.That((object) testClass).IsNotNull();

        testClass.NotBypassedMethod();

        await CheckMethod(Method.Body);
    }

    [Test]
    public async Task ShouldBypassBoolMethod()
    {
        var testClass = WeaverHelperWrapper.Assembly.GetInstance("SimpleTest.PnP.InterceptedMethods");
        await Assert.That((object) testClass).IsNotNull();

        await Assert.That((object) testClass.BypassedMethodRetTrue()).IsNotEqualTo(true);

        await CheckMethodSeq([]);
    }

    [Test]
    public async Task ShouldAlterString()
    {
        var testClass = WeaverHelperWrapper.Assembly.GetInstance("SimpleTest.PnP.InterceptedMethods");
        await Assert.That((object) testClass).IsNotNull();

        await Assert.That((object) testClass.AlteredMethodString()).IsEqualTo("altered");

        await CheckMethod(Method.Body);
    }

    [Test]
    public async Task ShouldAlterInt()
    {
        var testClass = WeaverHelperWrapper.Assembly.GetInstance("SimpleTest.PnP.InterceptedMethods");
        await Assert.That((object) testClass).IsNotNull();

        await Assert.That((object) testClass.AlteredMethodInt()).IsEqualTo(2);

        await CheckMethod(Method.Body);
    }

    [Test]
    public async Task ShouldAlterBypassString()
    {
        var testClass = WeaverHelperWrapper.Assembly.GetInstance("SimpleTest.PnP.InterceptedMethods");
        await Assert.That((object) testClass).IsNotNull();

        await Assert.That((object) testClass.AlteredBypassedMethodString()).IsEqualTo("altered");

        await CheckMethodSeq([]);
    }

    [Test]
    public async Task ShouldAlterBypassInt()
    {
        var testClass = WeaverHelperWrapper.Assembly.GetInstance("SimpleTest.PnP.InterceptedMethods");
        await Assert.That((object) testClass).IsNotNull();

        await Assert.That((object) testClass.AlteredBypassedMethodInt()).IsEqualTo(2);

        await CheckMethodSeq([]);
    }

    [Test]
    public async Task ShouldAlterBypassVoid()
    {
        var testClass = WeaverHelperWrapper.Assembly.GetInstance("SimpleTest.PnP.InterceptedMethods");
        await Assert.That((object) testClass).IsNotNull();

        testClass.AlteredBypassedMethodVoid();

        await CheckMethodSeq([]);
    }
}