public class WhenMatchingByCommaSeparatedA() :
    ClassTestsBase("SimpleTest.MatchingCommaSeparated.MatchingCommaSeparatedA")
{
    [Test]
    public async Task AppliesToNamespace()
    {
        TestClass.AppliesToNamespace();

        await CheckMethodSeq(
        [
            Method.Init, Method.OnEnter, Method.OnExit,
            Method.Init, Method.OnEnter, Method.Body, Method.OnExit
        ]);

        await CheckBody("AppliesToNamespace");
    }
}

public class WhenMatchingByCommaSeparatedB : ClassTestsBase
{
    public WhenMatchingByCommaSeparatedB()
        : base("SimpleTest.MatchingCommaSeparated.MatchingCommaSeparatedB")
    {
    }

    [Test]
    public async Task AppliesToNamespace()
    {
        TestClass.AppliesToNamespace();

        await CheckMethodSeq([
            Method.Init, Method.OnEnter, Method.OnExit,
            Method.Init, Method.OnEnter, Method.Body, Method.OnExit
        ]);

        await CheckBody("AppliesToNamespace");
    }
}