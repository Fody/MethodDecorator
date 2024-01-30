public class WhenDecoratedByDerivedMatchingModule() :
    ClassTestsBase("SimpleTest.DerivedMatchingModule.DerivedMatchingModule")
{
    [Fact]
    public void ConstructorTrigger()
    {
        var m = TestClass;
        CheckMethodSeq([Method.Init, Method.OnEnter, Method.OnExit]);
    }

    [Fact]
    public void AppliesToNamespace()
    {
        TestClass.AppliesToNamespace();
        CheckMethodSeq(
        [
            Method.Init, Method.OnEnter, Method.OnExit, // Constructor
            Method.Init, Method.OnEnter, Method.Body, Method.OnExit
        ]); // AppliesToNamespace()

        CheckBody("AppliesToNamespace");

    }

    [Fact]
    public void TurnOffAtMethodLevel()
    {
        TestClass.TurnOffAtMethodLevel();
        CheckMethodSeq(
        [
            Method.Init, Method.OnEnter, Method.OnExit, // Constructor
            Method.Body
        ]); // Nothing in body

        CheckBody("TurnOffAtMethodLevel");
    }
}