public class WhenMatchingByRegex() :
    ClassTestsBase("SimpleTest.MatchingByRegex.MatchingByRegex")
{
    [Test]
    public async Task MethodMatchInclude()
    {
        TestClass.MethodMatchInclude();

        await CheckMethodSeq(
        [
            Method.Init, Method.OnEnter, Method.Body, Method.OnExit
        ]);

        await CheckBody("MethodMatchInclude");
    }

    [Test]
    public async Task MethodMatchExclude()
    {
        TestClass.MethodMatchExclude();

        await CheckMethodSeq([Method.Body]);

        await CheckBody("MethodMatchExclude");
    }

    [Test]
    public async Task PropertyGetInclude()
    {
        object dummy = TestClass.PropertyGetInclude;

        await CheckMethodSeq(
        [
            Method.Init, Method.OnEnter, Method.Body, Method.OnExit
        ]);

        await CheckBody("PropertyGetInclude");
    }

    [Test]
    public async Task PropertyGetExclude()
    {
        object dummy = TestClass.PropertyGetExclude;

        await CheckMethodSeq([Method.Body]);

        await CheckBody("PropertyGetExclude");
    }
}