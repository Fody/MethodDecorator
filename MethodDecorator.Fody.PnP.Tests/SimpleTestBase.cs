public class SimpleTestBase : TestsBase
{
    public SimpleTestBase()
    {
        WeaverHelperWrapper.Assembly.GetStaticInstance("SimpleTest.PnP.TestRecords").Clear();
    }

    protected override dynamic RecordHost
    {
        get { return WeaverHelperWrapper.Assembly.GetStaticInstance("SimpleTest.PnP.TestRecords"); }
    }

    protected void CheckMethod(Method iMethod)
    {
        var record = Records.SingleOrDefault(_ => _.Item1 == iMethod);
        Assert.NotNull(record);
        Assert.Null(record.Item2);
    }

    protected void CheckMethod(Method iMethod, object[] iParams)
    {
        var record = Records.SingleOrDefault(_ => _.Item1 == iMethod);
        Assert.NotNull(record);
        Assert.Equal(iParams, record.Item2);
    }

    protected void CheckMethod(Method iMethod, object[][] iParams)
    {
        var records = Records.Where(_ => _.Item1 == iMethod).Select(_ => _.Item2);
        Assert.NotEmpty(records);
        Assert.Equal(iParams, records);
    }
}