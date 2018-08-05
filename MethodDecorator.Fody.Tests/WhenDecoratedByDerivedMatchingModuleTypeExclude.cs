using Xunit;

public class WhenDecoratedByDerivedMatchingModuleTypeExclude : ClassTestsBase
{
    public WhenDecoratedByDerivedMatchingModuleTypeExclude()
        : base("SimpleTest.DerivedMatchingModule.DerivedMatchingModuleTypeExclude")
    {
    }

    [Fact]
    public void ConstructorTrigger()
    {
        var m = TestClass;
        CheckMethodSeq(new Method[] { });
    }

    [Fact]
    public void ExcludeAtTypeLevel()
    {
        TestClass.ExcludeAtTypeLevel();
        CheckMethodSeq(new[]
        {
            Method.Body
        });

        CheckBody("ExcludeAtTypeLevel");
    }

    [Fact]
    public void ReIncludeAtMethodLevel()
    {
        TestClass.ReIncludeAtMethodLevel();
        CheckMethodSeq(new[]
        {
            Method.Init, Method.OnEnter, Method.Body, Method.OnExit
        });

        CheckBody("ReIncludeAtMethodLevel");
    }
}