using Xunit;

public class WhenMatchingByRegex : ClassTestsBase
{
    public WhenMatchingByRegex()
        : base("SimpleTest.MatchingByRegex.MatchingByRegex")
    {
    }

    [Fact]
    public void MethodMatchInclude()
    {
        TestClass.MethodMatchInclude();

        CheckMethodSeq(new[]
        {
            Method.Init, Method.OnEnter, Method.Body, Method.OnExit
        });

        CheckBody("MethodMatchInclude");
    }

    [Fact]
    public void MethodMatchExclude()
    {
        TestClass.MethodMatchExclude();

        CheckMethodSeq(new[]
        {
            Method.Body
        });

        CheckBody("MethodMatchExclude");
    }

    [Fact]
    public void PropertyGetInclude()
    {
        object dummy = TestClass.PropertyGetInclude;

        CheckMethodSeq(new[]
        {
            Method.Init, Method.OnEnter, Method.Body, Method.OnExit
        });

        CheckBody("PropertyGetInclude");
    }

    [Fact]
    public void PropertyGetExclude()
    {
        object dummy = TestClass.PropertyGetExclude;

        CheckMethodSeq(new[]
        {
            Method.Body
        });

        CheckBody("PropertyGetExclude");
    }
}