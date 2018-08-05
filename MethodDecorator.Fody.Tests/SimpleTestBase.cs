using System.Reflection;

public class SimpleTestBase : TestsBase
{
    protected static readonly Assembly assembly = CreateAssembly();

    public SimpleTestBase()
    {
        assembly.GetStaticInstance("SimpleTest.TestRecords").Clear();
    }

    protected override Assembly Assembly
    {
        get { return assembly; }
    }

    protected override dynamic RecordHost
    {
        get { return Assembly.GetStaticInstance("SimpleTest.TestRecords"); }
    }

    private static Assembly CreateAssembly()
    {
        var weaverHelper = new WeaverHelper(@"SimpleTest\SimpleTest.csproj");
        return weaverHelper.Weave();
    }
}