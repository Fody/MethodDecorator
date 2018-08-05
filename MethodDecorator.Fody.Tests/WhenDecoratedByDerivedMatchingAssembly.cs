using Xunit;

public class WhenDecoratedByDerivedMatchingAssembly : ClassTestsBase
{
    public WhenDecoratedByDerivedMatchingAssembly()
        : base("SimpleTest.DerivedMatchingAssembly.DerivedMatchingAssembly")
    {
    }

    [Fact]
    public void ConstructorTrigger()
    {
        var m = TestClass;
        CheckMethodSeq(new[] {Method.Init, Method.OnEnter, Method.OnExit});
    }

    [Fact]
    public void AppliesToNamespace()
    {
        TestClass.AppliesToNamespace();
        CheckMethodSeq(new[]
        {
            Method.Init, Method.OnEnter, Method.OnExit, // Constructor
            Method.Init, Method.OnEnter, Method.Body, Method.OnExit
        }); // AppliesToNamespace()

        CheckBody("AppliesToNamespace");

    }

    [Fact]
    public void TurnOffAtMethodLevel()
    {
        TestClass.TurnOffAtMethodLevel();
        CheckMethodSeq(new[]
        {
            Method.Init, Method.OnEnter, Method.OnExit, // Constructor
            Method.Body
        }); // Nothing in body

        CheckBody("TurnOffAtMethodLevel");
    }

}