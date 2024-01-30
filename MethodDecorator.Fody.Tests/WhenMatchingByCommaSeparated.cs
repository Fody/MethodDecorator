public class WhenMatchingByCommaSeparatedA() :
    ClassTestsBase("SimpleTest.MatchingCommaSeparated.MatchingCommaSeparatedA")
{
    [Fact]
    public void AppliesToNamespace()
    {
        TestClass.AppliesToNamespace();

        CheckMethodSeq(
        [
            Method.Init, Method.OnEnter, Method.OnExit,
            Method.Init, Method.OnEnter, Method.Body, Method.OnExit
        ]);

        CheckBody("AppliesToNamespace");
    }
}

public class WhenMatchingByCommaSeparatedB : ClassTestsBase
{
    public WhenMatchingByCommaSeparatedB()
        : base("SimpleTest.MatchingCommaSeparated.MatchingCommaSeparatedB")
    {
    }

    [Fact]
    public void AppliesToNamespace()
    {
        TestClass.AppliesToNamespace();

        CheckMethodSeq([
            Method.Init, Method.OnEnter, Method.OnExit,
            Method.Init, Method.OnEnter, Method.Body, Method.OnExit
        ]);

        CheckBody("AppliesToNamespace");
    }
}