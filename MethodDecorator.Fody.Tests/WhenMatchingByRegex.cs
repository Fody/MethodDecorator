public class WhenMatchingByRegex() :
    ClassTestsBase("SimpleTest.MatchingByRegex.MatchingByRegex")
{
    [Fact]
    public void MethodMatchInclude()
    {
        TestClass.MethodMatchInclude();

        CheckMethodSeq(
        [
            Method.Init, Method.OnEnter, Method.Body, Method.OnExit
        ]);

        CheckBody("MethodMatchInclude");
    }

    [Fact]
    public void MethodMatchExclude()
    {
        TestClass.MethodMatchExclude();

        CheckMethodSeq([Method.Body]);

        CheckBody("MethodMatchExclude");
    }

    [Fact]
    public void PropertyGetInclude()
    {
        object dummy = TestClass.PropertyGetInclude;

        CheckMethodSeq(
        [
            Method.Init, Method.OnEnter, Method.Body, Method.OnExit
        ]);

        CheckBody("PropertyGetInclude");
    }

    [Fact]
    public void PropertyGetExclude()
    {
        object dummy = TestClass.PropertyGetExclude;

        CheckMethodSeq([Method.Body]);

        CheckBody("PropertyGetExclude");
    }
}