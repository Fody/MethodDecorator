
// tests share the static TestRecords of the weaved assembly
[NotInParallel]
public class SimpleTestBase : TestsBase
{
    public SimpleTestBase()
    {
        WeaverHelperWrapper.Assembly.GetStaticInstance("SimpleTest.TestRecords").Clear();
    }

    protected override dynamic RecordHost
    {
        get { return WeaverHelperWrapper.Assembly.GetStaticInstance("SimpleTest.TestRecords"); }
    }
}