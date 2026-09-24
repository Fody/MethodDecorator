public class WhenDecoratedByDerivedMatchingModuleTypeExclude() :
    ClassTestsBase("SimpleTest.DerivedMatchingModule.DerivedMatchingModuleTypeExclude")
{
    [Test]
    public async Task ConstructorTrigger()
    {
        var m = TestClass;
        await CheckMethodSeq([]);
    }

    [Test]
    public async Task ExcludeAtTypeLevel()
    {
        TestClass.ExcludeAtTypeLevel();
        await CheckMethodSeq([Method.Body]);

        await CheckBody("ExcludeAtTypeLevel");
    }

    [Test]
    public async Task ReIncludeAtMethodLevel()
    {
        TestClass.ReIncludeAtMethodLevel();
        await CheckMethodSeq(
        [
            Method.Init, Method.OnEnter, Method.Body, Method.OnExit
        ]);

        await CheckBody("ReIncludeAtMethodLevel");
    }
}