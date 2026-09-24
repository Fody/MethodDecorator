using System.Reflection;

public class DecoratingConstructors :
    SimpleTestBase
{
    [Test]
    public async Task ShouldReportOnEntryAndExit()
    {
        var testClass = WeaverHelperWrapper.Assembly.GetInstance("SimpleTest.InterceptingConstructors+SimpleConstructor");
        await Assert.That((object) testClass).IsNotNull();
        await CheckInit(null, "SimpleTest.InterceptingConstructors+SimpleConstructor..ctor");
        await CheckMethodSeq([Method.Init, Method.OnEnter, Method.Body, Method.OnExit]);
    }

    [Test]
    public async Task ShouldReportOnEntryAndException()
    {
        Exception exception = null;
        try
        {
            WeaverHelperWrapper.Assembly.GetInstance("SimpleTest.InterceptingConstructors+ThrowingConstructor");
        }
        catch (Exception caught)
        {
            exception = caught;
        }

        // This is because we're using reflection to create the instance.
        // It will wrap any exception
        if (exception is TargetInvocationException)
            exception = exception.InnerException;

        await Assert.That(exception).IsTypeOf<InvalidOperationException>();

        await CheckMethodSeq([Method.Init, Method.OnEnter, Method.OnException]);
        await CheckInit(null, "SimpleTest.InterceptingConstructors+ThrowingConstructor..ctor");
        await CheckException<InvalidOperationException>("Ooops");
    }
}