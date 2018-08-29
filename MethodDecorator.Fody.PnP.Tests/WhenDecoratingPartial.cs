using System;
using Xunit;

public class WhenDecoratingPartial : SimpleTestBase
{
    [Fact]
    public void ShouldInterceptInit1()
    {
        var testClass = WeaverHelperWrapper.Assembly.GetInstance("SimpleTest.PnP.InterceptedMethods");
        Assert.NotNull(testClass);

        Assert.Equal(2, testClass.InterceptedInit1(1));

        CheckMethod(Method.Init, new object[] {"InterceptedInit1"});
    }

    [Fact]
    public void ShouldInterceptInit2()
    {
        var testClass = WeaverHelperWrapper.Assembly.GetInstance("SimpleTest.PnP.InterceptedMethods");
        Assert.NotNull(testClass);

        Assert.Equal(2, testClass.InterceptedInit2(1));

        CheckMethod(Method.Init, new object[] {testClass, "InterceptedInit2"});
    }

    [Fact]
    public void ShouldInterceptInit3()
    {
        var testClass = WeaverHelperWrapper.Assembly.GetInstance("SimpleTest.PnP.InterceptedMethods");
        Assert.NotNull(testClass);

        Assert.Equal(2, testClass.InterceptedInit3(1));

        CheckMethod(Method.Init, new object[] {testClass, "InterceptedInit3", new object[] {1}});
    }

    [Fact]
    public void ShouldInterceptEntry()
    {
        var testClass = WeaverHelperWrapper.Assembly.GetInstance("SimpleTest.PnP.InterceptedMethods");
        Assert.NotNull(testClass);

        Assert.Equal(2, testClass.InterceptedEntry(1));

        CheckMethod(Method.OnEnter);
    }

    [Fact]
    public void ShouldInterceptExit()
    {
        var testClass = WeaverHelperWrapper.Assembly.GetInstance("SimpleTest.PnP.InterceptedMethods");
        Assert.NotNull(testClass);

        Assert.Equal(2, testClass.InterceptedExit(1));

        CheckMethod(Method.OnExit);
    }

    [Fact]
    public void ShouldInterceptExit1()
    {
        var testClass = WeaverHelperWrapper.Assembly.GetInstance("SimpleTest.PnP.InterceptedMethods");
        Assert.NotNull(testClass);

        Assert.Equal(2, testClass.InterceptedExit1(1));

        CheckMethod(Method.OnExit, new object[] {2});
    }

    [Fact]
    public void ShouldInterceptException()
    {
        var testClass = WeaverHelperWrapper.Assembly.GetInstance("SimpleTest.PnP.InterceptedMethods");
        Assert.NotNull(testClass);

        try
        {
            testClass.InterceptedException(1);
        }
        catch (Exception e)
        {
            Assert.Equal("test", e.Message);
        }

        CheckMethod(Method.OnException, new object[] {"test"});
    }

    [Fact]
    public void ShouldInterceptExceptionExit1()
    {
        var testClass = WeaverHelperWrapper.Assembly.GetInstance("SimpleTest.PnP.InterceptedMethods");
        Assert.NotNull(testClass);

        try
        {
            testClass.InterceptedExit1Exception(0);
        }
        catch (Exception e)
        {
            Assert.Equal("test", e.Message);
        }

        Assert.Equal(2, testClass.InterceptedExit1Exception(1));

        CheckMethod(Method.OnExit, new object[] {2});
        CheckMethod(Method.OnException, new object[] {"test"});
    }

    [Fact]
    public void ShouldBypassMethod()
    {
        var testClass = WeaverHelperWrapper.Assembly.GetInstance("SimpleTest.PnP.InterceptedMethods");
        Assert.NotNull(testClass);

        testClass.BypassedMethod();

        CheckMethodSeq(new Method[] { });
    }

    [Fact]
    public void ShouldNotBypassMethod()
    {
        var testClass = WeaverHelperWrapper.Assembly.GetInstance("SimpleTest.PnP.InterceptedMethods");
        Assert.NotNull(testClass);

        testClass.NotBypassedMethod();

        CheckMethod(Method.Body);
    }

    [Fact]
    public void ShouldBypassBoolMethod()
    {
        var testClass = WeaverHelperWrapper.Assembly.GetInstance("SimpleTest.PnP.InterceptedMethods");
        Assert.NotNull(testClass);

        Assert.NotEqual(testClass.BypassedMethodRetTrue(), true);

        CheckMethodSeq(new Method[] { });
    }

    [Fact]
    public void ShouldAlterString()
    {
        var testClass = WeaverHelperWrapper.Assembly.GetInstance("SimpleTest.PnP.InterceptedMethods");
        Assert.NotNull(testClass);

        Assert.Equal(testClass.AlteredMethodString(), "altered");

        CheckMethod(Method.Body);
    }

    [Fact]
    public void ShouldAlterInt()
    {
        var testClass = WeaverHelperWrapper.Assembly.GetInstance("SimpleTest.PnP.InterceptedMethods");
        Assert.NotNull(testClass);

        Assert.Equal(testClass.AlteredMethodInt(), 2);

        CheckMethod(Method.Body);
    }

    [Fact]
    public void ShouldAlterBypassString()
    {
        var testClass = WeaverHelperWrapper.Assembly.GetInstance("SimpleTest.PnP.InterceptedMethods");
        Assert.NotNull(testClass);

        Assert.Equal(testClass.AlteredBypassedMethodString(), "altered");

        CheckMethodSeq(new Method[] { });
    }

    [Fact]
    public void ShouldAlterBypassInt()
    {
        var testClass = WeaverHelperWrapper.Assembly.GetInstance("SimpleTest.PnP.InterceptedMethods");
        Assert.NotNull(testClass);

        Assert.Equal(testClass.AlteredBypassedMethodInt(), 2);

        CheckMethodSeq(new Method[] { });
    }

    [Fact]
    public void ShouldAlterBypassVoid()
    {
        var testClass = WeaverHelperWrapper.Assembly.GetInstance("SimpleTest.PnP.InterceptedMethods");
        Assert.NotNull(testClass);

        testClass.AlteredBypassedMethodVoid();

        CheckMethodSeq(new Method[] { });
    }
}