using System.Reflection;

public class DecoratingConstructors :
    SimpleTestBase
{
    [Fact]
    public void ShouldReportOnEntryAndExit()
    {
        var testClass = WeaverHelperWrapper.Assembly.GetInstance("SimpleTest.InterceptingConstructors+SimpleConstructor");
        Assert.NotNull(testClass);
        CheckInit(null, "SimpleTest.InterceptingConstructors+SimpleConstructor..ctor");
        CheckMethodSeq([Method.Init, Method.OnEnter, Method.Body, Method.OnExit]);
    }

    [Fact]
    public void ShouldReportOnEntryAndException()
    {
        var exception =
            Record.Exception(
                () => WeaverHelperWrapper.Assembly.GetInstance("SimpleTest.InterceptingConstructors+ThrowingConstructor"));

        // This is because we're using reflection to create the instance.
        // It will wrap any exception
        if (exception is TargetInvocationException)
            exception = exception.InnerException;

        Assert.IsType<InvalidOperationException>(exception);

        CheckMethodSeq([Method.Init, Method.OnEnter, Method.OnException]);
        CheckInit(null, "SimpleTest.InterceptingConstructors+ThrowingConstructor..ctor");
        CheckException<InvalidOperationException>("Ooops");
    }
}