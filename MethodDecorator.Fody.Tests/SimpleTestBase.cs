using System.Reflection;

public class SimpleTestBase : TestsBase
{
    public static Assembly Assembly { get; }
    public SimpleTestBase()
    {
        Assembly.GetStaticInstance("SimpleTest.TestRecords").Clear();
    }

    protected override dynamic RecordHost
    {
        get { return Assembly.GetStaticInstance("SimpleTest.TestRecords"); }
    }

    static SimpleTestBase()
    {
        var weaverHelper = new WeaverHelper(@"SimpleTest\SimpleTest.csproj");
        Assembly= weaverHelper.Weave();
    }
}