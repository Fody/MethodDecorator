
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