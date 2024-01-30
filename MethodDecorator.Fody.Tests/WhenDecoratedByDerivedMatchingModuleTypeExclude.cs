public class WhenDecoratedByDerivedMatchingModuleTypeExclude() :
    ClassTestsBase("SimpleTest.DerivedMatchingModule.DerivedMatchingModuleTypeExclude")
{
    [Fact]
    public void ConstructorTrigger()
    {
        var m = TestClass;
        CheckMethodSeq([]);
    }

    [Fact]
    public void ExcludeAtTypeLevel()
    {
        TestClass.ExcludeAtTypeLevel();
        CheckMethodSeq([Method.Body]);

        CheckBody("ExcludeAtTypeLevel");
    }

    [Fact]
    public void ReIncludeAtMethodLevel()
    {
        TestClass.ReIncludeAtMethodLevel();
        CheckMethodSeq(
        [
            Method.Init, Method.OnEnter, Method.Body, Method.OnExit
        ]);

        CheckBody("ReIncludeAtMethodLevel");
    }
}