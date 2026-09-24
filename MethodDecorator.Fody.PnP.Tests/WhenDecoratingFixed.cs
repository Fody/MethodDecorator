public class WhenDecoratingFields : SimpleTestBase
{
    [Test]
    public async Task ShouldBypassFieldInitCalls()
    {
        dynamic testClass = WeaverHelperWrapper.Assembly.GetType("SimpleTest.PnP.InterceptedMethods", true);
        await Assert.That((object) testClass).IsNotNull();

        Activator.CreateInstance(testClass, "Test");

        await CheckMethod(Method.Init, [11, "parameter", "property", "field"]);
    }

    [Test]
    public async Task ShouldBypassCtorCalls()
    {
        dynamic testClass = WeaverHelperWrapper.Assembly.GetType("SimpleTest.PnP.InterceptedMethods", true);
        await Assert.That((object) testClass).IsNotNull();

        Activator.CreateInstance(testClass, 1);

        await CheckMethod(
            Method.Init,
            [
                [11, "parameter", "property", "field"],
                [12, "parameter", "property", "field"]
            ]);
    }

    [Test]
    public async Task ShouldFixJumps()
    {
        var testClass = WeaverHelperWrapper.Assembly.GetInstance("SimpleTest.PnP.InterceptedMethods");
        await Assert.That((object) testClass).IsNotNull();

        await Assert.That((object) testClass.SomeLongMethod()).IsEqualTo(13);

        await CheckMethod(Method.Init, [0, null, null, null]);
    }

    [Test]
    public async Task ShouldAllow255Locals()
    {
        var testClass = WeaverHelperWrapper.Assembly.GetInstance("SimpleTest.PnP.InterceptedMethods");
        await Assert.That((object) testClass).IsNotNull();

        testClass.MethodWith255Locals();

        await CheckMethod(Method.OnEnter);
        await CheckMethod(Method.OnExit, [260]);
    }

    [Test]
    public async Task ShouldChangePriority()
    {
        var testClass = WeaverHelperWrapper.Assembly.GetInstance("SimpleTest.PnP.InterceptedMethods");
        await Assert.That((object) testClass).IsNotNull();

        testClass.InterceptedWithoutPriorities();
        await CheckMethod(Method.Init, [[1, "Attr2", null, null], ["Attr1", 0, 0]]);
        RecordHost.Clear();

        testClass.InterceptedWithPriorities();
        await CheckMethod(Method.Init, [["Attr1", -1, 0], [1, "Attr2", null, null]]);
    }

    //TODO: debug these
    //[Fact]
    //public void MultipleInterceptedWithPriority()
    //{
    //    var testClass = WeaverHelperWrapper.Assembly.GetInstance("SimpleTest.PnP.InterceptedMethods");
    //    await Assert.That((object) testClass).IsNotNull();

    //    testClass.MultipleInterceptedWithPriority();

    //    CheckMethod(Method.Init, new object[] {"attr5", 0, 5});
    //}

    //[Fact]
    //public void ShouldPreferLastAttribute()
    //{
    //    var testClass = WeaverHelperWrapper.Assembly.GetInstance("SimpleTest.PnP.InterceptedMethods");
    //    await Assert.That((object) testClass).IsNotNull();

    //    testClass.MultipleIntercepted();

    //    CheckMethod(Method.Init, new object[] {"attr3", 0, 0});
    //}

    [Test]
    public async Task ShouldInterceptImplicitCastReturn()
    {
        var testClass = WeaverHelperWrapper.Assembly.GetInstance("SimpleTest.PnP.InterceptedMethods");
        await Assert.That((object) testClass).IsNotNull();

        IDisposable ret = testClass.InterceptedReturnsImplicitCasted();
        await Assert.That((object) ret).IsNotNull();
    }
}