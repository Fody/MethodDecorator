using System;
using Xunit;

public class WhenDecoratingFields : SimpleTestBase
{
    [Fact]
    public void ShouldBypassFieldInitCalls()
    {
        dynamic testClass = WeaverHelperWrapper.Assembly.GetType("SimpleTest.PnP.InterceptedMethods", true);
        Assert.NotNull(testClass);

        Activator.CreateInstance(testClass, "Test");

        CheckMethod(Method.Init, new object[] {11, "parameter", "property", "field"});
    }

    [Fact]
    public void ShouldBypassCtorCalls()
    {
        dynamic testClass = WeaverHelperWrapper.Assembly.GetType("SimpleTest.PnP.InterceptedMethods", true);
        Assert.NotNull(testClass);

        Activator.CreateInstance(testClass, 1);

        CheckMethod(Method.Init, new[]
        {
            new object[] {11, "parameter", "property", "field"},
            new object[] {12, "parameter", "property", "field"}
        });
    }

    [Fact]
    public void ShouldFixJumps()
    {
        var testClass = WeaverHelperWrapper.Assembly.GetInstance("SimpleTest.PnP.InterceptedMethods");
        Assert.NotNull(testClass);

        Assert.Equal(13, testClass.SomeLongMethod());

        CheckMethod(Method.Init, new object[] {0, null, null, null});
    }

    [Fact]
    public void ShouldAllow255Locals()
    {
        var testClass = WeaverHelperWrapper.Assembly.GetInstance("SimpleTest.PnP.InterceptedMethods");
        Assert.NotNull(testClass);

        testClass.MethodWith255Locals();

        CheckMethod(Method.OnEnter);
        CheckMethod(Method.OnExit, new object[] {260});
    }

    [Fact]
    public void ShouldChangePriority()
    {
        var testClass = WeaverHelperWrapper.Assembly.GetInstance("SimpleTest.PnP.InterceptedMethods");
        Assert.NotNull(testClass);

        testClass.InterceptedWithoutPriorities();
        CheckMethod(Method.Init, new[] {new object[] {1, "Attr2", null, null}, new object[] {"Attr1", 0, 0}});
        RecordHost.Clear();

        testClass.InterceptedWithPriorities();
        CheckMethod(Method.Init, new[] {new object[] {"Attr1", -1, 0}, new object[] {1, "Attr2", null, null}});
    }

    //TODO: debug these
    //[Fact]
    //public void MultipleInterceptedWithPriority()
    //{
    //    var testClass = WeaverHelperWrapper.Assembly.GetInstance("SimpleTest.PnP.InterceptedMethods");
    //    Assert.NotNull(testClass);

    //    testClass.MultipleInterceptedWithPriority();

    //    CheckMethod(Method.Init, new object[] {"attr5", 0, 5});
    //}

    //[Fact]
    //public void ShouldPreferLastAttribute()
    //{
    //    var testClass = WeaverHelperWrapper.Assembly.GetInstance("SimpleTest.PnP.InterceptedMethods");
    //    Assert.NotNull(testClass);

    //    testClass.MultipleIntercepted();

    //    CheckMethod(Method.Init, new object[] {"attr3", 0, 0});
    //}

    [Fact]
    public void ShouldInterceptImplicitCastReturn()
    {
        var testClass = WeaverHelperWrapper.Assembly.GetInstance("SimpleTest.PnP.InterceptedMethods");
        Assert.NotNull(testClass);

        IDisposable ret = testClass.InterceptedReturnsImplicitCasted();
        Assert.NotNull(ret);
    }
}