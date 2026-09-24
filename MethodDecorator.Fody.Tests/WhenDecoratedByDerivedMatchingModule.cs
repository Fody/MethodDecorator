public class WhenDecoratedByDerivedMatchingModule() :
    ClassTestsBase("SimpleTest.DerivedMatchingModule.DerivedMatchingModule")
{
    [Test]
    public async Task ConstructorTrigger()
    {
        var m = TestClass;
        await CheckMethodSeq([Method.Init, Method.OnEnter, Method.OnExit]);
    }

    [Test]
    public async Task AppliesToNamespace()
    {
        TestClass.AppliesToNamespace();
        await CheckMethodSeq(
        [
            Method.Init, Method.OnEnter, Method.OnExit, // Constructor
            Method.Init, Method.OnEnter, Method.Body, Method.OnExit
        ]); // AppliesToNamespace()

        await CheckBody("AppliesToNamespace");

    }

    [Test]
    public async Task TurnOffAtMethodLevel()
    {
        TestClass.TurnOffAtMethodLevel();
        await CheckMethodSeq(
        [
            Method.Init, Method.OnEnter, Method.OnExit, // Constructor
            Method.Body
        ]); // Nothing in body

        await CheckBody("TurnOffAtMethodLevel");
    }
}